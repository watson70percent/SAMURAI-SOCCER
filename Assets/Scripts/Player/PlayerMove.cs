using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;
using SamuraiSoccer.UI;
using SamuraiSoccer.SoccerGame;

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
        [SerializeField]
        public float speed=1.0f;
        private float sensitiveRotate = 2.0f;

        // Start is called before the first frame update
        void Start()
        {
            SetBoundy();
            velocity = Vector3.zero;

            InGameEvent.Reset.Subscribe(x => { m_state = State.Idle; }).AddTo(this);
            InGameEvent.Standby.Subscribe(x => { m_state = State.StandBy; }).AddTo(this);
            InGameEvent.Pause.Subscribe(isPause => { m_state = isPause ? State.Idle : State.Playing; }).AddTo(this);
            InGameEvent.Play.Subscribe(x => { m_state = State.Playing; }).AddTo(this);
            InGameEvent.Finish.Subscribe(x => { m_state = State.Idle; }).AddTo(this);


            PlayerEvent.StickInput.Subscribe(
                    stickDir => { ReceiveStick(stickDir); }
                ).AddTo(this);
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
            var stkx = (stickDir.x > 0 ? stickDir.x : stickDir.x / 2) * transform.forward;
            var stkz = -stickDir.z * transform.right;
            CalcRealVec(stkx.x + stkz.x, stkx.z + stkz.z);
            stickDir = (stickDir != Vector3.zero) ? stickDir : transform.forward;
            //transform.rotation = Quaternion.LookRotation(stickDir);
            float rotateX = Input.GetAxis("Mouse X") * sensitiveRotate;
	        //float rotateY = Input.GetAxis("Mouse Y") * sensitiveRotate;
	        transform.Rotate(0.0f, rotateX, 0.0f);

            CheckBoundy(transform.position, ref velocity);//�͈͊O�ɏo�Ă��O�ɍs�����Ƃ��Ă���Ƃ��͓������Ȃ�
            if (velocity.sqrMagnitude > 0.001)
            {
                transform.Translate(new Vector3(velocity.x * Time.deltaTime * speed, 0, velocity.y * Time.deltaTime * speed), Space.World);
                //transform.Translate(velocity.x * Time.deltaTime*speed, 0, velocity.y * Time.deltaTime * speed, Space.World);
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
            if (field != null && field.info != null)
            {
                coeff = (1 + c) / 2 * field.info.GetAccUpCoeff(transform.position) + (1 - c) / 2 * field.info.GetAccDownCoeff(transform.position);
            }
            else
            {
                coeff = 1;//�悭�킩��񂯂�null�p�ɂ�������
            }

            if (velocity.sqrMagnitude > 1 && diff.x * diff.x / 4 + diff.y * diff.y > coeff * coeff * 50)
            {
                Debug.LogWarning("�����Ă� : " + (diff.x * diff.x / 4 + diff.y * diff.y) + ", " + coeff * coeff * 50 + "," + (x * x * 0.1f + y * y * 0.4f + 3));
                diff = coeff / (x * x * 0.1f + y * y * 0.4f + 3) * diff.normalized;
            }
            velocity += diff;
        }


        /// <summary>
        /// �R�[�i�[�t���b�O�̈ʒu�����E�ɐݒ�
        /// </summary>
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
