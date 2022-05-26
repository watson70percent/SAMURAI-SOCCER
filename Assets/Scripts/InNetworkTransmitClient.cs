using Cysharp.Threading.Tasks;

using System.Text;
using UnityEngine;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SamuraiSoccer
{
    /// <summary>
    /// サーバーとデータを送受信する．
    /// </summary>
    /// <typeparam name="T">受け渡しするデータの型．</typeparam>
    public class InNetworkTransmitClient<T> : IDataTransmitClient<T>
    {
        private readonly HttpClient client = new();

        public InNetworkTransmitClient() { }

        /// <summary>
        /// デフォルトのヘッダを指定する際のコンストラクタ．
        /// </summary>
        /// <param name="defaultHeader">デフォルトのヘッダ．</param>
        public InNetworkTransmitClient(HttpRequestHeaders defaultHeader){
            foreach (var header in defaultHeader) { 
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        /// <summary>
        /// 使用できません．
        /// </summary>
        public T Get(string key)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 使用できません．
        /// </summary>
        public bool TryGet(string key, out T value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 使用できません．
        /// </summary>
        public void Set(string key, T value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public async UniTask<T> GetAsync(string key)
        {
            var response = await client.GetAsync(key);
            var content = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<T>(content);
        }

        /// <inheritdoc/>
        public async UniTask SetAsync(string key, T value)
        {
            var content = JsonUtility.ToJson(value);
            var body = new StringContent(content, Encoding.UTF8, "application/json");
            await client.PostAsync(key, body);
        }

    }
}
