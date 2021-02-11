using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiangshiSceneManager : MonoBehaviour
{
    [SerializeField] GameObject referee, prefab;
    GameObject jianshi;
    [SerializeField] RefereeMove refereeMove;
    [SerializeField] RefereeArea refereeArea;
    public int areaSize;
    private void Awake()
    {
        jianshi = Instantiate(prefab);
        jianshi.transform.parent = referee.transform.parent;
        jianshi.transform.localPosition = referee.transform.localPosition;
        Destroy(referee);

        refereeMove.anicon = jianshi.GetComponent<Animator>();
        areaSize = GameObject.Find("DefaultStage").GetComponent<StagePrefabManager>().refereeAreaSize;
    }

    private void Update()
    {
        float height = jianshi.transform.position.y;
        refereeArea.areaSize = areaSize + height*20;
        refereeArea.MeshMaker();
    }

}
