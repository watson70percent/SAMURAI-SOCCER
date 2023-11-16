using SamuraiSoccer.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SamuraiSoccer.SoccerGame
{
    public class EndingJoyCon : MonoBehaviour
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
        [SerializeField]
        EndingAim endingAim;

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
                    Controller(5 / m_radius * dir); //�ړ��C�x���g���s
                    m_joyrect.localPosition = m_joyStartPosition + dir / m_scale; //�R���g���[���̃X�e�B�b�N���ړ�
                }
            }
            else
            {
                Controller(Vector2.zero);
            }
        }

        /// <summary>
        /// �����炭EventTrigger�ŌĂт���
        /// �ŏ��̃^�b�`�̌��o
        /// </summary>
        /// <param name="baseEventData"></param>
        public void DragStart(BaseEventData baseEventData)
        {
            PointerEventData pointerEventData = baseEventData as PointerEventData; //�����^�b�`���
            m_fingerID = pointerEventData.pointerId; //���t���[���ȍ~�ł̃^�b�`���ʂɗp����
            m_isDragged = true;
            try
            {
                Touch touch = Input.GetTouch(m_fingerID);
                m_slideStartPosition = touch.position;
            }
            catch
            {
                m_isDragged = false; //��肪��������^�b�`����
            }
        }

        /// <summary>
        /// �����炭EventTrigger�ŌĂт���
        /// �^�b�`�I�����o
        /// </summary>
        public void DragEnd()
        {
            m_isDragged = false;
            m_joyrect.localPosition = m_joyStartPosition;
        }

        void Controller(Vector2 dir)
        {
            dir = new Vector2(dir.x, dir.y)*Time.deltaTime;
            endingAim.MoveAim(new Vector3(dir.x, dir.y, 0));
        }

        /// <summary>
        /// �^�b�`��񂩂�O�t���[���Ɠ����^�b�`������΂����Ԃ�
        /// ����^�b�`�͕ʂ̂Ƃ���œo�^����̂�return new Touch();�͌Ă΂�Ȃ�
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
