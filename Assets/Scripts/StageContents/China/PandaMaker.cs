using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using UniRx;

namespace SamuraiSoccer.StageContents.China
{

    public class PandaMaker : MonoBehaviour
    {

        enum State
        {
            Working, Stop
        }

        [SerializeField] GameObject panda;
        [SerializeField] float minSize, maxSize;
        private State state =State.Stop;

        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating("Spawn", 1, 0.4f);
            InGameEvent.Play.Subscribe(x => { state = State.Working; });
            InGameEvent.Standby.Subscribe(x => { state = State.Stop; });
            InGameEvent.Reset.Subscribe(x => { state = State.Stop; });
            InGameEvent.Finish.Subscribe(x => { state = State.Stop; });
            InGameEvent.Pause.Subscribe(isPause=> { state = isPause ? State.Stop : State.Working; });
        }

        // Update is called once per frame
        void Update()
        {
        }


        void Spawn()
        {
            switch (state)
            {
                case State.Working:
                    {
                        Vector3 pos = new Vector3(58 * Random.value, 100, 100 * Random.value);
                        GameObject p = Instantiate(panda, pos, Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360));
                        p.transform.localScale = Vector3.one * (Random.value * (maxSize - minSize) + minSize);
                        break;
                    }
                default: break;


            }
        }
    }

}