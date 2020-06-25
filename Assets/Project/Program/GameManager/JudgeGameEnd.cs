using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeGameEnd : MonoBehaviour
{
    private bool _endflag = false;
    /// <summary>
    /// 試合終了かどうか判定するフラグ
    /// false→試合中
    /// true→試合終了
    /// </summary>
    public bool EndFlag
    {
        get { return _endflag; }
        set { _endflag = value; }
    }

}
