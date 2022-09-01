using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using System;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class SlidePad : MonoBehaviour
    {
        private float m_radius;
        private float m_scale;
        private bool m_isDragged;
        [SerializeField]
        private GameObject m_joystick;
        private RectTransform m_joyrect;
        private Vector2 m_joyStartPosition;
        private Vector2 m_slideStartPosition;
        private int m_fingerID;

        // Start is called before the first frame update
        void Start()
        {
            m_joyrect = m_joystick.GetComponent<RectTransform>();
            m_joyStartPosition = m_joyrect.localPosition;

            m_scale = transform.localScale.x;
            m_radius = 50 * m_scale;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            PlayingState();
        }

        void PlayingState()
        {
            if (m_isDragged == true)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = FindFinger();
                    Vector2 dir = touch.position - m_slideStartPosition;
                    if (dir.magnitude > m_radius) { dir = dir.normalized * m_radius; }
                    Controller(5 / m_radius * dir); //移動イベント発行
                    m_joyrect.localPosition = m_joyStartPosition + dir / m_scale; //コントローラのスティックを移動
                }
            }
            else
            {
                Controller(Vector2.zero);
            }
        }

        /// <summary>
        /// おそらくEventTriggerで呼びだす
        /// 最初のタッチの検出
        /// </summary>
        /// <param name="baseEventData"></param>
        public void DragStart(BaseEventData baseEventData)
        {
            PointerEventData pointerEventData = baseEventData as PointerEventData; //多分タッチ情報
            m_fingerID = pointerEventData.pointerId; //次フレーム以降でのタッチ識別に用いる
            m_isDragged = true;
            try
            {
                Touch touch = Input.GetTouch(m_fingerID);
                m_slideStartPosition = touch.position;
            }
            catch
            {
                m_isDragged = false; //問題が生じたらタッチ無効
            }
        }

        /// <summary>
        /// おそらくEventTriggerで呼びだす
        /// タッチ終了検出
        /// </summary>
        public void DragEnd()
        {
            m_isDragged = false;
            m_joyrect.localPosition = m_joyStartPosition;
        }

        void Controller(Vector2 dir)
        {
            dir = new Vector2(2.0f * dir.y, -dir.x);
            PlayerEvent.StickControllerOnNext(new Vector3(dir.x, 0, dir.y));
        }

        /// <summary>
        /// タッチ情報から前フレームと同じタッチがあればそれを返す
        /// 初回タッチは別のところで登録するのでreturn new Touch();は呼ばれない
        /// </summary>
        /// <returns></returns>
        Touch FindFinger()
        {
            foreach (Touch t in Input.touches)
            {
                if (t.fingerId == m_fingerID) { return t; } 
            }
            return new Touch();
        }
    }
}

