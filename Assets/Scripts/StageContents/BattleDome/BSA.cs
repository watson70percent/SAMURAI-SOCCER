using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SamuraiSoccer.StageContents.BattleDome
{
    /// <summary>
    /// BSAファイルを扱うクラス。
    /// </summary>
    public sealed class BSA
    {
        private byte[] m_raw;
        private float m_strideTime;
        private byte m_fCount;
        private byte m_levelCount;
        private uint m_sampleCount;

        /// <summary>
        /// ファイルが読み込まれ使用できるかどうか。
        /// </summary>
        public bool IsAvailable { get; private set; } = false;

        /// <summary>
        /// サンプル間の時間間隔（s）。
        /// </summary>
        public float StrideTime
        {
            get
            {
                AvailableCheck();
                return m_strideTime;
            }
            private set { m_strideTime = value; }
        }

        /// <summary>
        /// 周波数サンプル数。
        /// </summary>
        public byte FCount
        {
            get
            {
                AvailableCheck();
                return m_fCount;
            }
            private set { m_fCount = value; }
        }

        /// <summary>
        /// 音圧レベルの分解能。
        /// </summary>
        public byte LevelCount
        {
            get
            {
                AvailableCheck();
                return m_levelCount;
            }
            private set { m_levelCount = value; }
        }

        /// <summary>
        /// 音圧レベルを0-1にする時の正規化係数。
        /// </summary>
        public double LevelUnit => 1.0 / LevelCount;

        /// <summary>
        /// サンプル数。
        /// </summary>
        public uint SampleCount
        {
            get
            {
                AvailableCheck();
                return m_sampleCount;
            }
            private set { m_sampleCount = value; }
        }

        /// <summary>
        /// ファイルをロードする。
        /// </summary>
        /// <param name="fileName">
        /// 読み込むファイル名。
        /// </param>
        /// <returns>
        /// UniTask
        /// </returns>
        public async UniTask Load(string fileName)
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (filePath.Contains("://"))
            {
                var www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
                await www.SendWebRequest();
                m_raw = www.downloadHandler.data;
            }
            else
            {
                m_raw = File.ReadAllBytes(filePath);
            }

            var strideLevel = m_raw[0];
            var strideSample = 1 << strideLevel;
            StrideTime = (float)((double)strideSample / 44100);
            FCount = m_raw[1];
            LevelCount = m_raw[2];

            var sampleCountRaw = m_raw[3..7];
            SampleCount = BitConverter.IsLittleEndian ? BitConverter.ToUInt32(sampleCountRaw) : BitConverter.ToUInt32(sampleCountRaw.Reverse().ToArray());

            IsAvailable = true;
        }

        /// <summary>
        /// 音圧レベルのデータを取得する。
        /// </summary>
        /// <param name="time">
        /// 時間のインデックス。
        /// </param>
        /// <returns>
        /// 音圧レベルのデータ。
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// インデックスが不正の場合投げられる。
        /// </exception>
        public ReadOnlySpan<byte> LevelData(int time)
        {
            if (time < 0 || time >= SampleCount)
            {
                throw new IndexOutOfRangeException("time is out of range");
            }
            var start = time * FCount + 7;
            var end = start + FCount;
            return m_raw[start..end];
        }

        /// <summary>
        /// ファイルが読み込まれ使用できるかチェック。
        /// </summary>
        /// <exception cref="MemberAccessException">
        /// ファイルが読み込まれていない場合投げられる。
        /// </exception>
        private void AvailableCheck()
        {
            if (!IsAvailable)
            {
                throw new MemberAccessException("This class is not ready");
            }
        }
    }
}