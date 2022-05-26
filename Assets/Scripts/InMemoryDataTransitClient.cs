using Cysharp.Threading.Tasks;

using System.Collections.Generic;

namespace SamuraiSoccer
{
    /// <summary>
    /// �V�[���Ԃ̃f�[�^�󂯓n�����̃f�[�^�`���Ɏg�p�D�ۑ��͂���Ȃ��D
    /// </summary>
    /// <typeparam name="T">�󂯓n������f�[�^�^�D</typeparam>
    public class InMemoryDataTransitClient<T> : IDataTransmitClient<T>
    {
        private readonly static Dictionary<string, T> strage = new();

        /// <inheritdoc/>
        public T Get(string key)
        {
            var ret = strage[key];
            strage.Remove(key);
            return ret;
        }

        /// <inheritdoc/>
        public bool TryGet(string key, out T value)
        {
            return strage.Remove(key, out value);
        }

        /// <inheritdoc/>
        public void Set(string key, T value)
        {
            strage[key] = value;
        }

#pragma warning disable CS1998 // �񓯊����\�b�h�́A'await' ���Z�q���Ȃ����߁A�����I�Ɏ��s����܂�
        /// <inheritdoc/>
        public async UniTask<T> GetAsync(string key)
        {
            var ret = strage[key];
            strage.Remove(key);
            return ret;
        }

        /// <inheritdoc/>
        public async UniTask SetAsync(string key, T value)
        {
            strage[key] = value;
        }

#pragma warning restore CS1998 // �񓯊����\�b�h�́A'await' ���Z�q���Ȃ����߁A�����I�Ɏ��s����܂�
    
    }
}
