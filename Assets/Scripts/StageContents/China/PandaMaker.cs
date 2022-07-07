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
        [SerializeField] GameObject panda;
        [SerializeField] float minSize, maxSize;

        enum State
        {
            On, Off
        }
        State state;

        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating("Spawn", 1, 0.4f);
            InGameEvent.Play.Subscribe(x => { state = State.On; }).AddTo(this);
            InGameEvent.Pause.Subscribe(isPause => { state = isPause ? State.Off : State.On; }).AddTo(this);
            InGameEvent.Goal.Subscribe(x => { state = State.Off; }).AddTo(this);

            var token = this.GetCancellationTokenOnDestroy();
            PandaSpawn(token).Forget();

        }

        // Update is called once per frame
        void Update()
        {
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

                    if (state == State.On)
                {

                    Vector3 pos = new Vector3(58 * UnityEngine.Random.value, 100, 100 * UnityEngine.Random.value);
                    GameObject p = Instantiate(panda, pos, Quaternion.Euler(UnityEngine.Random.value * 360, UnityEngine.Random.value * 360, UnityEngine.Random.value * 360));
                    p.transform.localScale = Vector3.one * (UnityEngine.Random.value * (maxSize - minSize) + minSize);

                }


            }
        }



    }

}