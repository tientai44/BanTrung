using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelScriptTableObject", menuName = "Custom/LevelScriptTableObject")]
public class LevelScriptTableObject : ScriptableObject
{
    public string Filename;
    public List<int> CheckPoints;
    public int NumBall;
}