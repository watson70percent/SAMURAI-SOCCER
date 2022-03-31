using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiangshiSceneManager : MonoBehaviour
{
    [SerializeField] GameObject referee, prefab;
    GameObject jianshi;
    [SerializeField] RefereeMove refereeMove;
    [SerializeField] RefereeArea refereeArea;
    public int areaSize,areaAngle;
    private void Awake()
    {
        jianshi = Instantiate(prefab);
        jianshi.transform.parent = referee.transform.parent;
        jianshi.transform.localPosition = referee.transform.localPosition;
        Destroy(referee);

        refereeMove.anicon = jianshi.GetComponent<Animator>();
          var stagePrefabManager=  GameObject.Find("DefaultStage").GetComponent<StagePrefabManager>();
        areaSize = stagePrefabManager.refereeAreaSize;
        areaAngle = stagePrefabManager.refereeMaxAng;
    }

    private void Update()
    {
        float height = jianshi.transform.position.y;
        refereeArea.SerAreaSize( areaSize + height*20);

        var angle = areaAngle + height * 6;
        angle = Mathf.Min(angle, 180);
        refereeArea.SerMaxAngle(angle);
        refereeArea.MeshMaker();
    }

}
