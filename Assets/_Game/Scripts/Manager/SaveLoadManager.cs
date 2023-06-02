using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using Unity.Burst.CompilerServices;
using System;

public class SaveLoadManager : GOSingleton<SaveLoadManager>
{
    [System.Serializable]
    public class Data
    {
        int currentLv;
        List<int> points;
        List<int> starNumbers;
        public Data()
        {
            currentLv = 1;
            points = new List<int>();
            starNumbers = new List<int>();
        }

        public int CurrentLv { get => currentLv; set => currentLv = value; }
        public List<int> Points { get => points; set => points = value; }
        public List<int> StarNumbers { get => starNumbers; set => starNumbers = value; }
    }
    private string saveFileName = Constants.SAVE_FILE_NAME;
    private string saveFilePath; // ???ng d?n ??n file l?u d? li?u

    [SerializeField] private bool loadOnStart = true;
    private Data data;// Object dung de luu du lieu
    private BinaryFormatter formatter; //Object dung ma hoa data roi luu ra file duoi dang binary

    public Data Data1 { get => data; set => data = value; }
    public void OnInit()// Goi den khi ma nguoi choi vao game
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName); // Lay duong dan cua file minh save
        formatter = new BinaryFormatter();
        Debug.Log(saveFilePath);
        if (loadOnStart)
        {
            Load();
        }
        //UIManager.GetInstance().DisplayMainMenuPanel();
        Debug.Log(saveFileName);
        
    }
    public void Load()
    {

        try
        {
            FileStream file = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read);
            try
            {
                data = (Data)formatter.Deserialize(file);
                
            }
            catch
            {
                Debug.Log("Cant Read Data");
                file.Close();
                Save();
            }
            file.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Save();
        }
    }

    public void Save()
    {
        if (data == null)
        {
            data = new Data();
        }
        try
        {
            FileStream file = new FileStream(saveFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            formatter.Serialize(file, data);
            file.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}

