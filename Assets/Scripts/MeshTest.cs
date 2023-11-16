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
    public class MeshTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var dataArray = Mesh.AllocateWritableMeshData(1);
            var data = dataArray[0];

            data.SetVertexBufferParams(12,
                new VertexAttributeDescriptor(VertexAttribute.Position),
                new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1));

            var sqrt075 = Mathf.Sqrt(0.75f);
            var p0 = new Vector3(0, 0, 0);
            var p1 = new Vector3(1, 0, 0);
            var p2 = new Vector3(0.5f, 0, sqrt075);
            var p3 = new Vector3(0.5f, sqrt075, sqrt075 / 3);

            NativeArray<Vector3> pos = data.GetVertexData<Vector3>();
            pos[0] = p0; pos[1] = p1; pos[2] = p2;
            pos[3] = p0; pos[4] = p2; pos[5] = p3;
            pos[6] = p2; pos[7] = p1; pos[8] = p3;
            pos[9] = p0; pos[10] = p3; pos[11] = p1;

            data.SetIndexBufferParams(12, IndexFormat.UInt16);
            NativeArray<ushort> indexBuffer = data.GetIndexData<ushort>();
            for (ushort i = 0; i < indexBuffer.Length; ++i)
                indexBuffer[i] = i;

            data.subMeshCount = 1;
            data.SetSubMesh(0, new SubMeshDescriptor(0, indexBuffer.Length));

            var mesh = new Mesh();
            mesh.name = "Tetrahedron";
            Mesh.ApplyAndDisposeWritableMeshData(dataArray, mesh);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        public static ICollection<Mesh> CutMesh(GameObject target, Vector3 planeAnchorPoint, Vector3 planeNormal)
        {
            var targetMesh = target.GetComponent<MeshFilter>().mesh;
            using var dataArray = Mesh.AcquireReadOnlyMeshData(targetMesh);
            var data = dataArray[0];
            var sideJudge = new VertexSideJudgeJob();
            using var vertex = new NativeArray<Vector3>(data.vertexCount, Allocator.TempJob);
            data.GetVertices(vertex);
            sideJudge.vertex = vertex;
            sideJudge.dir = target.transform.InverseTransformDirection(planeNormal).normalized;
            sideJudge.pos = target.transform.InverseTransformPoint(planeAnchorPoint);
            sideJudge.side = new NativeArray<byte>(data.vertexCount, Allocator.TempJob);
            sideJudge.normal = new NativeArray<float>(data.vertexCount, Allocator.TempJob);
            var handler = sideJudge.Schedule(vertex.Length, 0);

            var meshJobs = Enumerable.Range(0, targetMesh.subMeshCount).Select(sub =>
            {
                var meshJudge = new MeshSideJudgeJob();
                meshJudge.side = sideJudge.side;
                var meshHandler = meshJudge.Schedule(handler);
                return (meshJudge, meshHandler);
            }).ToArray();

            var indexJobs = meshJobs.Select(job =>
            {
                var indexJob = new IndexJob();
                indexJob.center = job.meshJudge.center;
                var indexHandler = indexJob.Schedule(job.meshHandler);
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
                var divideHandler = divideJob.Schedule(job.indexHandler);
                return (divideJob, divideHandler);
            }).ToArray();

            var triangleJobs = divideJobs.Zip(meshJobs, (d, m) => (d.divideJob, m.meshJudge, d.divideHandler)).Select(job =>
            {
                var triangleJob = new MakeTriangleJob();
                triangleJob.center = job.meshJudge.center;
                triangleJob.divideInfos = job.divideJob.divideInfo;
                var triangleHandler = triangleJob.Schedule(job.divideHandler);
                return (triangleJob, triangleHandler);
            }).ToArray();


            using var uv = new NativeArray<Vector2>(data.vertexCount, Allocator.TempJob);
            data.GetUVs(0, uv);
            using var normal = new NativeArray<Vector3>(data.vertexCount, Allocator.TempJob);
            data.GetNormals(normal);


            handler.Complete();
            throw new NotImplementedException();
        }
    }
}
