using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BallShootTest : MonoBehaviour
{
    public BallControler ball;
    public GameObject sender;
    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(2000);
        ball.Shoot(sender);
    }

    
}
