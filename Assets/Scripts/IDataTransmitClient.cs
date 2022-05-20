using Cysharp.Threading.Tasks;

namespace SamuraiSoccer
{
    /// <summary>
    /// �f�[�^���󂯓n���E����M���邽�߂̃C���^�[�t�F�[�X�D
    /// </summary>
    /// <typeparam name="T">����肷��f�[�^�̌^���D</typeparam>
    public interface IDataTransmitClient<T>
    {
        /// <summary>
        /// �f�[�^���󂯎��E��M����D
        /// </summary>
        /// <param name="key">�n���Ƃ��Ƌ��ʂ̃L�[�܂���URL�D</param>
        /// <returns>�󂯎�����f�[�^�D</returns>
        public T Get(string key);

        /// <summary>
        /// �f�[�^���󂯎��E��M����D
        /// </summary>
        /// <param name="key">�n���Ƃ��Ƌ��ʂ̃L�[�܂���URL�D</param>
        /// <param name="value">�󂯎�����f�[�^�D</param>
        /// <returns>�f�[�^�̎󂯎��E��M�ɐ����������D</returns>
        public bool TryGet(string key, out T value);

        /// <summary>
        /// �f�[�^��n���E���M����D
        /// </summary>
        /// <param name="key">�󂯎��Ƃ��Ƌ��ʂ̃L�[�܂���URL�D</param>
        /// <param name="value">����f�[�^�D</param>
        public void Set(string key, T value);

        /// <summary>
        /// �f�[�^���󂯎��E��M����D
        /// </summary>
        /// <param name="key">�n���Ƃ��Ƌ��ʂ̃L�[�܂���URL�D</param>
        /// <returns>�󂯎�����f�[�^�D</returns>
        public UniTask<T> GetAsync(string key);

        /// <summary>
        /// �f�[�^��n���E���M����D
        /// </summary>
        /// <param name="key">�󂯎��Ƃ��Ƌ��ʂ̃L�[�܂���URL�D</param>
        /// <param name="value">����f�[�^�D</param>
        public UniTask SetAsync(string key, T value);
    }
}
