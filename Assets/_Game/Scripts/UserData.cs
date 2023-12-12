using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserData 
{
    public static bool IsSoundOn
    {
        get
        {
            return PlayerPrefs.GetInt("IsSoundOn", 1)==1;
        }
        set
        {
            PlayerPrefs.SetInt("IsSoundOn", value ? 1 : 0);
        }
    }
    public static bool IsMusicOn
    {
        get
        {
            return PlayerPrefs.GetInt("IsMusicOn", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("IsMusicOn", value ? 1 : 0);
        }
    }
}
