using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiseStatue : MonoBehaviour
{
    private void Start()
    {
        StartCoroutineForPlayingState.AddTaskIEnumrator(RiseStatueCoroutine());
    }

    IEnumerator RiseStatueCoroutine()
    {
        while (gameObject.transform.position.y < 0.95f)
        {
            yield return 0.1f;
            gameObject.transform.position += new Vector3(0f, 1.0f, 0f);
        }
        while (gameObject.transform.eulerAngles.z < 90)
        {
            yield return 0.01f;
            gameObject.transform.eulerAngles += new Vector3(0f, 0f, 1.0f);

        }
        yield return 3.0f;
        Destroy(gameObject);
    }
}
