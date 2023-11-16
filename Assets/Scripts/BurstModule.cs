using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace SamuraiSoccer
{
    [BurstCompile]
    public struct VertexSideJudgeJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> vertex;

        [ReadOnly]
        public float3 dir;

        [ReadOnly]
        public float3 pos;

        [WriteOnly]
        public NativeArray<byte> side;

        [WriteOnly]
        public NativeArray<float> normal;

        public void Execute(int index)
        {
            float3 vp = vertex[index];
            float n = math.dot(dir, vp - pos);
            normal[index] = n;
            side[index] = (byte)math.step(0.0f, n);
        }
    }

    [BurstCompile]
    public struct MeshSideJudgeJob : IJob
    {
        [ReadOnly]
        public NativeArray<byte> side;

        [WriteOnly]
        public NativeArray<ushort> forward;

        [WriteOnly]
        public NativeArray<ushort> backward;

        [WriteOnly]
        public NativeArray<ushort> center;


        public void Execute()
        {
            var triCount = side.Length / 3;
            using var _forward = new NativeList<ushort>(triCount * 2, Allocator.Temp);
            using var _backward = new NativeList<ushort>(triCount * 2, Allocator.Temp);
            using var _center = new NativeList<ushort>(triCount, Allocator.Temp);
            for (ushort i = 0; i < triCount; i++)
            {
                var i0 = 3 * i;
                var i1 = 3 * i + 1;
                var i2 = 3 * i + 2;
                var sideScore = side[i0] + side[i1] + side[i2];
                switch (sideScore)
                {
                    case 0:
                        _backward.Add((ushort)i0);
                        _backward.Add((ushort)i1);
                        _backward.Add((ushort)i2); break;
                    case 3:
                        _forward.Add((ushort)i0);
                        _forward.Add((ushort)i1);
                        _forward.Add((ushort)i2); break;
                    default:
                        _center.Add((ushort)i0);
                        _center.Add((ushort)i1);
                        _center.Add((ushort)i2); break;
                }
            }
            forward = _forward.ToArray(Allocator.TempJob);
            backward = _backward.ToArray(Allocator.TempJob);
            center = _center.ToArray(Allocator.TempJob);
        }
    }

    [BurstCompile]
    public struct IndexJob : IJob
    {
        [ReadOnly]
        public NativeArray<ushort> center;

        [WriteOnly]
        public NativeParallelMultiHashMap<ushort, int> vertexIndex;

        public void Execute()
        {
            vertexIndex = new NativeParallelMultiHashMap<ushort, int>(center.Length, Allocator.TempJob);
            var triCount = center.Length / 3;
            for (int i = 0; i < triCount; i++)
            {
                vertexIndex.Add(center[3 * i], i);
                vertexIndex.Add(center[3 * i + 1], i);
                vertexIndex.Add(center[3 * i + 2], i);
            }
        }
    }

    [BurstCompile]
    public struct DivideJob : IJob
    {
        [ReadOnly]
        public NativeArray<float> normal;

        [ReadOnly]
        public NativeArray<byte> side;

        [ReadOnly]
        public NativeArray<Vector3> vertexPos;

        [ReadOnly]
        public NativeArray<ushort> center;

        [ReadOnly]
        public NativeParallelMultiHashMap<ushort, int> vertexIndex;

        [WriteOnly]
        public NativeArray<DividePoint> divideP;

        public NativeArray<DivideInfo> divideInfo;

        public void Execute()
        {
            var _divideP = new NativeList<DividePoint>(center.Length / 2, Allocator.Temp);

            var triCount = center.Length / 3;
            for (int i = 0; i < triCount; i++)
            {
                var i1 = 3 * i;
                var i2 = 3 * i + 1;
                var i3 = 3 * i + 2;
                var s1 = side[center[i1]];
                var s2 = side[center[i2]];
                var s3 = side[center[i3]];
                var singleIndex = (ushort)(3.0f - math.ceil(math.abs(s1 + s2 * 2 + s3 * 4 - 3.5f)));
                var singleSide = side[center[i1 + singleIndex]] == 0;
                ushort firstIdx, secondIdx;
                firstIdx = secondIdx = 0;
                switch (singleIndex)
                {
                    case 0: firstIdx = center[i2]; secondIdx = center[i3]; break;
                    case 1: firstIdx = center[i1]; secondIdx = center[i3]; break;
                    case 2: firstIdx = center[i1]; secondIdx = center[i2]; break;
                }
                var singleCount = vertexIndex.CountValuesForKey(singleIndex) - 1;
                Span<int> singleArray = stackalloc int[singleCount];
                var singlev = vertexIndex.GetValuesForKey(singleIndex);
                for (int j = 0; j < singleCount; j++)
                {
                    if (singlev.Current == i)
                    {
                        singlev.MoveNext();
                    }
                    singleArray[j] = singlev.Current;
                    singlev.MoveNext();
                }

                var firstv = vertexIndex.GetValuesForKey(firstIdx);
                var sideMeshIndex0 = FindMatchData(singleArray, firstv);

                var secondv = vertexIndex.GetValuesForKey(secondIdx);
                var sideMeshIndex1 = FindMatchData(singleArray, secondv);

                int divideIndex0, divideIndex1;

                if (sideMeshIndex0 < i)
                {
                    var sideInfo = divideInfo[sideMeshIndex0];
                    divideIndex0 = sideInfo.sideMeshIndex0 == i ? sideInfo.divideIndex0 : sideInfo.divideIndex1;
                }
                else
                {
                    var p2abs = math.abs(normal[firstIdx]);
                    var p1abs = math.abs(normal[singleIndex]);
                    var t = p2abs / (p1abs + p2abs); // 0:close to firstIdx, 1:close to singleIndex
                    var _pos = math.lerp(vertexPos[firstIdx], vertexPos[singleIndex], t);
                    var p = new DividePoint(firstIdx, singleIndex, t, _pos);
                    divideIndex0 = _divideP.Length;
                    _divideP.Add(p);
                }

                if (sideMeshIndex1 < i)
                {
                    var sideInfo = divideInfo[sideMeshIndex1];
                    divideIndex1 = sideInfo.sideMeshIndex0 == i ? sideInfo.divideIndex0 : sideInfo.divideIndex1;
                }
                else
                {
                    var p2abs = math.abs(normal[secondIdx]);
                    var p1abs = math.abs(normal[singleIndex]);
                    var t = p2abs / (p1abs + p2abs); // 0:close to secondIdx, 1:close to singleIndex
                    var _pos = math.lerp(vertexPos[secondIdx], vertexPos[singleIndex], t);
                    var p = new DividePoint(secondIdx, singleIndex, t, _pos);
                    divideIndex1 = _divideP.Length;
                    _divideP.Add(p);
                }

                var dir = (float3)_divideP[divideIndex1].pos - (float3)_divideP[divideIndex0].pos;
                dir = math.normalize(dir);

                var info = new DivideInfo(singleIndex, singleSide, sideMeshIndex0, sideMeshIndex1, divideIndex0, divideIndex1, dir);
                divideInfo[i] = info;
            }

            divideP = _divideP.ToArray(Allocator.TempJob);

            for (int i = 1; i < triCount; i++)
            {
                var selfInfo = divideInfo[i];
                var selfDir = selfInfo.dir;
                var sideIndex0 = selfInfo.sideMeshIndex0;
                var sideIndex1 = selfInfo.sideMeshIndex1;

                if (sideIndex0 < i)
                {
                    var sideDir = divideInfo[sideIndex0].dir;
                    var isStraight = 0.99999 < math.abs(math.dot(sideDir, selfDir));
                    divideInfo[i].SetStraight0(isStraight);
                    var isSide0 = divideInfo[sideIndex0].sideMeshIndex0 == i;
                    if (isSide0)
                    {
                        divideInfo[sideIndex0].SetStraight0(isStraight);
                    }
                    else
                    {
                        divideInfo[sideIndex0].SetStraight1(isStraight);
                    }
                }

                if (sideIndex1 < i)
                {
                    var sideDir = divideInfo[sideIndex1].dir;
                    var isStraight = 0.99999 < math.abs(math.dot(sideDir, selfDir));
                    divideInfo[i].SetStraight1(isStraight);
                    var isSide0 = divideInfo[sideIndex1].sideMeshIndex0 == i;
                    if (isSide0)
                    {
                        divideInfo[sideIndex1].SetStraight0(isStraight);
                    }
                    else
                    {
                        divideInfo[sideIndex1].SetStraight1(isStraight);
                    }
                }
            }
        }

        private int FindMatchData(in Span<int> src, in NativeParallelMultiHashMap<ushort, int>.Enumerator dst)
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

    public struct DividePoint : IEquatable<DividePoint>
    {
        public readonly ushort idx0;
        public readonly ushort idx1;
        public float t;
        public Vector3 pos;

        public DividePoint(ushort idx0, ushort idx1, float t, float3 pos)
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

        public void SetStraight0(bool isStraight0)
        {
            this.isStraight0 = isStraight0;
        }

        public void SetStraight1(bool isStraight1)
        {
            this.isStraight1 = isStraight1;
        }
    }

    [BurstCompile]
    public struct MakeTriangleJob : IJob
    {
        [ReadOnly]
        public NativeArray<ushort> center;

        [ReadOnly]
        public NativeArray<DivideInfo> divideInfos;

        [WriteOnly]
        public NativeArray<int> forward;

        [WriteOnly]
        public NativeArray<int> backward;

        public NativeParallelHashSet<int> forwardDivideIndex;

        public NativeParallelHashSet<int> backwardDivideIndex;

        public void Execute()
        {
            var _forward = new NativeList<int>();
            var _backward = new NativeList<int>();
            var forwardAdded = new NativeParallelHashSet<int>();
            var backwardAdded = new NativeParallelHashSet<int>();

            for (var i = 0; i < divideInfos.Length; i++)
            {
                var idx0 = i * 3;
                var idx1 = i * 3 + 1;
                var idx2 = i * 3 + 2;
                var info = divideInfos[i];
                if (info.singleSide)
                {
                    // two point is forward
                    {
                        // forward
                        var divideIndex0 = info.divideIndex0;
                        var divideIndex1 = info.divideIndex1;

                        if (info.isStraight0 && !forwardAdded.Contains(info.sideMeshIndex0))
                        {
                            RectExtendForward(ref divideIndex0, ref forwardAdded, i, info.sideMeshIndex0);
                        }

                        if (info.isStraight1 && !forwardAdded.Contains(info.sideMeshIndex1))
                        {
                            RectExtendForward(ref divideIndex1, ref forwardAdded, i, info.sideMeshIndex1);
                        }


                        switch (info.singleIndex)
                        {
                            case 0:
                                _forward.Add(-divideIndex0 - 1); _forward.Add(center[idx1]); _forward.Add(-divideIndex1 - 1);
                                _forward.Add(center[idx1]); _forward.Add(center[idx2]); _forward.Add(-divideIndex1 - 1); break;
                            case 1:
                                _forward.Add(center[idx0]); _forward.Add(-divideIndex0 - 1); _forward.Add(-divideIndex1 - 1);
                                _forward.Add(-divideIndex1 - 1); _forward.Add(center[idx2]); _forward.Add(center[idx0]); break;
                            case 2:
                                _forward.Add(center[idx0]); _forward.Add(center[idx1]); _forward.Add(-divideIndex0 - 1);
                                _forward.Add(-divideIndex0 - 1); _forward.Add(center[idx1]); _forward.Add(-divideIndex1 - 1); break;
                        }

                        forwardDivideIndex.Add(divideIndex0 - 1);
                        forwardDivideIndex.Add(divideIndex1 - 1);

                    }

                    #region backward one
                    {
                        // backward
                        var divideIndex0 = info.divideIndex0;
                        var divideIndex1 = info.divideIndex1;
                        var vindex = -1;
                        var is0Side = false;

                        if (info.isStraight0 && !backwardAdded.Contains(info.sideMeshIndex0))
                        {
                            TriangleExtendBackward(ref divideIndex0, ref vindex, ref backwardAdded, i, info.sideMeshIndex0, center[idx0 + info.singleIndex]);
                            if (vindex >= 0)
                            {
                                is0Side = true;
                            }
                        }

                        if (info.isStraight1 && !backwardAdded.Contains(info.sideMeshIndex1))
                        {
                            TriangleExtendBackward(ref divideIndex1, ref vindex, ref backwardAdded, i, info.sideMeshIndex1, center[idx0 + info.singleIndex]);
                        }

                        if (vindex >= 0)
                        {
                            if (is0Side)
                            {
                                switch (info.singleIndex)
                                {
                                    case 0:
                                        _backward.Add(center[idx0]); _backward.Add(center[vindex]); _backward.Add(-info.divideIndex0 - 1);
                                        _backward.Add(-info.divideIndex0 - 1); _backward.Add(-info.divideIndex1 - 1); _backward.Add(center[idx0]); break;
                                    case 1:
                                        _backward.Add(-info.divideIndex0 - 1); _backward.Add(center[vindex]); _backward.Add(center[idx1]);
                                        _backward.Add(center[idx1]); _backward.Add(-info.divideIndex1 - 1); _backward.Add(-info.divideIndex0 - 1); break;
                                    case 2:
                                        _backward.Add(center[idx2]); _backward.Add(center[vindex]); _backward.Add(-info.divideIndex0 - 1);
                                        _backward.Add(-info.divideIndex0 - 1); _backward.Add(-info.divideIndex1 - 1); _backward.Add(center[idx2]); break;
                                }
                            }
                            else
                            {
                                switch (info.singleIndex)
                                {
                                    case 0:
                                        _backward.Add(center[idx0]); _backward.Add(-info.divideIndex0 - 1); _backward.Add(-info.divideIndex1 - 1);
                                        _backward.Add(-info.divideIndex1 - 1); _backward.Add(center[vindex]); _backward.Add(center[idx0]); break;
                                    case 1:
                                        _backward.Add(center[idx0]); _backward.Add(-info.divideIndex0 - 1); _backward.Add(-info.divideIndex1 - 1);
                                        _backward.Add(-info.divideIndex1 - 1); _backward.Add(center[idx2]); _backward.Add(center[idx0]); break;
                                    case 2:
                                        _backward.Add(center[idx2]); _backward.Add(-info.divideIndex0 - 1); _backward.Add(-info.divideIndex1 - 1);
                                        _backward.Add(-info.divideIndex1 - 1); _backward.Add(center[vindex]); _backward.Add(center[idx2]); break;
                                }
                            }

                        }
                        else
                        {
                            switch (info.singleIndex)
                            {
                                case 0:
                                    _backward.Add(center[idx0]); _backward.Add(-info.divideIndex0 - 1); _backward.Add(-info.divideIndex1 - 1); break;
                                case 1:
                                    _backward.Add(center[idx1]); _backward.Add(-info.divideIndex1 - 1); _backward.Add(-info.divideIndex0 - 1); break;
                                case 2:
                                    _backward.Add(-info.divideIndex0 - 1); _backward.Add(-info.divideIndex1 - 1); _backward.Add(center[idx2]); break;
                            }
                        }

                        backwardDivideIndex.Add(divideIndex0 - 1);
                        backwardDivideIndex.Add(divideIndex1 - 1);
                    }
                    #endregion backward one
                }
                else
                {
                    // two point is backward
                    {
                        // backward
                        var divideIndex0 = info.divideIndex0;
                        var divideIndex1 = info.divideIndex1;

                        if (info.isStraight0 && !backwardAdded.Contains(info.sideMeshIndex0))
                        {
                            RectExtendBackward(ref divideIndex0, ref backwardAdded, i, info.sideMeshIndex0);
                        }

                        if (info.isStraight1 && !backwardAdded.Contains(info.sideMeshIndex1))
                        {
                            RectExtendBackward(ref divideIndex1, ref backwardAdded, i, info.sideMeshIndex1);
                        }

                        switch (info.singleIndex)
                        {
                            case 0:
                                _backward.Add(-divideIndex0 - 1); _backward.Add(center[idx1]); _backward.Add(-divideIndex1 - 1);
                                _backward.Add(center[idx1]); _backward.Add(center[idx2]); _backward.Add(-divideIndex1 - 1); break;
                            case 1:
                                _backward.Add(center[idx0]); _backward.Add(-divideIndex0 - 1); _backward.Add(-divideIndex1 - 1);
                                _backward.Add(-divideIndex1 - 1); _backward.Add(center[idx2]); _backward.Add(center[idx0]); break;
                            case 2:
                                _backward.Add(center[idx0]); _backward.Add(center[idx1]); _backward.Add(-divideIndex0 - 1);
                                _backward.Add(-divideIndex0 - 1); _backward.Add(center[idx1]); _backward.Add(-divideIndex1 - 1); break;
                        }

                        backwardDivideIndex.Add(divideIndex0 - 1);
                        backwardDivideIndex.Add(divideIndex1 - 1);

                    }
                    #region forward one
                    {
                        // forward
                        // backward
                        var divideIndex0 = info.divideIndex0;
                        var divideIndex1 = info.divideIndex1;
                        var vindex = -1;
                        var is0Side = false;

                        if (info.isStraight0 && !forwardAdded.Contains(info.sideMeshIndex0))
                        {
                            TriangleExtendForward(ref divideIndex0, ref vindex, ref forwardAdded, i, info.sideMeshIndex0, center[idx0 + info.singleIndex]);
                            if (vindex >= 0)
                            {
                                is0Side = true;
                            }
                        }

                        if (info.isStraight1 && !forwardAdded.Contains(info.sideMeshIndex1))
                        {
                            TriangleExtendForward(ref divideIndex1, ref vindex, ref forwardAdded, i, info.sideMeshIndex1, center[idx0 + info.singleIndex]);
                        }

                        if (vindex >= 0)
                        {
                            if (is0Side)
                            {
                                switch (info.singleIndex)
                                {
                                    case 0:
                                        _forward.Add(center[idx0]); _forward.Add(center[vindex]); _forward.Add(-info.divideIndex0 - 1);
                                        _forward.Add(-info.divideIndex0 - 1); _forward.Add(-info.divideIndex1 - 1); _forward.Add(center[idx0]); break;
                                    case 1:
                                        _forward.Add(-info.divideIndex0 - 1); _forward.Add(center[vindex]); _forward.Add(center[idx1]);
                                        _forward.Add(center[idx1]); _forward.Add(-info.divideIndex1 - 1); _forward.Add(-info.divideIndex0 - 1); break;
                                    case 2:
                                        _forward.Add(center[idx2]); _forward.Add(center[vindex]); _forward.Add(-info.divideIndex0 - 1);
                                        _forward.Add(-info.divideIndex0 - 1); _forward.Add(-info.divideIndex1 - 1); _forward.Add(center[idx2]); break;
                                }
                            }
                            else
                            {
                                switch (info.singleIndex)
                                {
                                    case 0:
                                        _forward.Add(center[idx0]); _forward.Add(-info.divideIndex0 - 1); _forward.Add(-info.divideIndex1 - 1);
                                        _forward.Add(-info.divideIndex1 - 1); _forward.Add(center[vindex]); _forward.Add(center[idx0]); break;
                                    case 1:
                                        _forward.Add(center[idx0]); _forward.Add(-info.divideIndex0 - 1); _forward.Add(-info.divideIndex1 - 1);
                                        _forward.Add(-info.divideIndex1 - 1); _forward.Add(center[idx2]); _forward.Add(center[idx0]); break;
                                    case 2:
                                        _forward.Add(center[idx2]); _forward.Add(-info.divideIndex0 - 1); _forward.Add(-info.divideIndex1 - 1);
                                        _forward.Add(-info.divideIndex1 - 1); _forward.Add(center[vindex]); _forward.Add(center[idx2]); break;
                                }
                            }

                        }
                        else
                        {
                            switch (info.singleIndex)
                            {
                                case 0:
                                    _forward.Add(center[idx0]); _forward.Add(-info.divideIndex0 - 1); _forward.Add(-info.divideIndex1 - 1); break;
                                case 1:
                                    _forward.Add(center[idx1]); _forward.Add(-info.divideIndex1 - 1); _forward.Add(-info.divideIndex0 - 1); break;
                                case 2:
                                    _forward.Add(-info.divideIndex0 - 1); _forward.Add(-info.divideIndex1 - 1); _forward.Add(center[idx2]); break;
                            }
                        }

                        forwardDivideIndex.Add(divideIndex0 - 1);
                        forwardDivideIndex.Add(divideIndex1 - 1);

                    }
                    #endregion forward one
                }
            }

            forward = _forward.ToArray(Allocator.TempJob);
            backward = _backward.ToArray(Allocator.TempJob);
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
                    if (!nextInfo.isStraight1 || forwardAdded.Contains(next))
                    {
                        return;
                    }
                }
                else
                {
                    divideIndex = nextInfo.divideIndex0;
                    before = next;
                    next = nextInfo.sideMeshIndex0;
                    if (!nextInfo.isStraight0 || forwardAdded.Contains(next))
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
                    if (!nextInfo.isStraight1 || backwardAdded.Contains(next))
                    {
                        return;
                    }
                }
                else
                {
                    divideIndex = nextInfo.divideIndex0;
                    before = next;
                    next = nextInfo.sideMeshIndex0;
                    if (!nextInfo.isStraight0 || backwardAdded.Contains(next))
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
                    if (!isRect)
                    {
                        var n0 = next * 3;
                        var n1 = next * 3 + 1;
                        var n2 = next * 3 + 2;
                        switch (nextInfo.singleIndex)
                        {
                            case 0: vindex = center[n1] == triangelIndex ? center[n2] : center[n1]; break;
                            case 1: vindex = center[n0] == triangelIndex ? center[n2] : center[n0]; break;
                            case 2: vindex = center[n0] == triangelIndex ? center[n1] : center[n0]; break;
                        }
                    }
                    before = next;
                    next = nextInfo.sideMeshIndex1;
                    if (!nextInfo.isStraight1 || forwardAdded.Contains(next))
                    {
                        return;
                    }
                }
                else
                {
                    divideIndex = nextInfo.divideIndex0;
                    if (!isRect)
                    {
                        var n0 = next * 3;
                        var n1 = next * 3 + 1;
                        var n2 = next * 3 + 2;
                        switch (nextInfo.singleIndex)
                        {
                            case 0: vindex = center[n1] == triangelIndex ? center[n2] : center[n1]; break;
                            case 1: vindex = center[n0] == triangelIndex ? center[n2] : center[n0]; break;
                            case 2: vindex = center[n0] == triangelIndex ? center[n1] : center[n0]; break;
                        }
                    }
                    before = next;
                    next = nextInfo.sideMeshIndex0;
                    if (!nextInfo.isStraight0 || forwardAdded.Contains(next))
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
                    if (!isRect)
                    {
                        var n0 = next * 3;
                        var n1 = next * 3 + 1;
                        var n2 = next * 3 + 2;
                        switch (nextInfo.singleIndex)
                        {
                            case 0: vindex = center[n1] == triangelIndex ? center[n2] : center[n1]; break;
                            case 1: vindex = center[n0] == triangelIndex ? center[n2] : center[n0]; break;
                            case 2: vindex = center[n0] == triangelIndex ? center[n1] : center[n0]; break;
                        }
                    }
                    before = next;
                    next = nextInfo.sideMeshIndex1;
                    if (!nextInfo.isStraight1 || backwardAdded.Contains(next))
                    {
                        return;
                    }
                }
                else
                {
                    divideIndex = nextInfo.divideIndex0;
                    if (!isRect)
                    {
                        var n0 = next * 3;
                        var n1 = next * 3 + 1;
                        var n2 = next * 3 + 2;
                        switch (nextInfo.singleIndex)
                        {
                            case 0: vindex = center[n1] == triangelIndex ? center[n2] : center[n1]; break;
                            case 1: vindex = center[n0] == triangelIndex ? center[n2] : center[n0]; break;
                            case 2: vindex = center[n0] == triangelIndex ? center[n1] : center[n0]; break;
                        }
                    }
                    before = next;
                    next = nextInfo.sideMeshIndex0;
                    if (!nextInfo.isStraight0 || backwardAdded.Contains(next))
                    {
                        return;
                    }
                }
            }
        }
    }

    [BurstCompile]
    public struct OrderingJob : IJob
    {
        [ReadOnly]
        public NativeArray<ushort> indexes;

        [WriteOnly]
        public NativeArray<int> indexCount;

        public NativeParallelHashMap<ushort, ushort> mapping;

        public void Execute()
        {
            ushort i = 0;
            foreach (var index in indexes)
            {
                if (mapping.TryAdd(index, i))
                {
                    i++;
                }
            }
            indexCount[0] = i;
        }
    }

    [BurstCompile]
    public struct IdentifyIndexJob : IJobParallelFor
    {
        public NativeArray<int> indexes;

        public int aggDCount;

        public void Execute(int index)
        {
            var i = indexes[index];
            indexes[index] = i < 0 ? i - aggDCount : i;
        }
    }

    [BurstCompile]
    public struct UniqueOrderingJob : IJob
    {
        [ReadOnly]
        public NativeArray<int> indexes;

        [ReadOnly]
        public NativeArray<int> aggICounts;

        [ReadOnly]
        public NativeArray<DividePoint> divideP;

        public NativeParallelHashMap<int, int> mapping;

        [WriteOnly]
        public NativeArray<DividePoint> uniqueDivideP;

        public void Execute()
        {
            var i = 0;
            var j = -1;
            var unique = new NativeParallelHashMap<DividePoint, int>(indexes.Length, Allocator.Temp);
            foreach(var ac in aggICounts)
            {
                while (ac < i)
                {
                    var idx = -indexes[i] + 1; // index = dindex - dc
                    if (unique.TryAdd(divideP[idx], j))
                    {
                        j--;
                    }
                    mapping.Add(indexes[i], j);
                    i++;
                }
            }

            var count = unique.Count();
            uniqueDivideP = new NativeArray<DividePoint>(count, Allocator.TempJob);
            foreach (var kv in unique)
            {
                var value = kv.Value;
                uniqueDivideP[-value - 1] = kv.Key;
            }
        }
    }

    [BurstCompile]
    public struct FinalIndexing : IJob
    {
        [ReadOnly]
        public NativeArray<int> index;

        [WriteOnly]
        public NativeArray<ushort> indicates;

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }

}
