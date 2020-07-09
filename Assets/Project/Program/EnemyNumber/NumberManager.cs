using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberManager : MonoBehaviour
{
    public EasyCPUManager cpuManager;
    public Text enemyNumber;
    public Text Point;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      enemyNumber.text="Team="+cpuManager.TeamMemberCount  + ", Opp=" + cpuManager.OpponentMemberCount;   
    }
}
