using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// 風の情報を持ったクラス。これから派生させてください。
    /// </summary>
    public abstract class WindInfoBase : MonoBehaviour
    {
        /// <summary>
        /// 風の力
        /// </summary>
        public Vector3[][] wind = default;

        /// <summary>
        /// 初期化。派生クラスでは自由に
        /// </summary>
        protected virtual void Start()
        {
            wind = new Vector3[Mathf.FloorToInt(Constants.Width)][];
            for (int i = 0; i < Mathf.FloorToInt(Constants.Width); i++)
            {
                wind[i] = new Vector3[Mathf.FloorToInt(Constants.G2G)];
                for (int j = 0; j < Mathf.FloorToInt(Constants.G2G); j++)
                {
                    wind[i][j] = Vector2.zero;
                }
            }
        }

        public Vector3 WindEffect(Vector3 position)
        {
            try
            {
                return wind[Mathf.FloorToInt(position.x)][Mathf.FloorToInt(position.z)];
            }
            catch (Exception)
            {
                return Vector2.zero;
            }
        }
    }
}
