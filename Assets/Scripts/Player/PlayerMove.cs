using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;
using SamuraiSoccer.UI;

namespace SamuraiSoccer.Player
{
    public class PlayerMove : MonoBehaviour
    {

        enum State
        {
            StandBy,
            Playing,
            Idle
        }
        State m_state = State.Idle;


        public FieldManager field;
        Vector2 velocity;
        private Boundy boundy;
        public Transform flagsParent;
        [SerializeField] private Slidepad m_slidepad;

        // Start is called before the first frame update
        void Start()
        {
            SetBoundy();
            velocity = Vector3.zero;
            
            InGameEvent.Reset.Subscribe(x => { m_state = State.Idle; });
            InGameEvent.Standby.Subscribe(x => { m_state = State.StandBy; });
            InGameEvent.Pause.Subscribe(isPause => { m_state = isPause ? State.Idle : State.Playing; });
            InGameEvent.Play.Subscribe(x => { m_state = State.Playing; });


            m_slidepad.StickControllerSubject.Subscribe(
                    stickDir => { ReceiveStick(stickDir); }
                );
        }


        void ReceiveStick(Vector3 stickDir)
        {
            switch (m_state)
            {
                case State.StandBy: Move(stickDir); velocity = Vector3.zero; break;
                case State.Playing: Move(stickDir); break;
                case State.Idle: break;
            }

        }



        private void Move(Vector3 stickDir)
        {
            CalcRealVec(stickDir.x, stickDir.z);
            stickDir = (stickDir != Vector3.zero) ? stickDir : transform.forward;
            transform.rotation = Quaternion.LookRotation(stickDir);

            CheckBoundy(transform.position, ref velocity);//範囲外に出てかつ外に行こうとしているときは動かさない
            if (velocity.sqrMagnitude > 0.001)
            {
                transform.Translate(velocity.x * Time.deltaTime, 0, velocity.y * Time.deltaTime, Space.World);
            }
        }

        private void CalcRealVec(float x, float y)
        {
            var diff = new Vector2(x, y) - velocity;
            var deg = Mathf.Acos(Vector2.Dot(velocity.normalized, diff.normalized));
            var c = (deg - Mathf.PI) / Mathf.PI * 2 - 1;
            if (Mathf.Abs(x) < 0.01 && Mathf.Abs(y) < 0.01)
            {
                c = -1;
            }

            float coeff = 0;
            if (field != null)
            {
                coeff = (1 + c) / 2 * field.info.GetAccUpCoeff(transform.position) + (1 - c) / 2 * field.info.GetAccDownCoeff(transform.position);
            }
            else
            {
                coeff = 1;//よくわからんけどnull用にくっつけた
            }

            if (velocity.sqrMagnitude > 1 && diff.x * diff.x / 4 + diff.y * diff.y > coeff * coeff * 50)
            {
                Debug.LogWarning("滑ってる : " + (diff.x * diff.x / 4 + diff.y * diff.y) + ", " + coeff * coeff * 50 + "," + (x * x * 0.1f + y * y * 0.4f + 3));
                diff = coeff / (x * x * 0.1f + y * y * 0.4f + 3) * diff.normalized;
            }
            velocity += diff;
        }


        void SetBoundy()
        {
            float xmin, xmax, zmin, zmax;

            int childCount = flagsParent.childCount;
            Vector3 temp = flagsParent.GetChild(0).transform.position;
            xmin = xmax = temp.x;
            zmin = zmax = temp.z;
            for (int i = 1; i < childCount; i++)
            {
                temp = flagsParent.GetChild(i).transform.position;
                xmin = Mathf.Min(xmin, temp.x);
                xmax = Mathf.Max(xmax, temp.x);
                zmin = Mathf.Min(zmin, temp.z);
                zmax = Mathf.Max(zmax, temp.z);
            }
            boundy = new Boundy(xmin, xmax, zmin, zmax);
        }


        void CheckBoundy(Vector3 pos, ref Vector2 dir)
        {

            Boundy bound = boundy;

            if (pos.x < bound.x_min)
            {
                if (pos.x + dir.x < pos.x) { dir.x = 0; }
            }
            if (pos.x > bound.x_max)
            {
                if (pos.x + dir.x > pos.x) { dir.x = 0; }
            }
            if (pos.z < bound.z_min)
            {
                if (pos.z + dir.y < pos.z) { dir.y = 0; }
            }
            if (pos.z > bound.z_max)
            {
                if (pos.z + dir.y > pos.z) { dir.y = 0; }
            }

        }

        private struct Boundy
        {
            public float x_max, x_min, z_max, z_min;
            public Boundy(float xmin, float xmax, float zmin, float zmax)
            {
                x_max = xmax;
                x_min = xmin;
                z_max = zmax;
                z_min = zmin;
            }
        }


    }
}
