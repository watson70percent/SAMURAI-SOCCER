using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase
{
    protected int[,] valueMap = new int[60, 100];
    protected bool ally;
    protected GameObject[] teamMate = new GameObject[10];
    protected GameObject[] opponent = new GameObject[10];

    public virtual void Revaluation() { }

    public virtual Vector2 MaxValuePoint(Vector2 self) { return self; }
}
