using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SamuraiSoccer.SoccerGame
{
    public class EndingAim : MonoBehaviour
    {
        [SerializeField]
        Transform limitXMax;
        [SerializeField]
        Transform limitXMin;
        [SerializeField]
        Transform limitYMax;
        [SerializeField]
        Transform limitYMin;
        [SerializeField]
        GameObject slashParent;
        [SerializeField]
        Camera m_Camera;
        [SerializeField]
        RectTransform m_aimUI;
        [SerializeField]
         RectTransform m_parentUI;


        public void MoveAim(Vector3 dir)
        {
            transform.position += dir;
            Vector3 pos = transform.position;

            if (pos.x > limitXMax.transform.position.x) pos.x = limitXMax.transform.position.x;
            if (pos.y > limitYMax.transform.position.y) pos.y = limitYMax.transform.position.y;
            if (pos.x < limitXMin.transform.position.x) pos.x = limitXMin.transform.position.x;
            if (pos.y < limitYMin.transform.position.y) pos.y = limitYMin.transform.position.y;
            transform.position = pos;

            slashParent.transform.LookAt(transform);

            // オブジェクトのワールド座標
            var targetWorldPos = transform.position;

            // ワールド座標をスクリーン座標に変換する
            var targetScreenPos = m_Camera.WorldToScreenPoint(targetWorldPos);
            
           

            // スクリーン座標→UIローカル座標変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_parentUI,
                targetScreenPos,
                null,
                out var uiLocalPos
            );
            Debug.Log(uiLocalPos);

            // RectTransformのローカル座標を更新
            m_aimUI.localPosition = uiLocalPos;
        }
    }
}
