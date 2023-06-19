using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum G4_LevelType
{
    ClearBall,SaveRabbit,CollectFlower
}

[CreateAssetMenu(fileName = "NewLevelScriptTableObject", menuName = "Custom/LevelScriptTableObject")]
public class G4_LevelScriptTableObject : ScriptableObject
{
    public string Filename;
    public List<int> CheckPoints;
    public int NumBall;
    public G4_LevelType LevelType;
    public static Dictionary<G4_LevelType, string> descriptions = new Dictionary<G4_LevelType, string>
    {
        {G4_LevelType.ClearBall,"Eliminate Targeted Elements" },
        {G4_LevelType.SaveRabbit,"Rescue Animals" },
        {G4_LevelType.CollectFlower,"Collect Flower" }
    };
}