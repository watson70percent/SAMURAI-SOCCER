﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// フィールド
    /// </summary>
    [Serializable]
    public class FieldInfo
    {
        public float[][] acc_up_coeff = default;
        public float[][] acc_down_coeff = default;
        public float[][] drag = default;

        public float GetAccUpCoeff(Vector3 position)
        {
            try
            {
                if (position.x < 0 || position.x > 59)
                {
                    return 1;
                }
                if (position.z < 0 || position.z > 99)
                {
                    return 1;
                }
                return acc_up_coeff[Mathf.FloorToInt(position.x)][Mathf.FloorToInt(position.z)];
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public float GetAccDownCoeff(Vector3 position)
        {
            try
            {
                if (position.x < 0 || position.x > 59)
                {
                    return 1;
                }
                if (position.z < 0 || position.z > 99)
                {
                    return 1;
                }
                return acc_down_coeff[Mathf.FloorToInt(position.x)][Mathf.FloorToInt(position.z)];
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public float Getdrag(Vector3 position)
        {
            try
            {
                if (position.x < 0 || position.x > 59)
                {
                    return 1;
                }
                if (position.z < 0 || position.z > 99)
                {
                    return 1;
                }
                return drag[Mathf.FloorToInt(position.x)][Mathf.FloorToInt(position.z)];
            }
            catch (Exception)
            {
                return 4;
            }
        }
    }
}