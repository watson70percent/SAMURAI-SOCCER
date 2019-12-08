using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slidepad : MonoBehaviour
{

    public float radius;
    bool isTapped;
    public GameObject joystick;
    RectTransform joyrect;
    Vector2 joystartposition;
    Vector2 slidestartposition;
    public  GameObject player;
    Rigidbody playerrig;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        joyrect = joystick.GetComponent<RectTransform>();
        joystartposition = joyrect.localPosition;
        playerrig = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }




    public void Drag()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //print(touch.position.x + ":" + touch.position.y);


            if (isTapped == false)
            {
                isTapped = true;
                slidestartposition = touch.position;

            }


            Vector2 dir = touch.position - slidestartposition;

            if (dir.magnitude > radius) { dir = dir.normalized * radius; }


            Controller(dir);

            joyrect.localPosition = joystartposition + dir;

        }




    }


    public void DragEnd()
    {
        isTapped = false;
        joyrect.localPosition = joystartposition;
    }

    void Controller(Vector2 dir)
    {
        Vector2 velocity= dir/radius * speed ;
        playerrig.position = new Vector3(playerrig.position.x + velocity.x * Time.deltaTime, playerrig.position.y, playerrig.position.z + velocity.y * Time.deltaTime);

    }

}
