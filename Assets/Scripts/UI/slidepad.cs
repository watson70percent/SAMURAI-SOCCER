using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using SamuraiSoccer.Event;
using System;

namespace SamuraiSoccer.UI
{
    public class Slidepad : MonoBehaviour
    {

        private Subject<Vector3> m_stickControllerSubject = new Subject<Vector3>();

        public IObservable<Vector3> StickControllerSubject
        {
            get { return m_stickControllerSubject; }
        }

        float radius;
        float scale;
        bool isdragged;
        public GameObject joystick;
        RectTransform joyrect;
        Vector2 joystartposition;
        Vector2 slidestartposition;
        public GameObject player;
        Rigidbody playerrig;
        public float speed;

        
        



        int fingerID;
        

        public FieldManager field;



        // Start is called before the first frame update
        void Start()
        {
            

            

            joyrect = joystick.GetComponent<RectTransform>();
            joystartposition = joyrect.localPosition;
            playerrig = player.GetComponent<Rigidbody>();
            
            scale = transform.localScale.x;
            radius = 50 * scale;
            Debug.Log("radius : " + radius);
        }

        // Update is called once per frame
        void LateUpdate()
        {

            PlayingState();

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

                    Controller(5 / radius * dir);

                    joyrect.localPosition = joystartposition + dir / scale;

                }
            }
            else
            {
                Controller(Vector2.zero);
            }
        }



        public void DragStart(BaseEventData baseEventData)
        {
            PointerEventData pointerEventData = baseEventData as PointerEventData;
            fingerID = pointerEventData.pointerId;
            isdragged = true;
            try
            {
                Touch touch = Input.GetTouch(fingerID);
                slidestartposition = touch.position;
            }
            catch
            {

            }
        }




        public void DragEnd()
        {
            isdragged = false;
            joyrect.localPosition = joystartposition;
        }

        void Controller(Vector2 dir)
        {
            dir = new Vector2(2.0f * dir.y, -dir.x);

            m_stickControllerSubject.OnNext(new Vector3(dir.x, 0, dir.y));
 
        }

        


        Touch FindFinger()
        {
            foreach (Touch t in Input.touches)
            {
                if (t.fingerId == fingerID) { return t; }

            }
            return new Touch();
        }

        
    }

}

