using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManagerable
{
    void AllResetedSignal();
    void PlaySignal();
    void PauseSignal();
    void PauseBackSignal();
    void PlayAgainSignal();
    void FinishSignal();

}
