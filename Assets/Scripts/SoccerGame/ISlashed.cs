using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    public interface ISlashed 
    {
        /// <summary>
        /// �Ռ��g�Ɏa���鎞�ɌĂ΂��
        /// </summary>
        /// <param name="dir">������ԕ���</param>
        public void Slashed(Vector3 dir);
    }
}
