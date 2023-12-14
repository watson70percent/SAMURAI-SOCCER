using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace SamuraiSoccer
{
    public static class BurstMeshCut
    {
        public async static UniTask<IList<Mesh>> CutMesh(GameObject target, Vector3 planeAnchorPoint, Vector3 planeNormal)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await UniTask.SwitchToMainThread();
            var targetMesh = target.GetComponent<MeshFilter>().mesh;
            var dataArray = Mesh.AcquireReadOnlyMeshData(targetMesh);
            var dir = target.transform.InverseTransformDirection(planeNormal).normalized;
            var pos = target.transform.InverseTransformPoint(planeAnchorPoint);
            var newMeshArray = CutMesh(dataArray, pos, dir);
            dataArray.Dispose();
            var meshs = new List<Mesh>();
            var fmesh = new Mesh();
            fmesh.name = "Split Mesh front";
            meshs.Add(fmesh);
            var bmesh = new Mesh();
            bmesh.name = "Split Mesh back";
            meshs.Add(bmesh);

            Mesh.ApplyAndDisposeWritableMeshData(newMeshArray, meshs);
            sw.Stop();
            Debug.Log("cut time = " + sw.Elapsed);
            return meshs;
        }

        [RuntimeInitializeOnLoadMethod]
        private static async UniTask WarmUp()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await UniTask.SwitchToMainThread();
            var dataArray = Mesh.AllocateWritableMeshData(1);
            var data = dataArray[0];
            data.SetVertexBufferParams(3,
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2));
            var vdata = data.GetVertexData<VertexData>();
            vdata[0] = new VertexData(new Vector3(1, 1, 1), Vector3.zero, Vector2.zero);
            vdata[1] = new VertexData(new Vector3(-1, -1, -1), Vector3.zero, Vector2.zero);
            vdata[2] = new VertexData(new Vector3(1, 1, -1), Vector3.zero, Vector2.zero);
            data.SetIndexBufferParams(3, IndexFormat.UInt16);
            data.subMeshCount = 1;
            var idata = data.GetIndexData<ushort>();
            idata[0] = 0;
            idata[1] = 1;
            idata[2] = 2;
            data.SetSubMesh(0, new SubMeshDescriptor(0, 3));
            var m = CutMesh(dataArray, Vector3.zero, Vector3.up);
            dataArray.Dispose();
            m.Dispose();

            sw.Stop();
            Debug.Log("wake up = " + sw.Elapsed);
        }

        private static Mesh.MeshDataArray CutMesh(in Mesh.MeshDataArray dataArray, Vector3 pos, Vector3 dir)
        {
            var disposable = new List<IDisposable>();
            var newMeshArray = Mesh.AllocateWritableMeshData(2);
            try
            {
                var data = dataArray[0];
                var sideJudge = new VertexSideJudgeJob();
                var vertex = new NativeArray<Vector3>(data.vertexCount, Allocator.TempJob);
                disposable.Add(vertex);
                data.GetVertices(vertex);

                sideJudge.vertex = vertex;
                sideJudge.dir = dir;
                sideJudge.offset = -Vector3.Dot(dir, pos);
                sideJudge.side = new NativeArray<byte>(data.vertexCount, Allocator.TempJob);
                sideJudge.normal = new NativeArray<float>(data.vertexCount, Allocator.TempJob);
                var sideHandler = sideJudge.Schedule(vertex.Length, 0);
                disposable.Add(sideJudge);
                var triCount = data.vertexCount * 2 / 3;

                var meshJobs = Enumerable.Range(0, data.subMeshCount).Select(sub =>
                {
                    var subMeshInfo = data.GetSubMesh(sub);
                    var subMeshIndexes = new NativeArray<int>(subMeshInfo.indexCount, Allocator.TempJob);
                    data.GetIndices(subMeshIndexes, sub);
                    var meshJudge = new MeshSideJudgeJob();
                    meshJudge.side = sideJudge.side;
                    meshJudge.indexes = subMeshIndexes;
                    meshJudge.forward = new NativeList<int>(triCount, Allocator.TempJob);
                    meshJudge.backward = new NativeList<int>(triCount, Allocator.TempJob);
                    meshJudge.center = new NativeList<int>(triCount, Allocator.TempJob);
                    var meshHandler = meshJudge.Schedule(sideHandler);
                    disposable.Add(meshJudge);
                    disposable.Add(subMeshIndexes);
                    return (meshJudge, meshHandler);
                }).ToArray();

                var sHandles = new NativeArray<JobHandle>(meshJobs.Select(job => job.meshHandler).ToArray(), Allocator.TempJob);
                var sAllHandler = JobHandle.CombineDependencies(sHandles);
                disposable.Add(sHandles);

                var indexJobs = meshJobs.Select(job =>
                {
                    var indexJob = new IndexJob();
                    indexJob.center = job.meshJudge.center;
                    indexJob.vertexIndex = new NativeParallelMultiHashMap<int, int>(0, Allocator.TempJob);
                    var indexHandler = indexJob.Schedule(job.meshHandler);
                    disposable.Add(indexJob);
                    return (indexJob, indexHandler);
                }).ToArray();

                var divideJobs = indexJobs.Zip(meshJobs, (i, m) => (i.indexJob, m.meshJudge, i.indexHandler)).Select(job =>
                {
                    var divideJob = new DivideJob();
                    divideJob.normal = sideJudge.normal;
                    divideJob.side = sideJudge.side;
                    divideJob.vertexPos = vertex;
                    divideJob.center = job.meshJudge.center;
                    divideJob.vertexIndex = job.indexJob.vertexIndex;
                    divideJob.divideP = new NativeList<DividePoint>(Allocator.TempJob);
                    divideJob.divideInfo = new NativeList<DivideInfo>(Allocator.TempJob);
                    var divideHandler = divideJob.Schedule(job.indexHandler);
                    disposable.Add(divideJob);
                    return (divideJob, divideHandler);
                }).ToArray();

                var triangleJobs = divideJobs.Zip(meshJobs, (d, m) => (d.divideJob, m.meshJudge, d.divideHandler)).Select(job =>
                {
                    var triangleJob = new MakeTriangleJob();
                    triangleJob.center = job.meshJudge.center;
                    triangleJob.divideInfos = job.divideJob.divideInfo;
                    triangleJob.forward = new NativeList<int>(Allocator.TempJob);
                    triangleJob.existForward = new NativeList<int>(Allocator.TempJob);
                    triangleJob.backward = new NativeList<int>(Allocator.TempJob);
                    triangleJob.existBackward = new NativeList<int>(Allocator.TempJob);
                    triangleJob.forwardDivideIndex = new NativeParallelHashSet<int>(0, Allocator.TempJob);
                    triangleJob.backwardDivideIndex = new NativeParallelHashSet<int>(0, Allocator.TempJob);
                    var triangleHandler = triangleJob.Schedule(job.divideHandler);
                    disposable.Add(triangleJob);
                    return (triangleJob, triangleHandler);
                }).ToArray();

                var dHandles = new NativeArray<JobHandle>(triangleJobs.Select(job => job.triangleHandler).ToArray(), Allocator.TempJob);
                var dAllHandler = JobHandle.CombineDependencies(dHandles);
                disposable.Add(dHandles);

                JobHandle.CompleteAll(ref dAllHandler, ref sAllHandler);

                using var _forwardIndexes = new NativeList<int>(meshJobs.Select(job => job.meshJudge.forward.Length).Sum() + triangleJobs.Select(job => job.triangleJob.forward.Length).Sum(), Allocator.Temp);
                using var _backwardIndexes = new NativeList<int>(meshJobs.Select(job => job.meshJudge.backward.Length).Sum() + triangleJobs.Select(job => job.triangleJob.backward.Length).Sum(), Allocator.Temp);
                foreach (var (meshJudge, meshHandler) in meshJobs)
                {
                    _forwardIndexes.AddRange(meshJudge.forward.AsArray());
                    _backwardIndexes.AddRange(meshJudge.backward.AsArray());
                }
                foreach (var (triangle, triangleHandler) in triangleJobs)
                {
                    _forwardIndexes.AddRange(triangle.existForward.AsArray());
                    _backwardIndexes.AddRange(triangle.existBackward.AsArray());
                }

                var forwardIndexes = _forwardIndexes.ToArray(Allocator.TempJob);
                var backwardIndexes = _backwardIndexes.ToArray(Allocator.TempJob);

                disposable.Add(forwardIndexes);
                disposable.Add(backwardIndexes);

                var _orderingHandle = new NativeList<JobHandle>(4, Allocator.TempJob);
                disposable.Add(_orderingHandle);

                var fOrderingJob = new OrderingJob();
                fOrderingJob.indexes = forwardIndexes;
                fOrderingJob.useIndexes = new NativeList<int>(Allocator.TempJob);
                fOrderingJob.mapping = new NativeParallelHashMap<int, int>(forwardIndexes.Length, Allocator.TempJob);
                var fOrderingHandle = fOrderingJob.Schedule(sAllHandler);
                _orderingHandle.Add(fOrderingHandle);
                disposable.Add(fOrderingJob);

                var bOrderingJob = new OrderingJob();
                bOrderingJob.indexes = backwardIndexes;
                bOrderingJob.useIndexes = new NativeList<int>(Allocator.TempJob);
                bOrderingJob.mapping = new NativeParallelHashMap<int, int>(backwardIndexes.Length, Allocator.TempJob);
                var bOrderingHandle = bOrderingJob.Schedule(sAllHandler);
                _orderingHandle.Add(bOrderingHandle);
                disposable.Add(bOrderingJob);

                var _IdentifyDIndexHandler = new NativeList<JobHandle>(triangleJobs.Length * 2, Allocator.TempJob);
                var fIdentifyDIndexJob = new List<IdentifyDIndexJob>();
                var bIdentifyDIndexJob = new List<IdentifyDIndexJob>();
                var _IdentifyIndexHandler = new NativeList<JobHandle>(triangleJobs.Length * 2, Allocator.TempJob);
                var fIdentifyIndexJob = new List<IdentifyIndexJob>();
                var bIdentifyIndexJob = new List<IdentifyIndexJob>();
                disposable.Add(_IdentifyDIndexHandler);
                disposable.Add(_IdentifyIndexHandler);

                using var _dfaic = new NativeList<int>(triangleJobs.Length, Allocator.Temp);
                using var _dbaic = new NativeList<int>(triangleJobs.Length, Allocator.Temp);
                using var _dadc = new NativeList<int>(triangleJobs.Length + 1, Allocator.Temp);
                _dadc.Add(0);

                for (var i = 0; i < triangleJobs.Length; i++)
                {
                    var triangle = triangleJobs[i].triangleJob;
                    var _fl = _dfaic.Length == 0 ? 0 : _dfaic[^1];
                    var _bl = _dbaic.Length == 0 ? 0 : _dbaic[^1];
                    _dfaic.Add(_fl + triangle.forwardDivideIndex.Count());
                    _dbaic.Add(_bl + triangle.backwardDivideIndex.Count());


                    var _fIdentifyDIndexJob = new IdentifyDIndexJob();
                    _fIdentifyDIndexJob.indexes = triangle.forwardDivideIndex.ToNativeArray(Allocator.TempJob);
                    _fIdentifyDIndexJob.aggDCount = _dadc[^1];
                    var _bIdentifyDIndexJob = new IdentifyDIndexJob();
                    _bIdentifyDIndexJob.indexes = triangle.backwardDivideIndex.ToNativeArray(Allocator.TempJob);
                    _bIdentifyDIndexJob.aggDCount = _dadc[^1];
                    var _fIdentifyIndexJob = new IdentifyIndexJob();
                    _fIdentifyIndexJob.indexes = triangle.forward;
                    _fIdentifyIndexJob.outIndexes = new NativeList<int>(Allocator.TempJob);
                    _fIdentifyIndexJob.aggDCount = _dadc[^1];
                    var _bIdentifyIndexJob = new IdentifyIndexJob();
                    _bIdentifyIndexJob.indexes = triangle.backward;
                    _bIdentifyIndexJob.outIndexes = new NativeList<int>(Allocator.TempJob);
                    _bIdentifyIndexJob.aggDCount = _dadc[^1];

                    _IdentifyDIndexHandler.Add(_fIdentifyDIndexJob.Schedule(_fIdentifyDIndexJob.indexes.Length, 0, dAllHandler));
                    fIdentifyDIndexJob.Add(_fIdentifyDIndexJob);
                    _IdentifyDIndexHandler.Add(_bIdentifyDIndexJob.Schedule(_bIdentifyDIndexJob.indexes.Length, 0, dAllHandler));
                    bIdentifyDIndexJob.Add(_bIdentifyDIndexJob);
                    _IdentifyIndexHandler.Add(_fIdentifyIndexJob.Schedule(dAllHandler));
                    fIdentifyIndexJob.Add(_fIdentifyIndexJob);
                    _IdentifyIndexHandler.Add(_bIdentifyIndexJob.Schedule(dAllHandler));
                    bIdentifyIndexJob.Add(_bIdentifyIndexJob);

                    disposable.Add(_fIdentifyDIndexJob);
                    disposable.Add(_bIdentifyDIndexJob);
                    disposable.Add(_fIdentifyIndexJob);
                    disposable.Add(_bIdentifyIndexJob);

                    _dadc.Add(_dadc[^1] + divideJobs[i].divideJob.divideP.Length);
                }
                var dCount = _dadc[^1];
                _dadc.RemoveAt(_dadc.Length - 1);

                var dfaic = _dfaic.ToArray(Allocator.TempJob);
                var dbaic = _dbaic.ToArray(Allocator.TempJob);
                var dadc = _dadc.ToArray(Allocator.TempJob);
                var __IdentifyDIndexHandler = _IdentifyDIndexHandler.ToArray(Allocator.TempJob);
                var __IdentifyIndexHandler = _IdentifyIndexHandler.ToArray(Allocator.TempJob);
                var identifyDIndexHandler = JobHandle.CombineDependencies(__IdentifyDIndexHandler);
                var identifyIndexHandler = JobHandle.CombineDependencies(__IdentifyIndexHandler);
                disposable.Add(dfaic);
                disposable.Add(dbaic);
                disposable.Add(dadc);
                disposable.Add(__IdentifyDIndexHandler);
                disposable.Add(__IdentifyIndexHandler);

                identifyDIndexHandler.Complete();

                using var _dfIndexes = new NativeList<int>(dCount, Allocator.Temp);
                using var _dbIndexes = new NativeList<int>(dCount, Allocator.Temp);

                foreach (var dfidxJob in fIdentifyDIndexJob)
                {
                    _dfIndexes.AddRange(dfidxJob.indexes);
                }

                foreach (var dbidxJob in bIdentifyDIndexJob)
                {
                    _dbIndexes.AddRange(dbidxJob.indexes);
                }

                using var _divideP = new NativeList<DividePoint>(dCount, Allocator.Temp);

                foreach (var (divideJob, handler) in divideJobs)
                {
                    _divideP.AddRange(divideJob.divideP.AsArray());
                }

                var divideP = _divideP.ToArray(Allocator.TempJob);
                disposable.Add(divideP);

                var dfIndexes = _dfIndexes.ToArray(Allocator.TempJob);
                var dbIndexes = _dbIndexes.ToArray(Allocator.TempJob);
                disposable.Add(dfIndexes);
                disposable.Add(dbIndexes);

                var funiqueOrderingJob = new UniqueOrderingJob();
                funiqueOrderingJob.indexes = dfIndexes;
                funiqueOrderingJob.aggICounts = dfaic;
                funiqueOrderingJob.divideP = divideP;
                funiqueOrderingJob.mapping = new NativeParallelHashMap<int, int>(dCount, Allocator.TempJob);
                funiqueOrderingJob.uniqueDivideP = new NativeList<DividePoint>(Allocator.TempJob);
                var buniqueOrderingJob = new UniqueOrderingJob();
                buniqueOrderingJob.indexes = dbIndexes;
                buniqueOrderingJob.aggICounts = dbaic;
                buniqueOrderingJob.divideP = divideP;
                buniqueOrderingJob.mapping = new NativeParallelHashMap<int, int>(dCount, Allocator.TempJob);
                buniqueOrderingJob.uniqueDivideP = new NativeList<DividePoint>(Allocator.TempJob);

                _orderingHandle.Add(funiqueOrderingJob.Schedule(identifyDIndexHandler));
                disposable.Add(funiqueOrderingJob);
                _orderingHandle.Add(buniqueOrderingJob.Schedule(identifyDIndexHandler));
                disposable.Add(buniqueOrderingJob);

                using var __orderingHandle = _orderingHandle.ToArray(Allocator.Temp);
                var orderingJob = JobHandle.CombineDependencies(__orderingHandle);

                var fIndexCount = meshJobs.Select(job => job.meshJudge.forward.Length).Sum() + triangleJobs.Select(job => job.triangleJob.forward.Length).Sum();
                var bIndexCount = meshJobs.Select(job => job.meshJudge.backward.Length).Sum() + triangleJobs.Select(job => job.triangleJob.backward.Length).Sum();
                
                var _finalJobHandler = new NativeList<JobHandle>(triangleJobs.Length * 2 + 2, Allocator.TempJob);
                disposable.Add(_finalJobHandler);

                identifyIndexHandler.Complete();

                var faic = 0;
                var baic = 0;
                var fFinalIndexJobs = new List<FinalIndexing>();
                var bFinalIndexJobs = new List<FinalIndexing>();
                var fSubMeshDescriptor = new List<SubMeshDescriptor>();
                var bSubMeshDescriptor = new List<SubMeshDescriptor>();
                for (var i = 0; i < fIdentifyIndexJob.Count; i++)
                {
                    var fexistCount = meshJobs[i].meshJudge.forward.Length;
                    var ftotalCount = fexistCount + fIdentifyIndexJob[i].indexes.Length;
                    fSubMeshDescriptor.Add(new SubMeshDescriptor(faic, ftotalCount));
                    using var _fIndexes = new NativeList<int>(ftotalCount, Allocator.Temp);
                    _fIndexes.AddRange(meshJobs[i].meshJudge.forward.AsArray());
                    _fIndexes.AddRange(fIdentifyIndexJob[i].outIndexes.AsArray());
                    var fIndexes = _fIndexes.ToArray(Allocator.TempJob);
                    var fFinalIndexingJob = new FinalIndexing();
                    fFinalIndexingJob.indexes = fIndexes;
                    fFinalIndexingJob.existMapping = fOrderingJob.mapping;
                    fFinalIndexingJob.useIndexes = fOrderingJob.useIndexes;
                    fFinalIndexingJob.newMapping = funiqueOrderingJob.mapping;
                    fFinalIndexingJob.indicates = new NativeArray<int>(ftotalCount, Allocator.TempJob);
                    faic += ftotalCount;
                    disposable.Add(fIndexes);
                    disposable.Add(fFinalIndexingJob);
                    fFinalIndexJobs.Add(fFinalIndexingJob);

                    var bexistCount = meshJobs[i].meshJudge.backward.Length;
                    var btotalCount = bexistCount + bIdentifyIndexJob[i].indexes.Length;
                    bSubMeshDescriptor.Add(new SubMeshDescriptor(baic, btotalCount));
                    using var _bIndexes = new NativeList<int>(btotalCount, Allocator.Temp);
                    _bIndexes.AddRange(meshJobs[i].meshJudge.backward.AsArray());
                    _bIndexes.AddRange(bIdentifyIndexJob[i].outIndexes.AsArray());
                    var bIndexes = _bIndexes.ToArray(Allocator.TempJob);
                    var bFinalIndexingJob = new FinalIndexing();
                    bFinalIndexingJob.indexes = bIndexes;
                    bFinalIndexingJob.existMapping = bOrderingJob.mapping;
                    bFinalIndexingJob.useIndexes = bOrderingJob.useIndexes;
                    bFinalIndexingJob.newMapping = buniqueOrderingJob.mapping;
                    bFinalIndexingJob.indicates = new NativeArray<int>(btotalCount, Allocator.TempJob);
                    baic += btotalCount;
                    disposable.Add(bIndexes);
                    disposable.Add(bFinalIndexingJob);
                    bFinalIndexJobs.Add(bFinalIndexingJob);

                    _finalJobHandler.Add(fFinalIndexingJob.Schedule(ftotalCount, 0, orderingJob));
                    _finalJobHandler.Add(bFinalIndexingJob.Schedule(btotalCount, 0, orderingJob));
                }

                orderingJob.Complete();

                var fMeshData = newMeshArray[0];
                var bMeshData = newMeshArray[1];

                var ffVertexCount = fOrderingJob.useIndexes.Length + funiqueOrderingJob.uniqueDivideP.Length;
                var bfVertexCount = bOrderingJob.useIndexes.Length + buniqueOrderingJob.uniqueDivideP.Length;

                var fFormat = ffVertexCount > ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
                var bFormat = bfVertexCount > ushort.MaxValue ? IndexFormat.UInt16 : IndexFormat.UInt32;
                fMeshData.SetIndexBufferParams(fIndexCount, fFormat);
                bMeshData.SetIndexBufferParams(bIndexCount, bFormat);

                fMeshData.subMeshCount = fIdentifyIndexJob.Count;
                bMeshData.subMeshCount = bIdentifyIndexJob.Count;

                fMeshData.SetVertexBufferParams(ffVertexCount,
                    new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                    new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
                    new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2));
                bMeshData.SetVertexBufferParams(bfVertexCount,
                    new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                    new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
                    new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2));

                var normal = new NativeArray<Vector3>(data.vertexCount, Allocator.TempJob);
                data.GetNormals(normal);
                var uv = new NativeArray<Vector2>(data.vertexCount, Allocator.TempJob);
                data.GetUVs(0, uv);
                disposable.Add(normal);
                disposable.Add(uv);

                var fFinalVertexJob = new FinalVertexJob();
                fFinalVertexJob.vertexs = vertex;
                fFinalVertexJob.normals = normal;
                fFinalVertexJob.uvs = uv;
                fFinalVertexJob.useIndexes = fOrderingJob.useIndexes.AsArray();
                fFinalVertexJob.useDivideP = funiqueOrderingJob.uniqueDivideP;
                fFinalVertexJob.vertexData = new NativeArray<VertexData>(ffVertexCount, Allocator.TempJob);

                var bFinalVertexJob = new FinalVertexJob();
                bFinalVertexJob.vertexs = vertex;
                bFinalVertexJob.normals = normal;
                bFinalVertexJob.uvs = uv;
                bFinalVertexJob.useIndexes = bOrderingJob.useIndexes.AsArray();
                bFinalVertexJob.useDivideP = buniqueOrderingJob.uniqueDivideP;
                bFinalVertexJob.vertexData = new NativeArray<VertexData>(bfVertexCount, Allocator.TempJob);

                _finalJobHandler.Add(fFinalVertexJob.Schedule(orderingJob));
                disposable.Add(fFinalVertexJob);
                _finalJobHandler.Add(bFinalVertexJob.Schedule(orderingJob));
                disposable.Add(bFinalVertexJob);

                using var __finalJobHandler = _finalJobHandler.ToArray(Allocator.Temp);
                var finalJobHandler = JobHandle.CombineDependencies(__finalJobHandler);

                finalJobHandler.Complete();

                var fVertexData = fMeshData.GetVertexData<VertexData>();
                var bVertexData = bMeshData.GetVertexData<VertexData>();
                fVertexData.CopyFrom(fFinalVertexJob.vertexData);
                bVertexData.CopyFrom(bFinalVertexJob.vertexData);

                if (fFormat == IndexFormat.UInt16)
                {
                    var fMeshIndexData = fMeshData.GetIndexData<ushort>();
                    using var _fMeshIndexData = new NativeList<int>(fIndexCount, Allocator.Temp);
                    foreach (var fij in fFinalIndexJobs)
                    {
                        _fMeshIndexData.AddRange(fij.indicates);
                    }
                    var __fMeshIndexData = new NativeArray<ushort>(_fMeshIndexData.Length, Allocator.Temp);
                    for (var i = 0; i < _fMeshIndexData.Length; i++)
                    {
                        __fMeshIndexData[i] = (ushort)_fMeshIndexData[i];
                    }
                    fMeshIndexData.CopyFrom(__fMeshIndexData);
                    __fMeshIndexData.Dispose();
                } else
                {
                    var fMeshIndexData = fMeshData.GetIndexData<int>();
                    var _fMeshIndexData = new NativeList<int>(fIndexCount, Allocator.Temp);
                    foreach (var fij in fFinalIndexJobs)
                    {
                        _fMeshIndexData.AddRange(fij.indicates);
                    }
                    using var __fMeshIndexData = _fMeshIndexData.ToArray(Allocator.Temp);
                    fMeshIndexData.CopyFrom(__fMeshIndexData);
                }

                if (bFormat == IndexFormat.UInt16)
                {
                    var bMeshIndexData = bMeshData.GetIndexData<ushort>();
                    using var _bMeshIndexData = new NativeList<int>(bIndexCount, Allocator.Temp);
                    foreach (var bij in bFinalIndexJobs)
                    {
                        _bMeshIndexData.AddRange(bij.indicates);
                    }
                    var __bMeshIndexData = new NativeArray<ushort>(_bMeshIndexData.Length, Allocator.Temp);
                    for (var i = 0; i < _bMeshIndexData.Length; i++)
                    {
                        __bMeshIndexData[i] = (ushort)_bMeshIndexData[i];
                    }
                    bMeshIndexData.CopyFrom(__bMeshIndexData);
                    __bMeshIndexData.Dispose();
                }
                else
                {
                    var bMeshIndexData = bMeshData.GetIndexData<int>();
                    var _bMeshIndexData = new NativeList<int>(bIndexCount, Allocator.Temp);
                    foreach (var bij in bFinalIndexJobs)
                    {
                        _bMeshIndexData.AddRange(bij.indicates);
                    }
                    using var __bMeshIndexData = _bMeshIndexData.ToArray(Allocator.Temp);
                    bMeshIndexData.CopyFrom(__bMeshIndexData);
                }

                for (var i = 0; i < fIdentifyIndexJob.Count; i++)
                {
                    fMeshData.SetSubMesh(i, fSubMeshDescriptor[i]);
                    bMeshData.SetSubMesh(i, bSubMeshDescriptor[i]);
                }
            }
            finally
            {
                foreach (var d in disposable)
                {
                    d.Dispose();
                }
            }

            return newMeshArray;
        }
    }
}
