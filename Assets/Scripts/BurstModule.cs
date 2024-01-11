using System;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using System.Runtime.InteropServices;
using Unity.Burst.CompilerServices;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;

namespace SamuraiSoccer
{
    [BurstCompile]
    public struct VertexSideJudgeJob : IJobParallelFor, IDisposable
    {
        [ReadOnly]
        public NativeArray<Vector3> vertex;

        [ReadOnly]
        public float3 dir;

        [ReadOnly]
        public float offset;

        [WriteOnly]
        public NativeArray<byte> side;

        [WriteOnly]
        public NativeArray<float> normal;

        public void Dispose()
        {
            side.Dispose();
            normal.Dispose();
        }

        public void Execute(int index)
        {
            float3 vp = vertex[index];
            float n = math.dot(dir, vp) + offset;
            normal[index] = n;
            side[index] = (byte)math.step(0.0f, n);
        }
    }

    [BurstCompile]
    public struct MeshSideJudgeJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeArray<byte> side;

        [ReadOnly]
        public NativeArray<int> indexes;

        [WriteOnly]
        public NativeList<int> forward;

        [WriteOnly]
        public NativeList<int> backward;

        [WriteOnly]
        public NativeList<int> center;

        public void Dispose()
        {
            forward.Dispose();
            backward.Dispose();
            center.Dispose();
        }

        public void Execute()
        {
            var triCount = indexes.Length / 3;
            for (var i = 0; i < triCount; i++)
            {
                var i0 = indexes[3 * i];
                var i1 = indexes[3 * i + 1];
                var i2 = indexes[3 * i + 2];
                var sideScore = side[i0] + side[i1] + side[i2];
                switch (sideScore)
                {
                    case 0:
                        backward.Add(i0);
                        backward.Add(i1);
                        backward.Add(i2); break;
                    case 3:
                        forward.Add(i0);
                        forward.Add(i1);
                        forward.Add(i2); break;
                    default:
                        center.Add(i0);
                        center.Add(i1);
                        center.Add(i2); break;
                }
            }
        }
    }

    [BurstCompile]
    public struct IndexJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeList<int> center;

        [ReadOnly]
        public NativeArray<Vector3> pos;

        [WriteOnly]
        public NativeParallelMultiHashMap<Vector3, int> vertexIndex;

        public void Dispose()
        {
            vertexIndex.Dispose();
        }

        public void Execute()
        {
            var triCount = center.Length / 3;
            for (int i = 0; i < triCount; i++)
            {
                vertexIndex.Add(pos[center[3 * i]], i);
                vertexIndex.Add(pos[center[3 * i + 1]], i);
                vertexIndex.Add(pos[center[3 * i + 2]], i);
            }
        }
    }

    [BurstCompile]
    public struct DivideJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeArray<float> normal;

        [ReadOnly]
        public NativeArray<byte> side;

        [ReadOnly]
        public NativeArray<Vector3> vertexPos;

        [ReadOnly]
        public NativeList<int> center;

        [ReadOnly]
        public NativeParallelMultiHashMap<Vector3, int> vertexIndex;

        public NativeList<DividePoint> divideP;

        public NativeList<DivideInfo> divideInfo;

        public void Dispose()
        {
            divideP.Dispose();
            divideInfo.Dispose();
        }

        public void Execute()
        {
            var triCount = center.Length / 3;
            for (int i = 0; i < triCount; i++)
            {
                var i1 = 3 * i;
                var i2 = 3 * i + 1;
                var i3 = 3 * i + 2;
                var s1 = side[center[i1]];
                var s2 = side[center[i2]];
                var s3 = side[center[i3]];
                var singleIndex = (int)(3.0f - math.ceil(math.abs(s1 + s2 * 2 + s3 * 4 - 3.5f)));
                var singleIdx = center[i1 + singleIndex];
                var singleSide = side[singleIdx] == 0;
                int firstIdx, secondIdx;
                firstIdx = secondIdx = 0;
                switch (singleIndex)
                {
                    case 0: firstIdx = center[i2]; secondIdx = center[i3]; break;
                    case 1: firstIdx = center[i1]; secondIdx = center[i3]; break;
                    case 2: firstIdx = center[i1]; secondIdx = center[i2]; break;
                }
                var singleCount = vertexIndex.CountValuesForKey(vertexPos[singleIdx]) - 1;
                Span<int> singleArray = stackalloc int[singleCount];
                var singlev = vertexIndex.GetValuesForKey(vertexPos[singleIdx]);
                singlev.MoveNext();
                for (int j = 0; j < singleCount; j++)
                {
                    if (singlev.Current == i)
                    {
                        singlev.MoveNext();
                    }
                    singleArray[j] = singlev.Current;
                    singlev.MoveNext();
                }

                var firstv = vertexIndex.GetValuesForKey(vertexPos[firstIdx]);
                var sideMeshIndex0 = FindMatchData(singleArray, firstv);

                var secondv = vertexIndex.GetValuesForKey(vertexPos[secondIdx]);
                var sideMeshIndex1 = FindMatchData(singleArray, secondv);

                int divideIndex0, divideIndex1;

                if (Hint.Unlikely(sideMeshIndex0 < i))
                {
                    var sideInfo = divideInfo[sideMeshIndex0];
                    divideIndex0 = sideInfo.sideMeshIndex0 == i ? sideInfo.divideIndex0 : sideInfo.divideIndex1;
                }
                else
                {
                    var p2abs = math.abs(normal[firstIdx]);
                    var p1abs = math.abs(normal[singleIdx]);
                    var t = p2abs / (p1abs + p2abs); // 0:close to firstIdx, 1:close to singleIndex
                    var _pos = math.lerp(vertexPos[firstIdx], vertexPos[singleIdx], t);
                    var p = new DividePoint(firstIdx, singleIdx, t, _pos);
                    divideIndex0 = divideP.Length;
                    divideP.Add(p);
                }

                if (Hint.Unlikely(sideMeshIndex1 < i))
                {
                    var sideInfo = divideInfo[sideMeshIndex1];
                    divideIndex1 = sideInfo.sideMeshIndex0 == i ? sideInfo.divideIndex0 : sideInfo.divideIndex1;
                }
                else
                {
                    var p2abs = math.abs(normal[secondIdx]);
                    var p1abs = math.abs(normal[singleIdx]);
                    var t = p2abs / (p1abs + p2abs); // 0:close to secondIdx, 1:close to singleIndex
                    var _pos = math.lerp(vertexPos[secondIdx], vertexPos[singleIdx], t);
                    var p = new DividePoint(secondIdx, singleIdx, t, _pos);
                    divideIndex1 = divideP.Length;
                    divideP.Add(p);
                }

                var dir = (float3)divideP[divideIndex1].pos - (float3)divideP[divideIndex0].pos;
                dir = math.normalize(dir);

                var info = new DivideInfo(singleIndex, singleSide, sideMeshIndex0, sideMeshIndex1, divideIndex0, divideIndex1, dir);
                divideInfo.Add(info);
            }

            for (int i = 1; i < triCount; i++)
            {
                var selfInfo = divideInfo[i];
                var selfDir = selfInfo.dir;
                var sideIndex0 = selfInfo.sideMeshIndex0;
                var sideIndex1 = selfInfo.sideMeshIndex1;

                if (Hint.Unlikely(sideIndex0 < i))
                {
                    var sideDir = divideInfo[sideIndex0].dir;
                    var isStraight = 0.99999 < math.abs(math.dot(sideDir, selfDir));
                    divideInfo[i] = divideInfo[i].SetStraight0(isStraight);
                    var isSide0 = divideInfo[sideIndex0].sideMeshIndex0 == i;
                    if (isSide0)
                    {
                        divideInfo[sideIndex0] = divideInfo[sideIndex0].SetStraight0(isStraight);
                    }
                    else
                    {
                        divideInfo[sideIndex0] = divideInfo[sideIndex0].SetStraight1(isStraight);
                    }
                }

                if (Hint.Unlikely(sideIndex1 < i))
                {
                    var sideDir = divideInfo[sideIndex1].dir;
                    var isStraight = 0.99999 < math.abs(math.dot(sideDir, selfDir));
                    divideInfo[i] = divideInfo[i].SetStraight1(isStraight);
                    var isSide0 = divideInfo[sideIndex1].sideMeshIndex0 == i;
                    if (isSide0)
                    {
                        divideInfo[sideIndex1] = divideInfo[sideIndex1].SetStraight0(isStraight);
                    }
                    else
                    {
                        divideInfo[sideIndex1] = divideInfo[sideIndex1].SetStraight1(isStraight);
                    }
                }
            }
        }

        private int FindMatchData(in Span<int> src, in NativeParallelMultiHashMap<Vector3, int>.Enumerator dst)
        {
            foreach (var s in src)
            {
                foreach (var d in dst)
                {
                    if (s == d)
                    {
                        return d;
                    }
                }
            }
            return int.MaxValue;
        }
    }

    [BurstCompile]
    public struct DividePoint : IEquatable<DividePoint>
    {
        public readonly int idx0;
        public readonly int idx1;
        public float t;
        public Vector3 pos;

        public DividePoint(int idx0, int idx1, float t, float3 pos)
        {
            this.idx0 = idx0;
            this.idx1 = idx1;
            this.t = t;
            this.pos = pos;
        }

        public override readonly int GetHashCode()
        {
            return idx0.GetHashCode() ^ idx1.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is DividePoint point)
            {
                return Equals(point);
            }
            return false;
        }

        public readonly bool Equals(DividePoint other)
        {
            return (idx0 == other.idx0 && idx1 == other.idx1) || (idx0 == other.idx1 && idx1 == other.idx0);
        }

        public static bool operator ==(DividePoint lhs, DividePoint rhs) { return lhs.Equals(rhs); }
        public static bool operator !=(DividePoint lhs, DividePoint rhs) { return !(lhs == rhs); }
    }

    [BurstCompile]
    public struct DivideInfo
    {
        public int singleIndex;
        // is backward
        public bool singleSide;
        public int sideMeshIndex0;
        public int sideMeshIndex1;
        public int divideIndex0;
        public int divideIndex1;
        public float3 dir;
        public bool isStraight0;
        public bool isStraight1;

        public DivideInfo(int singleIndex, bool singleSide, int sideMeshIndex0, int sideMeshIndex1, int divideIndex0, int divideIndex1, float3 dir)
        {
            this.singleIndex = singleIndex;
            this.singleSide = singleSide;
            this.sideMeshIndex0 = sideMeshIndex0;
            this.sideMeshIndex1 = sideMeshIndex1;
            this.divideIndex0 = divideIndex0;
            this.divideIndex1 = divideIndex1;
            this.dir = dir;
            this.isStraight0 = false;
            this.isStraight1 = false;
        }

        public DivideInfo SetStraight0(bool isStraight0)
        {
            this.isStraight0 = isStraight0;
            return this;
        }

        public DivideInfo SetStraight1(bool isStraight1)
        {
            this.isStraight1 = isStraight1;
            return this;
        }
    }

    [BurstCompile]
    public struct MakeTriangleJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeList<int> center;

        [ReadOnly]
        public NativeList<DivideInfo> divideInfos;

        [WriteOnly]
        public NativeList<int> forward;

        [WriteOnly]
        public NativeList<int> existForward;

        [WriteOnly]
        public NativeList<int> backward;

        [WriteOnly]
        public NativeList<int> existBackward;

        public NativeParallelHashSet<int> forwardDivideIndex;

        public NativeParallelHashSet<int> backwardDivideIndex;


        public void Dispose()
        {
            forward.Dispose();
            existForward.Dispose();
            backward.Dispose();
            existBackward.Dispose();
            forwardDivideIndex.Dispose();
            backwardDivideIndex.Dispose();
        }

        public void Execute()
        {
            var forwardAdded = new NativeParallelHashSet<int>(divideInfos.Length, Allocator.Temp);
            var backwardAdded = new NativeParallelHashSet<int>(divideInfos.Length, Allocator.Temp);

            for (var i = 0; i < divideInfos.Length; i++)
            {
                var idx0 = i * 3;
                var idx1 = i * 3 + 1;
                var idx2 = i * 3 + 2;
                var info = divideInfos[i];

                if (info.singleSide)
                {
                    // two point is forward
                    if (!forwardAdded.Contains(i))
                    {
                        // forward
                        var divideIndex0 = info.divideIndex0;
                        var divideIndex1 = info.divideIndex1;
                        if (Hint.Unlikely(Hint.Unlikely(info.isStraight0) && Hint.Likely(!forwardAdded.Contains(info.sideMeshIndex0))))
                        {
                            RectExtendForward(ref divideIndex0, ref forwardAdded, i, info.sideMeshIndex0);
                        }

                        if (Hint.Unlikely(Hint.Unlikely(info.isStraight1) && Hint.Likely(!forwardAdded.Contains(info.sideMeshIndex1))))
                        {
                            RectExtendForward(ref divideIndex1, ref forwardAdded, i, info.sideMeshIndex1);
                        }

                        var divideIndexM0 = -divideIndex0 - 1;
                        var divideIndexM1 = -divideIndex1 - 1;

                        switch (info.singleIndex)
                        {
                            case 0:
                                existForward.Add(center[idx1]); existForward.Add(center[idx2]);
                                forward.Add(divideIndexM0); forward.Add(center[idx1]); forward.Add(divideIndexM1);
                                forward.Add(center[idx1]); forward.Add(center[idx2]); forward.Add(divideIndexM1); break;
                            case 1:
                                existForward.Add(center[idx0]); existForward.Add(center[idx2]);
                                forward.Add(center[idx0]); forward.Add(divideIndexM0); forward.Add(divideIndexM1);
                                forward.Add(divideIndexM1); forward.Add(center[idx2]); forward.Add(center[idx0]); break;
                            case 2:
                                existForward.Add(center[idx0]); existForward.Add(center[idx1]);
                                forward.Add(center[idx0]); forward.Add(center[idx1]); forward.Add(divideIndexM0);
                                forward.Add(divideIndexM0); forward.Add(center[idx1]); forward.Add(divideIndexM1); break;
                        }

                        forwardDivideIndex.Add(divideIndexM0);
                        forwardDivideIndex.Add(divideIndexM1);
                        forwardAdded.Add(i);
                    }

                    #region backward one
                    if (!backwardAdded.Contains(i))
                    {
                        // backward
                        var divideIndex0 = info.divideIndex0;
                        var divideIndex1 = info.divideIndex1;
                        var vindex = -1;
                        var is0Side = false;

                        if (Hint.Unlikely(Hint.Unlikely(info.isStraight0) && Hint.Likely(!backwardAdded.Contains(info.sideMeshIndex0))))
                        {
                            TriangleExtendBackward(ref divideIndex0, ref vindex, ref backwardAdded, i, info.sideMeshIndex0, center[idx0 + info.singleIndex]);
                            if (vindex >= 0)
                            {
                                is0Side = true;
                            }
                        }

                        if (Hint.Unlikely(Hint.Unlikely(info.isStraight1) && Hint.Likely(!backwardAdded.Contains(info.sideMeshIndex1))))
                        {
                            TriangleExtendBackward(ref divideIndex1, ref vindex, ref backwardAdded, i, info.sideMeshIndex1, center[idx0 + info.singleIndex]);
                        }

                        var divideIndexM0 = -divideIndex0 - 1;
                        var divideIndexM1 = -divideIndex1 - 1;

                        if (Hint.Unlikely(vindex >= 0))
                        {
                            if (is0Side)
                            {
                                switch (info.singleIndex)
                                {
                                    case 0:
                                        existBackward.Add(center[idx0]); existBackward.Add(center[vindex]);
                                        backward.Add(center[idx0]); backward.Add(center[vindex]); backward.Add(divideIndexM0);
                                        backward.Add(divideIndexM0); backward.Add(divideIndexM1); backward.Add(center[idx0]); break;
                                    case 1:
                                        existBackward.Add(center[idx1]); existBackward.Add(center[vindex]);
                                        backward.Add(divideIndexM1); backward.Add(center[vindex]); backward.Add(center[idx1]);
                                        backward.Add(center[idx1]); backward.Add(divideIndexM1); backward.Add(divideIndexM0); break;
                                    case 2:
                                        existBackward.Add(center[idx2]); existBackward.Add(center[vindex]);
                                        backward.Add(center[idx2]); backward.Add(center[vindex]); backward.Add(divideIndexM0);
                                        backward.Add(divideIndexM0); backward.Add(divideIndexM1); backward.Add(center[idx2]); break;
                                }
                            }
                            else
                            {
                                switch (info.singleIndex)
                                {
                                    case 0:
                                        existBackward.Add(center[idx0]); existBackward.Add(center[vindex]);
                                        backward.Add(center[idx0]); backward.Add(divideIndexM0); backward.Add(divideIndexM1);
                                        backward.Add(divideIndexM1); backward.Add(center[vindex]); backward.Add(center[idx0]); break;
                                    case 1:
                                        existBackward.Add(center[idx1]); existBackward.Add(center[vindex]);
                                        backward.Add(center[idx1]); backward.Add(center[vindex]); backward.Add(divideIndexM1);
                                        backward.Add(divideIndexM1); backward.Add(divideIndexM0); backward.Add(center[idx1]); break;
                                    case 2:
                                        existBackward.Add(center[idx2]); existBackward.Add(center[vindex]);
                                        backward.Add(center[idx2]); backward.Add(divideIndexM0); backward.Add(divideIndexM1);
                                        backward.Add(divideIndexM1); backward.Add(center[vindex]); backward.Add(center[idx2]); break;
                                }
                            }

                        }
                        else
                        {
                            switch (info.singleIndex)
                            {
                                case 0:
                                    existBackward.Add(center[idx0]);
                                    backward.Add(center[idx0]); backward.Add(divideIndexM0); backward.Add(divideIndexM1); break;
                                case 1:
                                    existBackward.Add(center[idx1]);
                                    backward.Add(center[idx1]); backward.Add(divideIndexM1); backward.Add(divideIndexM0); break;
                                case 2:
                                    existBackward.Add(center[idx2]);
                                    backward.Add(divideIndexM0); backward.Add(divideIndexM1); backward.Add(center[idx2]); break;
                            }
                        }

                        backwardDivideIndex.Add(divideIndexM0);
                        backwardDivideIndex.Add(divideIndexM1);
                        backwardAdded.Add(i);
                    }
                    #endregion backward one
                }
                else
                {
                    // two point is backward
                    if (!backwardAdded.Contains(i))
                    {
                        // backward
                        var divideIndex0 = info.divideIndex0;
                        var divideIndex1 = info.divideIndex1;
                        if (Hint.Unlikely(Hint.Unlikely(info.isStraight0) && Hint.Likely(!backwardAdded.Contains(info.sideMeshIndex0))))
                        {
                            RectExtendBackward(ref divideIndex0, ref backwardAdded, i, info.sideMeshIndex0);
                        }

                        if (Hint.Unlikely(Hint.Unlikely(info.isStraight1) && Hint.Likely(!backwardAdded.Contains(info.sideMeshIndex1))))
                        {
                            RectExtendBackward(ref divideIndex1, ref backwardAdded, i, info.sideMeshIndex1);
                        }

                        var divideIndexM0 = -divideIndex0 - 1;
                        var divideIndexM1 = -divideIndex1 - 1;

                        switch (info.singleIndex)
                        {
                            case 0:
                                existBackward.Add(center[idx1]); existBackward.Add(center[idx2]);
                                backward.Add(divideIndexM0); backward.Add(center[idx1]); backward.Add(divideIndexM1);
                                backward.Add(center[idx1]); backward.Add(center[idx2]); backward.Add(divideIndexM1); break;
                            case 1:
                                existBackward.Add(center[idx0]); existBackward.Add(center[idx2]);
                                backward.Add(center[idx0]); backward.Add(divideIndexM0); backward.Add(divideIndexM1);
                                backward.Add(divideIndexM1); backward.Add(center[idx2]); backward.Add(center[idx0]); break;
                            case 2:
                                existBackward.Add(center[idx0]); existBackward.Add(center[idx1]);
                                backward.Add(center[idx0]); backward.Add(center[idx1]); backward.Add(divideIndexM0);
                                backward.Add(divideIndexM0); backward.Add(center[idx1]); backward.Add(divideIndexM1); break;
                        }

                        backwardDivideIndex.Add(divideIndexM0);
                        backwardDivideIndex.Add(divideIndexM1);
                        backwardAdded.Add(i);
                    }
                    #region forward one
                    if (!forwardAdded.Contains(i))
                    {
                        // forward
                        var divideIndex0 = info.divideIndex0;
                        var divideIndex1 = info.divideIndex1;
                        var vindex = -1;
                        var is0Side = false;

                        if (Hint.Unlikely(Hint.Unlikely(info.isStraight0) && Hint.Likely(!forwardAdded.Contains(info.sideMeshIndex0))))
                        {
                            TriangleExtendForward(ref divideIndex0, ref vindex, ref forwardAdded, i, info.sideMeshIndex0, center[idx0 + info.singleIndex]);
                            if (vindex >= 0)
                            {
                                is0Side = true;
                            }
                        }

                        if (Hint.Unlikely(Hint.Unlikely(info.isStraight1) && Hint.Likely(!forwardAdded.Contains(info.sideMeshIndex1))))
                        {
                            TriangleExtendForward(ref divideIndex1, ref vindex, ref forwardAdded, i, info.sideMeshIndex1, center[idx0 + info.singleIndex]);
                        }

                        var divideIndexM0 = -divideIndex0 - 1;
                        var divideIndexM1 = -divideIndex1 - 1;

                        if (Hint.Unlikely(vindex >= 0))
                        {
                            if (is0Side)
                            {
                                switch (info.singleIndex)
                                {
                                    case 0:
                                        existForward.Add(center[idx0]); existForward.Add(center[vindex]);
                                        forward.Add(center[idx0]); forward.Add(center[vindex]); forward.Add(divideIndexM0);
                                        forward.Add(divideIndexM0); forward.Add(divideIndexM1); forward.Add(center[idx0]); break;
                                    case 1:
                                        existForward.Add(center[idx1]); existForward.Add(center[vindex]);
                                        forward.Add(divideIndexM1); forward.Add(center[vindex]); forward.Add(center[idx1]);
                                        forward.Add(center[idx1]); forward.Add(divideIndexM1); forward.Add(divideIndexM0); break;
                                    case 2:
                                        existForward.Add(center[idx2]); existForward.Add(center[vindex]);
                                        forward.Add(center[idx2]); forward.Add(center[vindex]); forward.Add(divideIndexM0);
                                        forward.Add(divideIndexM0); forward.Add(divideIndexM1); forward.Add(center[idx2]); break;
                                }
                            }
                            else
                            {
                                switch (info.singleIndex)
                                {
                                    case 0:
                                        existForward.Add(center[idx0]); existForward.Add(center[vindex]);
                                        forward.Add(center[idx0]); forward.Add(divideIndexM0); forward.Add(divideIndexM1);
                                        forward.Add(divideIndexM1); forward.Add(center[vindex]); forward.Add(center[idx0]); break;
                                    case 1:
                                        existForward.Add(center[idx1]); existForward.Add(center[vindex]);
                                        forward.Add(center[idx1]); forward.Add(center[vindex]); forward.Add(divideIndexM1);
                                        forward.Add(divideIndexM1); forward.Add(divideIndexM0); forward.Add(center[idx1]); break;
                                    case 2:
                                        existForward.Add(center[idx2]); existForward.Add(center[vindex]);
                                        forward.Add(center[idx2]); forward.Add(divideIndexM0); forward.Add(divideIndexM1);
                                        forward.Add(divideIndexM1); forward.Add(center[vindex]); forward.Add(center[idx2]); break;
                                }
                            }

                        }
                        else
                        {
                            switch (info.singleIndex)
                            {
                                case 0:
                                    existForward.Add(center[idx0]);
                                    forward.Add(center[idx0]); forward.Add(divideIndexM0); forward.Add(divideIndexM1); break;
                                case 1:
                                    existForward.Add(center[idx1]);
                                    forward.Add(center[idx1]); forward.Add(divideIndexM1); forward.Add(divideIndexM0); break;
                                case 2:
                                    existForward.Add(center[idx2]);
                                    forward.Add(divideIndexM0); forward.Add(divideIndexM1); forward.Add(center[idx2]); break;
                            }
                        }

                        forwardDivideIndex.Add(divideIndexM0);
                        forwardDivideIndex.Add(divideIndexM1);
                        forwardAdded.Add(i);
                    }
                    #endregion forward one
                }
            }

            forwardAdded.Dispose();
            backwardAdded.Dispose();
        }

        private void RectExtendForward(ref int divideIndex, ref NativeParallelHashSet<int> forwardAdded, int before, int next)
        {
            while (true)
            {
                var nextInfo = divideInfos[next];
                if (nextInfo.singleSide)
                {
                    return;
                }

                forwardAdded.Add(next);
                if (nextInfo.sideMeshIndex0 == before)
                {
                    divideIndex = nextInfo.divideIndex1;
                    before = next;
                    next = nextInfo.sideMeshIndex1;
                    if (Hint.Likely(Hint.Likely(!nextInfo.isStraight1) || Hint.Unlikely(forwardAdded.Contains(next))))
                    {
                        return;
                    }
                }
                else
                {
                    divideIndex = nextInfo.divideIndex0;
                    before = next;
                    next = nextInfo.sideMeshIndex0;
                    if (Hint.Likely(Hint.Likely(!nextInfo.isStraight0) || Hint.Unlikely(forwardAdded.Contains(next))))
                    {
                        return;
                    }
                }
            }
        }

        private void RectExtendBackward(ref int divideIndex, ref NativeParallelHashSet<int> backwardAdded, int before, int next)
        {
            while (true)
            {
                var nextInfo = divideInfos[next];
                if (!nextInfo.singleSide)
                {
                    return;
                }

                backwardAdded.Add(next);
                if (nextInfo.sideMeshIndex0 == before)
                {
                    divideIndex = nextInfo.divideIndex1;
                    before = next;
                    next = nextInfo.sideMeshIndex1;
                    if (Hint.Likely(Hint.Likely(!nextInfo.isStraight1) || Hint.Unlikely(backwardAdded.Contains(next))))
                    {
                        return;
                    }
                }
                else
                {
                    divideIndex = nextInfo.divideIndex0;
                    before = next;
                    next = nextInfo.sideMeshIndex0;
                    if (Hint.Likely(Hint.Likely(!nextInfo.isStraight0) || Hint.Unlikely(backwardAdded.Contains(next))))
                    {
                        return;
                    }
                }
            }
        }

        private void TriangleExtendForward(ref int divideIndex, ref int vindex, ref NativeParallelHashSet<int> forwardAdded, int before, int next, int triangelIndex)
        {
            while (true)
            {
                var nextInfo = divideInfos[next];
                var isRect = vindex >= 0;
                if (nextInfo.singleSide && isRect)
                {
                    return;
                }

                forwardAdded.Add(next);
                if (nextInfo.sideMeshIndex0 == before)
                {
                    divideIndex = nextInfo.divideIndex1;
                    if (!isRect && nextInfo.singleSide)
                    {
                        var n0 = next * 3;
                        var n1 = next * 3 + 1;
                        var n2 = next * 3 + 2;
                        switch (nextInfo.singleIndex)
                        {
                            case 0: vindex = center[n1] == triangelIndex ? n2 : n1; break;
                            case 1: vindex = center[n0] == triangelIndex ? n2 : n0; break;
                            case 2: vindex = center[n0] == triangelIndex ? n1 : n0; break;
                        }
                    }
                    before = next;
                    next = nextInfo.sideMeshIndex1;
                    if (Hint.Likely(Hint.Likely(!nextInfo.isStraight1) || Hint.Unlikely(forwardAdded.Contains(next))))
                    {
                        return;
                    }
                }
                else
                {
                    divideIndex = nextInfo.divideIndex0;
                    if (!isRect && nextInfo.singleSide)
                    {
                        var n0 = next * 3;
                        var n1 = next * 3 + 1;
                        var n2 = next * 3 + 2;
                        switch (nextInfo.singleIndex)
                        {
                            case 0: vindex = center[n1] == triangelIndex ? n2 : n1; break;
                            case 1: vindex = center[n0] == triangelIndex ? n2 : n0; break;
                            case 2: vindex = center[n0] == triangelIndex ? n1 : n0; break;
                        }
                    }
                    before = next;
                    next = nextInfo.sideMeshIndex0;
                    if (Hint.Likely(Hint.Likely(!nextInfo.isStraight0) || Hint.Unlikely(forwardAdded.Contains(next))))
                    {
                        return;
                    }
                }
            }
        }

        private void TriangleExtendBackward(ref int divideIndex, ref int vindex, ref NativeParallelHashSet<int> backwardAdded, int before, int next, int triangelIndex)
        {
            while (true)
            {
                var nextInfo = divideInfos[next];
                var isRect = vindex >= 0;
                if (!nextInfo.singleSide && isRect)
                {
                    return;
                }

                backwardAdded.Add(next);
                if (nextInfo.sideMeshIndex0 == before)
                {
                    divideIndex = nextInfo.divideIndex1;
                    if (!isRect && !nextInfo.singleSide)
                    {
                        var n0 = next * 3;
                        var n1 = next * 3 + 1;
                        var n2 = next * 3 + 2;
                        switch (nextInfo.singleIndex)
                        {
                            case 0: vindex = center[n1] == triangelIndex ? n2 : n1; break;
                            case 1: vindex = center[n0] == triangelIndex ? n2 : n0; break;
                            case 2: vindex = center[n0] == triangelIndex ? n1 : n0; break;
                        }
                    }
                    before = next;
                    next = nextInfo.sideMeshIndex1;
                    if (Hint.Likely(Hint.Likely(!nextInfo.isStraight1) || Hint.Unlikely(backwardAdded.Contains(next))))
                    {
                        return;
                    }
                }
                else
                {
                    divideIndex = nextInfo.divideIndex0;
                    if (!isRect && !nextInfo.singleSide)
                    {
                        var n0 = next * 3;
                        var n1 = next * 3 + 1;
                        var n2 = next * 3 + 2;
                        switch (nextInfo.singleIndex)
                        {
                            case 0: vindex = center[n1] == triangelIndex ? n2 : n1; break;
                            case 1: vindex = center[n0] == triangelIndex ? n2 : n0; break;
                            case 2: vindex = center[n0] == triangelIndex ? n1 : n0; break;
                        }
                    }
                    before = next;
                    next = nextInfo.sideMeshIndex0;
                    if (Hint.Likely(Hint.Likely(!nextInfo.isStraight0) || Hint.Unlikely(backwardAdded.Contains(next))))
                    {
                        return;
                    }
                }
            }
        }
    }

    [BurstCompile]
    public struct MakeLoopJob : IJob, IDisposable
    {
        public int deleteAxis;

        [ReadOnly]
        public NativeList<DivideInfo> divideInfo;

        [ReadOnly]
        public NativeList<DividePoint> divideP;

        public NativeList<int> loopNo;

        [WriteOnly]
        public NativeList<LoopInfo> loopInfo;

        public void Dispose()
        {
            loopNo.Dispose();
            loopInfo.Dispose();
        }

        public void Execute()
        {
            var _loopNo = new NativeArray<int>(divideInfo.Length, Allocator.Temp);
            var loopMap = new NativeList<int>(Allocator.Temp);
            var no = 0;

            for (var i = 0; i < divideInfo.Length; i++)
            {
                var haveLoopNo0 = divideInfo[i].sideMeshIndex0 < i;
                var haveLoopNo1 = divideInfo[i].sideMeshIndex1 < i;

                if (haveLoopNo0 && haveLoopNo1)
                {
                    var info = divideInfo[i];
                    var no0 = _loopNo[info.sideMeshIndex0];
                    var no1 = _loopNo[info.sideMeshIndex1];
                    var mapped0 = loopMap[no0];
                    var mapped1 = loopMap[no1];
                    var mappedNo = math.min(mapped0, mapped1);
                    _loopNo[i] = no0;
                    loopMap[no0] = mappedNo;
                    loopMap[no1] = mappedNo;
                }
                else if (!haveLoopNo0 && !haveLoopNo1)
                {
                    _loopNo[i] = no;
                    loopMap.Add(no);
                    no++;
                }
                else if (haveLoopNo0)
                {
                    _loopNo[i] = _loopNo[divideInfo[i].sideMeshIndex0];
                }
                else
                {
                    _loopNo[i] = _loopNo[divideInfo[i].sideMeshIndex1];
                }
            }

            var _mappedSet = new NativeParallelHashSet<int>(loopMap.Length, Allocator.Temp);
            for (var i = 0; i < loopMap.Length; i++)
            {
                _mappedSet.Add(loopMap[i]);
                loopMap[i] = _mappedSet.Count() - 1;
            }
            var loopCount = _mappedSet.Count();

            loopNo.SetCapacity(divideInfo.Length);
            for (var i = 0; i < divideInfo.Length; i++)
            {
                loopNo.Add(loopMap[_loopNo[i]]);
            }

            for (var i = 0; i < loopCount; i++)
            {
                var firstIndex = 0;
                while (loopNo[firstIndex] != i)
                {
                    firstIndex++;
                }

                var isLoop = false;
                var beforeIndex = firstIndex;
                var nextIndex = firstIndex;
                var xMin = float.MaxValue;
                var xMax = float.MinValue;
                var yMin = float.MaxValue;
                var yMax = float.MinValue;

                while (true)
                {
                    var dpIndex = -1;
                    var thisIndex = nextIndex;
                    var info = divideInfo[nextIndex];
                    if (Hint.Unlikely(info.sideMeshIndex0 != int.MaxValue) && info.sideMeshIndex0 != beforeIndex)
                    {
                        nextIndex = info.sideMeshIndex0;
                        dpIndex = info.divideIndex0;
                    }

                    if (Hint.Unlikely(info.sideMeshIndex1 != int.MaxValue) && info.sideMeshIndex1 != beforeIndex)
                    {
                        nextIndex = info.sideMeshIndex1;
                        dpIndex = info.divideIndex1;
                    }

                    if (dpIndex < 0)
                    {
                        break;
                    }

                    var x = 0.0f;
                    var y = 0.0f;
                    var pos = divideP[dpIndex].pos;
                    switch (deleteAxis)
                    {
                        case 0: x = pos.z; y = pos.y; break;
                        case 1: x = pos.x; y = pos.z; break;
                        case 2: x = pos.x; y = pos.y; break;
                    }

                    xMin = math.min(x, xMin);
                    xMax = math.max(x, xMax);
                    yMin = math.min(y, yMin);
                    yMax = math.max(y, yMax);
                    
                    if (Hint.Unlikely(nextIndex == firstIndex))
                    {
                        isLoop = true;
                        break;
                    }
                    beforeIndex = thisIndex;
                }

                var _loopInfo = new LoopInfo(firstIndex, isLoop, xMin, xMax, yMin, yMax);
                loopInfo.Add(_loopInfo);
            }
        }
    }

    [BurstCompile]
    public struct LoopInfo
    {
        public int firstIndex;
        public bool isLoop;
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;
        public float area;

        public LoopInfo(int firstIndex, bool isLoop, float xMin, float xMax, float yMin, float yMax)
        {
            this.firstIndex = firstIndex;
            this.isLoop = isLoop;
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            this.area = (xMax - xMin) * (yMax - yMin);
        }
    }

    public struct CutMeshMapping: IJobParallelFor, IDisposable
    {
        public int deleteAxis;

        [ReadOnly]
        public NativeList<DividePoint> divideP;

        [WriteOnly]
        public NativeList<Vector2> cutMeshMapped;

        public void Dispose()
        {
            cutMeshMapped.Dispose();
        }

        public void Execute(int index)
        {
            var pos = divideP[index].pos;
            switch (deleteAxis)
            {
                case 0: cutMeshMapped[index] = new Vector2(pos.z, pos.y); break;
                case 1: cutMeshMapped[index] = new Vector2(pos.x, pos.z); break;
                case 2: cutMeshMapped[index] = new Vector2(pos.x, pos.y); break;
            }
        }
    }

    [BurstCompile]
    public struct LoopPairingJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeList<Vector2> cutMeshMapped;

        [ReadOnly]
        public NativeList<int> loopNo;

        [ReadOnly]
        public NativeList<LoopInfo> loopInfo;

        [WriteOnly]
        public NativeParallelMultiHashMap<int, int> loopPair;

        public void Dispose()
        {
            loopPair.Dispose();
        }

        public void Execute()
        {
            var loopOrder = new NativeList<LoopOrderTuple>(loopInfo.Length, Allocator.Temp);
            for(var i = 0; i < loopInfo.Length; i++)
            {
                loopOrder.Add(new LoopOrderTuple() { index = i, area = loopInfo[i].area });
            }
            var comp = new LoopOrderTupleComp();
            loopOrder.Sort(comp);

            var checkedSet = new NativeParallelHashSet<int>(loopInfo.Length, Allocator.Temp);
            for (var i = loopInfo.Length - 1; i > 0; i--)
            {
                if (checkedSet.Contains(i))
                {
                    continue;
                }

                for (var j = i - 1; j >= 0; j--)
                {
                    if (checkedSet.Contains(j))
                    {
                        continue;
                    }

                    var inner = loopInfo[loopOrder[j].index];
                    var outer = loopInfo[loopOrder[i].index];
                    if (inner.xMin < outer.xMin || inner.xMax > outer.xMax || inner.yMin < outer.yMin || inner.yMax > outer.yMax)
                    {
                        continue;
                    }

                }
            }
        }

        private bool isInner(int inner, int outer)
        {

        }
    }

    [BurstCompile]
    public struct LoopOrderTupleComp: IComparer<LoopOrderTuple>
    {
        public readonly int Compare(LoopOrderTuple x, LoopOrderTuple y)
        {
            return x.area.CompareTo(y.area);
        }
    }

    [BurstCompile]
    public struct LoopOrderTuple
    {
        public int index;
        public float area;
    }

    [BurstCompile]
    public struct OrderingJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeArray<int> indexes;

        [WriteOnly]
        public NativeList<int> useIndexes;

        public NativeParallelHashMap<int, int> mapping;

        public void Dispose()
        {
            useIndexes.Dispose();
            mapping.Dispose();
        }

        public void Execute()
        {
            int i = 0;
            for (var j = 0; j < indexes.Length; j++)
            {
                var index = indexes[j];
                if (Hint.Likely(mapping.TryAdd(index, i)))
                {
                    useIndexes.Add(index);
                    i++;
                }
            }
        }
    }

    [BurstCompile]
    public struct IdentifyDIndexJob : IJobParallelFor, IDisposable
    {
        public NativeArray<int> indexes;

        public int aggDCount;

        public void Dispose()
        {
            indexes.Dispose();
        }

        public void Execute(int index)
        {
            var i = indexes[index];
            indexes[index] = i - aggDCount;
        }
    }

    [BurstCompile]
    public struct IdentifyIndexJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeList<int> indexes;

        [WriteOnly]
        public NativeList<int> outIndexes;

        public int aggDCount;

        public void Dispose()
        {
            outIndexes.Dispose();
        }

        public void Execute()
        {
            for (var idx = 0; idx < indexes.Length; idx++)
            {
                var i = indexes[idx];
                outIndexes.Add(i < 0 ? i - aggDCount : i);
            }
        }
    }

    [BurstCompile]
    public struct UniqueOrderingJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeArray<int> indexes;

        [ReadOnly]
        public NativeArray<int> aggICounts;

        [ReadOnly]
        public NativeArray<DividePoint> divideP;

        public NativeParallelHashMap<int, int> mapping;

        public NativeList<DividePoint> uniqueDivideP;

        public void Dispose()
        {
            mapping.Dispose();
            uniqueDivideP.Dispose();
        }

        public void Execute()
        {
            var i = 0;
            ushort j = 0;
            var unique = new NativeParallelHashMap<DividePoint, int>(indexes.Length, Allocator.Temp);
            for (var k = 0; k < aggICounts.Length; k++)
            {
                var ac = aggICounts[k];
                while (i < ac)
                {
                    var idx = -indexes[i] - 1;
                    if (Hint.Likely(unique.TryAdd(divideP[idx], j)))
                    {
                        j++;
                    }
                    mapping.Add(indexes[i], unique[divideP[idx]]);
                    i++;
                }
            }

            uniqueDivideP.ResizeUninitialized(unique.Count());

            foreach (var kv in unique)
            {
                var value = kv.Value;
                uniqueDivideP[value] = kv.Key;
            }
        }
    }

    [BurstCompile]
    public struct FinalIndexing : IJobParallelFor, IDisposable
    {
        [ReadOnly]
        public NativeArray<int> indexes;

        [ReadOnly]
        public NativeParallelHashMap<int, int> existMapping;

        [ReadOnly]
        public NativeList<int> useIndexes;

        [ReadOnly]
        public NativeParallelHashMap<int, int> newMapping;

        [WriteOnly]
        public NativeArray<int> indicates;

        public void Dispose()
        {
            indicates.Dispose();
        }

        public void Execute(int index)
        {
            var value = indexes[index];
            var convert = value < 0 ? newMapping[value] + useIndexes.Length : existMapping[value];
            indicates[index] = convert;
        }
    }

    [BurstCompile]
    public struct FinalVertexJob : IJob, IDisposable
    {
        [ReadOnly]
        public NativeArray<Vector3> vertexs;

        [ReadOnly]
        public NativeArray<Vector3> normals;

        [ReadOnly]
        public NativeArray<Vector2> uvs;

        [ReadOnly]
        public NativeArray<int> useIndexes;

        [ReadOnly]
        public NativeList<DividePoint> useDivideP;

        [WriteOnly]
        public NativeArray<VertexData> vertexData;

        public void Dispose()
        {
            vertexData.Dispose();
        }

        public void Execute()
        {
            var i = 0;
            for (var j = 0; j < useIndexes.Length; j++)
            {
                var idx = useIndexes[j];
                vertexData[i] = new VertexData(vertexs[idx], normals[idx], uvs[idx]);
                i++;
            }

            for (var j = 0; j < useDivideP.Length; j++)
            {
                var divideP = useDivideP[j];
                var pos = divideP.pos;
                var normal = math.lerp(normals[divideP.idx0], normals[divideP.idx1], divideP.t);
                var uv = math.lerp(uvs[divideP.idx0], uvs[divideP.idx1], divideP.t);
                vertexData[i] = new VertexData(pos, normal, uv);
                i++;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexData
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;

        public VertexData(Vector3 position, Vector3 normal, Vector2 uv)
        {
            this.position = position;
            this.normal = normal;
            this.uv = uv;
        }
    }
}
