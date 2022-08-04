using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using SamuraiSoccer.Event;

namespace Name
{
    public class AvoidCarScript : MonoBehaviour
    {
        void Start()
        {
            this.OnTriggerStayAsObservable().Where(x => x.gameObject.tag == "Untagged")
            .Subscribe(_ =>
            {
                Rigidbody rb = _.transform.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 v3 = rb.velocity;
                    v3.y = 10;
                    rb.velocity = v3;
                }
            }).AddTo(this);
        }
    }
}