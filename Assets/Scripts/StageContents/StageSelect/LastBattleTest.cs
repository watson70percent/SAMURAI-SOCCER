using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class LastBattleTest : MonoBehaviour
    {
        [SerializeField]
        StageIcon stageIcon;
        // Start is called before the first frame update
        void Start()
        {
            stageIcon.State = StageState.Playable;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
