using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents
{
    public class MidareAttack : MonoBehaviour
    {
        public GameObject slash;
        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating("Attack", 1, 1);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void Attack()
        {
            Instantiate(slash, transform.position, transform.rotation);
        }
    }
}
