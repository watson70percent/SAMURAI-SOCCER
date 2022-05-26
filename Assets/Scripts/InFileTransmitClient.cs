using Cysharp.Threading.Tasks;

using System.IO;
using System;
using UnityEngine;

namespace SamuraiSoccer
{
    /// <summary>
    /// �Z�[�u�f�[�^���C�ۑ�����f�[�^�̎󂯓n�����s���D
    /// </summary>
    /// <typeparam name="T">�󂯓n������f�[�^�̌^�D</typeparam>
    public class InFileTransmitClient<T> : IDataTransmitClient<T>
    {
        private readonly string folderPath = Application.dataPath;
        private readonly string typeName = typeof(T).Name;

        private string GetSavePath(string key)
        {
            return Path.Combine(folderPath, typeName + ".json");
        }

        /// <inheritdoc/>
        public T Get(string key)
        {
            var savePath = GetSavePath(key);
            if (!File.Exists(savePath))
            {
                throw new ArgumentException("�L�[ : " + key + "�͑��݂��܂���D");
            }
            var content = File.ReadAllText(savePath);
            return JsonUtility.FromJson<T>(content);
        }

        /// <inheritdoc/>
        public bool TryGet(string key, out T value)
        {
            var savePath = GetSavePath(key);
            if (!File.Exists(savePath))
            {
                value = default;
                return false;
            }
            var content = File.ReadAllText(savePath);
            value = JsonUtility.FromJson<T>(content);
            return true;
        }

        /// <inheritdoc/>
        public void Set(string key, T value)
        {
            var savePath = GetSavePath(key);
            var content = JsonUtility.ToJson(value);
            File.WriteAllText(savePath, content);
        }

        /// <inheritdoc/>
        public async UniTask<T> GetAsync(string key)
        {
            var savePath = GetSavePath(key);
            if (!File.Exists(savePath))
            {
                throw new ArgumentException("�L�[ : " + key + "�͑��݂��܂���D");
            }
            var content = await File.ReadAllTextAsync(savePath);
            return JsonUtility.FromJson<T>(content);
        }

        /// <inheritdoc/>
        public async UniTask SetAsync(string key, T value)
        {
            var savePath = GetSavePath(key);
            var content = JsonUtility.ToJson(value);
            await File.WriteAllTextAsync(savePath, content);
        }
    }
}
