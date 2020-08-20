using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BallPassTest : MonoBehaviour
{
    public BallControler ball;
    public GameObject sender;
    public GameObject recever;
    public PassHeight height;
    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(2000);
        ball.Pass(sender.ToVector2Int(), recever.ToVector2Int(),height);
    }
}
