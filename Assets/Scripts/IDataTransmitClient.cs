using Cysharp.Threading.Tasks;

namespace SamuraiSoccer
{
    /// <summary>
    /// データを受け渡し・送受信するためのインターフェース．
    /// </summary>
    /// <typeparam name="T">やり取りするデータの型名．</typeparam>
    public interface IDataTransmitClient<T>
    {
        /// <summary>
        /// データを受け取り・受信する．
        /// </summary>
        /// <param name="key">渡すときと共通のキーまたはURL．</param>
        /// <returns>受け取ったデータ．</returns>
        public T Get(string key);

        /// <summary>
        /// データを受け取り・受信する．
        /// </summary>
        /// <param name="key">渡すときと共通のキーまたはURL．</param>
        /// <param name="value">受け取ったデータ．</param>
        /// <returns>データの受け取り・受信に成功したか．</returns>
        public bool TryGet(string key, out T value);

        /// <summary>
        /// データを渡す・送信する．
        /// </summary>
        /// <param name="key">受け取るときと共通のキーまたはURL．</param>
        /// <param name="value">送るデータ．</param>
        public void Set(string key, T value);

        /// <summary>
        /// データを受け取り・受信する．
        /// </summary>
        /// <param name="key">渡すときと共通のキーまたはURL．</param>
        /// <returns>受け取ったデータ．</returns>
        public UniTask<T> GetAsync(string key);

        /// <summary>
        /// データを渡す・送信する．
        /// </summary>
        /// <param name="key">受け取るときと共通のキーまたはURL．</param>
        /// <param name="value">送るデータ．</param>
        public UniTask SetAsync(string key, T value);
    }
}
