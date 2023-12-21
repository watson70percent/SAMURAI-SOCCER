using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SamuraiSoccer.StageContents.BattleDome
{
    /// <summary>
    /// BSA�t�@�C���������N���X�B
    /// </summary>
    public sealed class BSA
    {
        private byte[] m_raw;
        private float m_strideTime;
        private byte m_fCount;
        private byte m_levelCount;
        private uint m_sampleCount;

        /// <summary>
        /// �t�@�C�����ǂݍ��܂�g�p�ł��邩�ǂ����B
        /// </summary>
        public bool IsAvailable { get; private set; } = false;

        /// <summary>
        /// �T���v���Ԃ̎��ԊԊu�is�j�B
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
        /// ���g���T���v�����B
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
        /// �������x���̕���\�B
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
        /// �������x����0-1�ɂ��鎞�̐��K���W���B
        /// </summary>
        public double LevelUnit => 1.0 / LevelCount;

        /// <summary>
        /// �T���v�����B
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
        /// �t�@�C�������[�h����B
        /// </summary>
        /// <param name="fileName">
        /// �ǂݍ��ރt�@�C�����B
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
        /// �������x���̃f�[�^���擾����B
        /// </summary>
        /// <param name="time">
        /// ���Ԃ̃C���f�b�N�X�B
        /// </param>
        /// <returns>
        /// �������x���̃f�[�^�B
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// �C���f�b�N�X���s���̏ꍇ��������B
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
        /// �t�@�C�����ǂݍ��܂�g�p�ł��邩�`�F�b�N�B
        /// </summary>
        /// <exception cref="MemberAccessException">
        /// �t�@�C�����ǂݍ��܂�Ă��Ȃ��ꍇ��������B
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