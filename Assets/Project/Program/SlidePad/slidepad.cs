﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class slidepad : MonoBehaviour
{

    float radius;
    float scale;
    bool isdragged;
    public GameObject joystick;
    RectTransform joyrect;
    Vector2 joystartposition;
    Vector2 slidestartposition;
    public  GameObject player;
    Rigidbody playerrig;
    public float speed;

    public Transform flagsParent;
    private Boundy boundy;
   
    GameState state = GameState.Standby;

    int fingerID;
    Vector2 velocity;

    void SwitchState(StateChangedArg a)
    {
        state = a.gameState;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().StateChange += SwitchState;

        SetBoundy();

        joyrect = joystick.GetComponent<RectTransform>();
        joystartposition = joyrect.localPosition;
        playerrig = player.GetComponent<Rigidbody>();
        velocity = Vector3.zero;
        scale = transform.localScale.x;
        radius = 50 * scale;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        switch (state)
        {
            case GameState.Standby: PlayingState(); break;
            case GameState.Playing: PlayingState(); break;
            default: break;
        }
        
        
    }



    void PlayingState()
    {
        if (isdragged == true)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = FindFinger();

                Vector2 dir = touch.position - slidestartposition;


                if (dir.magnitude > radius) { dir = dir.normalized * radius; }

                Controller(dir / radius);

                joyrect.localPosition = joystartposition + dir / scale;

            }
        }
    }



    public void DragStart(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        fingerID = pointerEventData.pointerId;
        isdragged = true;
        Touch touch = Input.GetTouch(fingerID);
        slidestartposition = touch.position;
    }




    public void DragEnd()
    {
        isdragged = false;
        joyrect.localPosition = joystartposition;
    }

    void Controller(Vector2 dir)
    {
        dir = new Vector2(dir.y, -dir.x);

        Move(dir);


    }




    void Move(Vector2 dir)
    {


        Vector3 direction = new Vector3(dir.x, 0, dir.y);

        direction = (direction != Vector3.zero) ? direction : player.transform.forward;
        playerrig.rotation = Quaternion.LookRotation(direction);

        //Vector2 force = dir* speed;
        //velocity = force;
        //Vector3 direction3d = new Vector3(playerrig.position.x + velocity.x * Time.deltaTime, playerrig.position.y, playerrig.position.z + velocity.y * Time.deltaTime);
        //playerrig.position = direction3d;

        CheckBoundy(player.transform.position, ref direction);//範囲外に出てかつ外に行こうとしているときは動かさない

        Vector3 force = direction * speed;
        if (Vector3.Dot(playerrig.velocity,force.normalized)<speed)
        {
            playerrig.AddForce(force*30);
        }
    }


    Touch FindFinger()
    {
        foreach(Touch t in Input.touches)
        {
            if (t.fingerId == fingerID) { return t; }

        }
        return new Touch();
    }

    void SetBoundy()
    {
        float xmin, xmax, zmin, zmax;

        int childCount = flagsParent.childCount;
        Vector3 temp = flagsParent.GetChild(0).transform.position;
        xmin = xmax = temp.x;
        zmin = zmax = temp.z;
        for(int i = 1; i < childCount; i++)
        {
            temp = flagsParent.GetChild(i).transform.position;
            xmin = Mathf.Min(xmin, temp.x);
            xmax = Mathf.Max(xmax, temp.x);
            zmin = Mathf.Min(zmin, temp.z);
            zmax = Mathf.Max(zmax, temp.z);
        }
        boundy = new Boundy(xmin, xmax, zmin, zmax);
    }
    

    void CheckBoundy(Vector3 pos,ref Vector3 dir)
    {

        Boundy bound = boundy;

        if(pos.x < bound.x_min)
        {
            if (pos.x + dir.x < pos.x) { dir.x=0; }
        }
        if(pos.x > bound.x_max)
        {
            if (pos.x + dir.x > pos.x) { dir.x = 0; }
        }
        if(pos.z < bound.z_min)
        {
            if (pos.z + dir.z < pos.z) { dir.z = 0; }
        }
        if(pos.z > bound.z_max)
        {
            if (pos.z + dir.z > pos.z) { dir.z = 0; }
        }

    }

    private struct Boundy
    {
        public float x_max, x_min, z_max, z_min;
        public Boundy(float xmin,float xmax,float zmin, float zmax)
        {
            x_max = xmax;
            x_min = xmin;
            z_max = zmax;
            z_min = zmin;
        }
    }
}
