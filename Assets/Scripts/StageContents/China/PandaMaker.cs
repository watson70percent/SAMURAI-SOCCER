using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace SamuraiSoccer.StageContents.China
{
    public class PandaMaker : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_panda; //空から降らせるパンダ
        [SerializeField]
        private float m_minSize, m_maxSize;//パンダの最大最小サイズ

        enum State
        {
            On, Off
        }
        State m_state;

        // Start is called before the first frame update
        void Start()
        {
            InGameEvent.Play.Subscribe(x => { m_state = State.On; }).AddTo(this);
            InGameEvent.Pause.Subscribe(isPause => { m_state = isPause ? State.Off : State.On; }).AddTo(this);
            InGameEvent.Goal.Subscribe(x => { m_state = State.Off; }).AddTo(this);

            var token = this.GetCancellationTokenOnDestroy();
            PandaSpawn(token).Forget();

        }

        async UniTask PandaSpawn(CancellationToken token)
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));


                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (m_state == State.On)
                {
                    Vector3 pos = new Vector3(58 * UnityEngine.Random.value, 100, 100 * UnityEngine.Random.value);
                    GameObject p = Instantiate(m_panda, pos, Quaternion.Euler(UnityEngine.Random.value * 360, UnityEngine.Random.value * 360, UnityEngine.Random.value * 360));
                    p.transform.localScale = Vector3.one * (UnityEngine.Random.value * (m_maxSize - m_minSize) + m_minSize);
                }


            }
        }



    }

}