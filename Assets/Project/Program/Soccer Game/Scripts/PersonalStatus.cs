using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalStatus
{
    public readonly int MAX_HP;
    public int hp;
    public int Power { get { return power * (hp / MAX_HP); } private set { power = value;} }
    private int power;
    public readonly bool ally;

    public PersonalStatus(int _hp = 1, int _power = 0, bool _ally = true)
    {
        MAX_HP = _hp;
        hp = _hp;
        Power = _power;
        ally = _ally;
    }

}
