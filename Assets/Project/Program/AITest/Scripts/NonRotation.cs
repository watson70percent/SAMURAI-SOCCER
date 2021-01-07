using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonRotation : FieldRotationBase
{
    protected override void Awake()
    {
        rotation = Quaternion.Euler(0, 0, 0);
    }

}
