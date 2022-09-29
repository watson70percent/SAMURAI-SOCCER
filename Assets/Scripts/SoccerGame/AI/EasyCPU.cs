using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using System.Runtime.Serialization;

using UniRx;

using SamuraiSoccer.Event;

namespace SamuraiSoccer.SoccerGame.AI
{
    /// <summary>
    /// 簡単なCPUの動作をさせるクラス．
    /// </summary>
    public class EasyCPU : MonoBehaviour
    {
        public Transform ball;
        public EasyCPUManager manager;
        public PersonalStatus status;
        public FieldManager field;
        public Rigidbody rb;
        private bool m_isPause = true;
        private Vector2 before_velocity = Vector2.zero;
        private LinkedList<Vector2> rot_chain = new LinkedList<Vector2>();


        /// <summary>
        /// 吹っ飛びやすさを再設定
        /// </summary>
        public void SetMass()
        {
            rb.mass = Mathf.Pow(2, status.hp - 1);
        }

        /// <summary>
        /// 攻撃されたときに呼ぶ。hpが減り、吹っ飛びやすくなる。
        /// </summary>
        public void Attacked()
        {
            status.hp--;
            SetMass();
        }

        /// <summary>
        /// ポーズ状態か設定する。初期化のとき呼ぶ。
        /// </summary>
        /// <param name="isPause">ポーズか。</param>
        public void SetPause(bool isPause)
        {
            m_isPause = isPause;
        }

        private void Start()
        {
            InGameEvent.Pause.Subscribe(Pause).AddTo(this);
            InGameEvent.Play.Subscribe(Play).AddTo(this);
            InGameEvent.Standby.Subscribe(Standby).AddTo(this);
        }

        private void Pause(bool isPause)
        {
            m_isPause = isPause;
        }

        private void Play(Unit _)
        {
            m_isPause = false;
        }

        private void Standby(Unit _)
        {
            m_isPause = true;
        }

        public void SlowDown()
        {
            m_isPause = true;
            StartCoroutine(SlowMove());
        }

        private IEnumerator SlowMove()
        {
            while (true)
            {
                yield return null;
                gameObject.transform.Translate(before_velocity.x * 0.2f * Time.deltaTime, 0, before_velocity.y * 0.2f * Time.deltaTime, Space.World);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (m_isPause)
            {
                return;
            }

            if ((ball.position - transform.position).sqrMagnitude < 4)
            {
                try
                {
                    if (status.ally)
                    {
                        var temp = manager.team.Where(value => field.AdaptInversePosition(value.transform.position).z > field.AdaptInversePosition(transform.position).z);
                        var to = Random.Range(0, temp.Count() + 1);
                        if (to == temp.Count())
                        {
                            BallAction.CommandStreamOnNext(new ShootCommand(gameObject.transform, status));
                        }
                        else
                        {
                            BallAction.CommandStreamOnNext(new PassCommand(gameObject.ToVector2Int(), temp.Skip(to).First().ToVector2Int(), (PassHeight)Random.Range(0, 3), status));
                        }
                    }
                    else
                    {
                        var temp = manager.opp.Where(value => field.AdaptInversePosition(value.transform.position).z < field.AdaptInversePosition(transform.position).z);
                        var to = Random.Range(0, temp.Count() + 1);
                        if (to == temp.Count())
                        {
                            BallAction.CommandStreamOnNext(new ShootCommand(gameObject.transform, status));
                        }
                        else
                        {
                            BallAction.CommandStreamOnNext(new PassCommand(gameObject.ToVector2Int(), temp.Skip(to).First().ToVector2Int(), (PassHeight)Random.Range(0, 3), status));
                        }

                    }
                }
                catch (Exception) { }
            }

            AllMove(ball.ToVector2());
        }

        private void AllMove(Vector2 dest)
        {
            Vector2 vec = dest - gameObject.ToVector2Int();
            vec = vec.normalized * 10;

            try
            {
                if (status.ally)
                {
                    if (vec.sqrMagnitude > status.seelen * status.seelen * 100 && field.AdaptInversePosition(ball.position).z > field.AdaptInversePosition(gameObject.transform.position).z + 10)
                    {
                        vec = Vector2.zero;
                    }

                    if (gameObject != manager.near_team)
                    {

                        manager.team.ForEach(value =>
                        {
                            if (value != gameObject)
                            {
                                Vector2 t = gameObject.ToVector2Int() - value.ToVector2Int();
                                float tmp = status.seelen - t.magnitude > 0 ? status.seelen - t.magnitude : 0;
                                vec += 2 * tmp * t.normalized;
                            }
                        });
                    }
                }
                else
                {
                    if (vec.sqrMagnitude > 40000 && field.AdaptInversePosition(ball.position).z < field.AdaptInversePosition(gameObject.transform.position).z - 10)
                    {
                        vec = Vector2.zero;
                    }

                    if (gameObject != manager.near_opp)
                    {
                        manager.opp.ForEach(value =>
                        {
                            if (value != gameObject)
                            {
                                Vector2 t = gameObject.ToVector2Int() - value.ToVector2Int();
                                float tmp = status.seelen - t.magnitude > 0 ? status.seelen - t.magnitude : 0;
                                vec += 2 * tmp * t.normalized;
                            }
                        });

                    }
                }

            }
            catch (Exception)
            {


            }
            float dis = vec.magnitude;

            if (dis > 9)
            {
                vec = vec.normalized * status.fast;
            }
            else
            {
                vec = Mathf.Log10(dis + 1) * status.fast * vec.normalized;
            }

            if (FieldNumber.no == 1)
            {
                rot_chain.AddLast(vec);
                if (rot_chain.Count > 30)
                {
                    rot_chain.RemoveFirst();
                }
            }
            vec = CalcNextPoint(vec);
            if (FieldNumber.no != 1)
            {
                rot_chain.AddLast(vec);
                if (rot_chain.Count > 30)
                {
                    rot_chain.RemoveFirst();
                }
            }
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, CalcRotAverage()));
            gameObject.transform.Translate(vec.x, 0, vec.y, Space.World);
        }

        private Vector2 CalcNextPoint(Vector2 vec)
        {
            var realVec = CalcRealVec(vec);
            var move = realVec * Time.deltaTime;
            var next = field.AdaptInversePosition(new Vector3(move.x + transform.position.x, 0, move.y + transform.position.z));
            var now = field.AdaptInversePosition(transform.position);
            var infield = Vector3.zero;

            if (next.x < 0 || now.x < 0)
            {
                infield += field.AdaptPosition(new Vector3((-field.AdaptInversePosition(transform.position).x + 5) * Time.deltaTime, 0, 0));
                if (rot_chain.Count > 0)
                {
                    rot_chain.RemoveLast();
                }
                rot_chain.AddLast(move);
            }
            else if (next.x > Constants.Width || now.x > Constants.Width)
            {
                infield += field.AdaptPosition(new Vector3((Constants.Width - field.AdaptInversePosition(transform.position).x - 5) * Time.deltaTime, 0, 0));
                if (rot_chain.Count > 0)
                {
                    rot_chain.RemoveLast();
                }
                rot_chain.AddLast(move);
            }

            if (next.z < 0 || now.z < 0)
            {
                infield += field.AdaptPosition(new Vector3(0, 0, (-field.AdaptInversePosition(transform.position).z + 5) * Time.deltaTime));
                if (rot_chain.Count > 0)
                {
                    rot_chain.RemoveLast();
                }
                rot_chain.AddLast(move);
            }
            else if (next.z > Constants.G2G || now.z > Constants.G2G)
            {
                infield += field.AdaptPosition(new Vector3(0, 0, (Constants.G2G - field.AdaptInversePosition(transform.position).z - 5) * Time.deltaTime));
                if (rot_chain.Count > 0)
                {
                    rot_chain.RemoveLast();
                }
                rot_chain.AddLast(move);
            }

            if (infield != Vector3.zero)
            {
                move.x = infield.x;
                move.y = infield.z;
            }

            before_velocity = move / Time.deltaTime;

            return move;
        }

        private Vector2 CalcRealVec(Vector2 vec)
        {
            var diff = vec - before_velocity;
            float deg = Vector2.Dot(before_velocity, diff) / before_velocity.magnitude / diff.magnitude;
            float coeff = (deg + 1) / 2 * field.info.GetAccUpCoeff(transform.position) + (1 - deg) / 2 * field.info.GetAccDownCoeff(transform.position);
            if (diff.sqrMagnitude > status.fast * status.fast * coeff * coeff / 180 / 180)
            {
                diff = coeff * status.fast * diff.normalized / 180;
            }

            return before_velocity + diff;
        }

        private float CalcRotAverage()
        {
            Vector2 vec = default;
            foreach (var v in rot_chain)
            {
                vec += v;
            }

            return Mathf.Atan2(vec.x, vec.y) * Mathf.Rad2Deg;
        }


        private void OnCollisionEnter(Collision other)
        {

            if (other.gameObject == ball.gameObject)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
}