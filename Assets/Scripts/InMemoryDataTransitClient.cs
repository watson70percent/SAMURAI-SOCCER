using Cysharp.Threading.Tasks;

using System.Collections.Generic;

namespace SamuraiSoccer
{
    /// <summary>
    /// シーン間のデータ受け渡し等のデータ伝送に使用．保存はされない．
    /// </summary>
    /// <typeparam name="T">受け渡しするデータ型．</typeparam>
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

#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
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

#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
    
    }
}
