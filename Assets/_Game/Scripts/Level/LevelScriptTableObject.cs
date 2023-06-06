using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LevelType
{
    ClearBall,SaveRabbit,CollectFlower
}

[CreateAssetMenu(fileName = "NewLevelScriptTableObject", menuName = "Custom/LevelScriptTableObject")]
public class LevelScriptTableObject : ScriptableObject
{
    public string Filename;
    public List<int> CheckPoints;
    public int NumBall;
    public LevelType LevelType;
    public static Dictionary<LevelType, string> descriptions = new Dictionary<LevelType, string>
    {
        {LevelType.ClearBall,"Eliminate Targeted Elements" },
        {LevelType.SaveRabbit,"Rescue Animals" },
        {LevelType.CollectFlower,"Collect Flower" }
    };
}