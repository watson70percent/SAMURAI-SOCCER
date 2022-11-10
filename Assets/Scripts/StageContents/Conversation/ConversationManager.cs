using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.UI;

namespace SamuraiSoccer.StageContents.Conversation
{
    public class ConversationManager : MonoBehaviour
    {
        [SerializeField]
        private UIFade m_uiFade;

        [SerializeField]
        private List<GameObject> m_textObjects = new List<GameObject>();

        // Start is called before the first frame update
        public async void Start()
        {
           await ConversationProcess();
        }

        public async UniTask ConversationProcess()
        {
            // �����̈ړ�
            // UI�̃t�F�[�h�C��
            await m_uiFade.FadeInUI();
            // �����̕\��
            for (int i=0; i< m_textObjects.Count; i++)
            {
                m_textObjects[i].SetActive(true);
            }
            // ��b���̍Đ�
            await UniTask.Delay(3600);
            // �����̔�\��
            for (int i = 0; i < m_textObjects.Count; i++)
            {
                m_textObjects[i].SetActive(false);
            }
            // UI�̃t�F�[�h�A�E�g
            await m_uiFade.FadeOutUI();
            // �����̈ړ�
        }
    }
}
