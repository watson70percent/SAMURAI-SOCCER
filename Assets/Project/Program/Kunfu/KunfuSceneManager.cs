using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunfuSceneManager : MonoBehaviour
{
    [SerializeField] GameObject referee, prefab;
    GameObject kunfu;
    private void Start()
    {
        kunfu = Instantiate(prefab);
        kunfu.transform.parent = referee.transform.parent;
        kunfu.transform.localPosition = referee.transform.localPosition;

        var parent= referee.transform.parent;

        Destroy(parent.GetComponent<RefereeMove>());
        parent.gameObject.AddComponent<KunfuReferee>();


        Destroy(referee);
    }

    private void Update()
    {
        float height = kunfu.transform.position.y;
    }

}