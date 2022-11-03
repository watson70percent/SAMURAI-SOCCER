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
            // 巻物の移動
            // UIのフェードイン
            await m_uiFade.FadeInUI();
            // 文字の表示
            for (int i=0; i< m_textObjects.Count; i++)
            {
                m_textObjects[i].SetActive(true);
            }
            // 会話文の再生
            await UniTask.Delay(3600);
            // 文字の非表示
            for (int i = 0; i < m_textObjects.Count; i++)
            {
                m_textObjects[i].SetActive(false);
            }
            // UIのフェードアウト
            await m_uiFade.FadeOutUI();
            // 巻物の移動
        }
    }
}
