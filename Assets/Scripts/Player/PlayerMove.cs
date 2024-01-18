using System;
using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;
using SamuraiSoccer.SoccerGame;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.Player
{
    public class PlayerMove : MonoBehaviour
    {

        enum State
        {
            StandBy,
            Playing,
            ChargeAttack,
            Idle
        }
        State m_state = State.Idle;


        public FieldManager field;
        Vector2 velocity;
        private Rect boundy;
        public Transform flagsParent;
        [SerializeField]
        private float speed = 1.0f;
        [SerializeField]
        GameObject slashTrail;
        Rigidbody rigidbody;
        [SerializeField]
        GameObject slashCollider;
        [SerializeField]
        private float speedUpCoeff = 1.5f;
        [SerializeField]
        private float wakeUpT = 1.6f;
        [SerializeField]
        private float alpha = 7.0f;

        private float currentSpeed = 1.0f;
        private DateTime lastSlash;
        private bool m_isSlipping = false;


        // Start is called before the first frame update
        void Start()
        {
            boundy = SetBoundy();
            rigidbody = GetComponent<Rigidbody>();
            SetBoundy();
            velocity = Vector3.zero;

            InGameEvent.Reset.Subscribe(x => { m_state = State.Idle; m_isSlipping = false; }).AddTo(this);
            InGameEvent.Standby.Subscribe(x => { m_state = State.StandBy; m_isSlipping = false; }).AddTo(this);
            InGameEvent.Pause.Subscribe(isPause => { m_state = isPause ? State.Idle : State.Playing; if (!isPause) { UpdateSlashTime(); } }).AddTo(this);
            InGameEvent.Play.Subscribe(x => { m_state = State.Playing; m_isSlipping = false; UpdateSlashTime(); }).AddTo(this);
            InGameEvent.Goal.Subscribe(x => { PlayerEvent.SetIsEnableChargeAtack(false); m_isSlipping = false; }).AddTo(this);
            InGameEvent.Finish.Subscribe(x => { m_state = State.Idle; }).AddTo(this);


            PlayerEvent.StickInput.Subscribe(
                    stickDir => { ReceiveStick(stickDir); }
                ).AddTo(this);
            PlayerEvent.IsInChargeAttack.Subscribe(
                    x =>
                    {
                        if (x && m_state == State.Playing) { ChargeAttack(); }
                        else { PlayerEvent.SetIsInChargeAtack(false); }
                    }
                ).AddTo(this);
            PlayerEvent.Attack.Subscribe(_ => UpdateSlashTime());
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
            UpdateCurrentSpeed();
            var nextVelocity = CalcRealVec(stickDir.x, stickDir.z, velocity);
            stickDir = (stickDir != Vector3.zero) ? stickDir : transform.forward;
            transform.rotation = Quaternion.LookRotation(stickDir);

            nextVelocity = CheckBoundy(transform.position, nextVelocity);//範囲外に出てかつ外に行こうとしているときは動かさない
            velocity = nextVelocity;
            if (velocity.sqrMagnitude > 0.001)
            {
                transform.Translate(velocity.x * Time.deltaTime * currentSpeed, 0, velocity.y * Time.deltaTime * currentSpeed, Space.World);
            }
            velocity *= 0.99f;
        }

        private Vector2 CalcRealVec(float x, float y, Vector2 currentVelocity)
        {
            var diff = new Vector2(x, y) - currentVelocity;
            var deg = Mathf.Acos(Vector2.Dot(currentVelocity.normalized, diff.normalized));
            var c = (deg - Mathf.PI) / Mathf.PI * 2 - 1;
            if (Mathf.Abs(x) < 0.01 && Mathf.Abs(y) < 0.01)
            {
                c = -1;
            }

            float coeff;
            if (field != null)
            {
                coeff = (1 + c) / 2 * field.info.GetAccUpCoeff(transform.position) + (1 - c) / 2 * field.info.GetAccDownCoeff(transform.position);
            }
            else
            {
                coeff = 1;//よくわからんけどnull用にくっつけた
            }

            if (currentVelocity.sqrMagnitude > 1 && diff.x * diff.x / 4 + diff.y * diff.y > coeff * coeff * 50)
            {
                m_isSlipping = true;
                Debug.LogWarning("滑ってる : " + (diff.x * diff.x / 4 + diff.y * diff.y) + ", " + coeff * coeff * 50 + "," + (x * x * 0.1f + y * y * 0.4f + 3));
                diff = coeff / (x * x * 0.1f + y * y * 0.4f + 3) * diff.normalized;
            }
            else
            {
                m_isSlipping = false;
            }
            return currentVelocity + diff;
        }


        /// <summary>
        /// コーナーフラッグの位置を境界に設定
        /// </summary>
        private Rect SetBoundy()
        {
            var flagPositions = flagsParent.GetComponentsInChildren<Transform>().Where(obj => obj.tag == "Flag").Select(t => t.position).ToArray();
            var xPoints = flagPositions.Select(p => p.x).ToArray();
            var zPoints = flagPositions.Select(p => p.z).ToArray();
            var r = new Rect
            {
                xMin = xPoints.Min(),
                yMin = zPoints.Min(),
                xMax = xPoints.Max(),
                yMax = zPoints.Max()
            };
            return r;
        }


        private Vector2 CheckBoundy(Vector3 pos, Vector2 dir)
        {
            if (pos.x < boundy.xMin && pos.x + dir.x < pos.x)
            {
                dir.x = 0;
            }
            if (pos.x > boundy.xMax && pos.x + dir.x > pos.x)
            {
                dir.x = 0;
            }
            if (pos.z < boundy.yMin && pos.z + dir.y < pos.z)
            {
                dir.y = 0;
            }
            if (pos.z > boundy.yMax && pos.z + dir.y > pos.z)
            {
                dir.y = 0;
            }
            return dir;
        }

        private void UpdateCurrentSpeed()
        {
            var elapsed = DateTime.Now - lastSlash;
            currentSpeed = speed * (1 + (speedUpCoeff - 1) / (1 + Mathf.Exp(alpha * (float)((wakeUpT - 2 * elapsed.TotalSeconds) / wakeUpT))));
            if (m_state != State.Playing) currentSpeed = speed;
        }

        private void UpdateSlashTime()
        {
            lastSlash = DateTime.Now;
        }

        /// <summary>
        /// ため攻撃
        /// </summary>
        async UniTask ChargeAttack()
        {
            SoundMaster.Instance.PlaySE(13);
            slashTrail.SetActive(true); //斬撃の残像を表示
            Vector3 step = m_isSlipping ? new Vector3(velocity.x, 0, velocity.y).normalized * 0.1f : PlayerEvent.StickDir.Value.normalized;
            if (step == Vector3.zero)
            {
                step = transform.forward;
            }
            Vector3 vec = step * 6;

            for (int i = 0; i < Mathf.Floor(vec.magnitude / step.magnitude); i++) //細かく進んでslashColliderを撒いていく
            {

                Vector3 tempNewPos = transform.position + vec;
                bool isSlash = false;
                if (tempNewPos.x > FieldBoundary.XMin && tempNewPos.x < FieldBoundary.XMax)//壁にぶつからなければ移動する
                {
                    isSlash = true;
                    transform.position = new Vector3(transform.position.x + vec.x, transform.position.y, transform.position.z);
                }
                if (tempNewPos.z > FieldBoundary.ZMin && tempNewPos.z < FieldBoundary.ZMax)
                {
                    isSlash = true;
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + vec.z);
                }
                if (isSlash)
                {
                    Instantiate(slashCollider, transform.position + Vector3.up, transform.rotation);
                }
                await UniTask.Yield();
            }

            PlayerEvent.FaulCheckOnNext(); //斬撃の最後に審判のファールチェック
            PlayerEvent.SetIsInChargeAtack(false);
            slashTrail.SetActive(false);
            UpdateSlashTime();
        }
    }
}
