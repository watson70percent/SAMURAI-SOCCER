using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberManager : MonoBehaviour
{
    public EasyCPUManager cpuManager;
    public Text enemyNumber;

    // Update is called once per frame
    void Update()
    {
      enemyNumber.text="Team="+cpuManager.TeamMemberCount  + ", Opp=" + cpuManager.OpponentMemberCount;   
    }
}
