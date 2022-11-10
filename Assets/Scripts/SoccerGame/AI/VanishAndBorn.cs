using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame.AI
{
    public class VanishAndBorn : MonoBehaviour
    {
        Vector3 center = new Vector3(30, 0, 50);
        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating("Check", 1, 1);
        }

        void Check()
        {
            var dis = (transform.position - center).magnitude;
            if (dis > 100)
            {
                var easyCpuManager = GameObject.Find("GameManager").GetComponent<EasyCPUManager>();
                if (easyCpuManager != null)
                {
                    easyCpuManager.Kill(this.gameObject);
                }
            }
        }
    }
}

