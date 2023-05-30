using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : GOSingleton<SaveLoadManager>
{
    [System.Serializable]
    public class Data
    {
        int currentLv;
        List<int> points;
        public Data()
        {
            currentLv = 1;
            points = new List<int>();
        }
        
    }
}
