using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class BallMoveTest : MonoBehaviour
{
    public Vector3 force;
    public Vector3 pos;
    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(2000);
        var rb = GetComponent<Rigidbody>();
        rb.AddForceAtPosition(force, pos, ForceMode.Impulse);
    }

}
