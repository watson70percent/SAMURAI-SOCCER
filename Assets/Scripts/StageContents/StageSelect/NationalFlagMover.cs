using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NationalFlagMover : MonoBehaviour
{
    public Transform ankerPoint;
    public Transform cameraPoint;
    public float distance;

    // Update is called once per frame
    void Update()
    {
        var direction = cameraPoint.position - ankerPoint.position;
        if (direction.magnitude >= distance * 1.7)
        {
            gameObject.transform.position = direction.normalized * distance + ankerPoint.position;
        }
    }
}
