using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScrptableObject/CreateSamuraiWordBase",fileName ="SamuraiWordBase")]
public class SamuraiWordBase : ScriptableObject
{
    [TextArea(1,3)]
    public List<string> samuraiwords = new List<string>();
}

