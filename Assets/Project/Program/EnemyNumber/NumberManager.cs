using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberManager : MonoBehaviour
{
    public EasyCPUManager cpuManager;
    public Text enemyNumber,teamNumber;


    // Update is called once per frame
    void Update()
    {
      enemyNumber.text= "✕"+ cpuManager.OpponentMemberCount;
        teamNumber.text = "✕" + cpuManager.TeamMemberCount;
    }
}
