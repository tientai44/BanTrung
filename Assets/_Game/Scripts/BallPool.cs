using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : GOSingleton<BallPool>
{
    [System.Serializable]
    public class Pool
    {
        public GameObject poolObjectPrefab;
        public int poolCount;
        public bool canGrow;
        public string tag;
    }
    public List<Pool> poolList = new List<Pool>();
    Dictionary<string, List<GameObject>> deActiveObjectPools = new Dictionary<string, List<GameObject>>();
    Dictionary<string, List<GameObject>> activeObjectPools = new Dictionary<string, List<GameObject>>();

  
    public void OnInit()
    {
        GetInstance();

        //weaponHolds[WeaponType.Uzi] = WeaponHold.UziHold;
        foreach (Pool pool in poolList)
        {
            List<GameObject> l = new List<GameObject>();
            for (int i = 0; i < pool.poolCount; i++)
            {
                GameObject obj = Instantiate(pool.poolObjectPrefab);
                obj.SetActive(false);
                l.Add(obj);

            }
            deActiveObjectPools[pool.tag] = l;
            activeObjectPools[pool.tag] = new List<GameObject>();
            //objectPools.Add(pool.tag, l);
            Debug.Log(pool.tag);
        }
    }
    public GameObject GetFromPool(string tag)
    {
        Pool tempPool = new Pool();
        foreach (Pool pool in poolList)
        {
            if (tag == pool.tag)
            {
                tempPool = pool;
                break;
            }
        }

        if (deActiveObjectPools[tag].Count > 0)
        {
            GameObject go = deActiveObjectPools[tag][0];
            go.SetActive(true);
            deActiveObjectPools[tag].Remove(go);
            activeObjectPools[tag].Add(go);
            return go;
        }
        else if (tempPool.canGrow)
        {
            GameObject go = Instantiate(tempPool.poolObjectPrefab);
            go.SetActive(true);
            activeObjectPools[tag].Add(go);

            return go;
        }
        else
        {
            return null;
        }
    }
    public GameObject GetFromPool(string tag, Vector3 pos)
    {
        Pool tempPool = new Pool();
        foreach (Pool pool in poolList)
        {
            if (tag == pool.tag)
            {
                tempPool = pool;
                break;
            }
        }

        if (deActiveObjectPools[tag].Count > 0 && deActiveObjectPools[tag][0] != null)
        {
            GameObject go = deActiveObjectPools[tag][0];
            go.transform.position = pos;
            go.SetActive(true);
            deActiveObjectPools[tag].Remove(go);
            activeObjectPools[tag].Add(go);

            return go;
        }
        else if (tempPool.canGrow)
        {
            GameObject go = Instantiate(tempPool.poolObjectPrefab);
            go.transform.position = pos;
            go.SetActive(true);
            activeObjectPools[tag].Add(go);

            return go;
        }
        else
        {
            return null;
        }
    }
    public void ReturnToPool(string tag, GameObject go)
    {
        Pool tempPool = new Pool();
        foreach (Pool pool in poolList)
        {
            if (tag == pool.tag)
            {
                tempPool = pool;
                break;
            }
        }
        switch (tag)
        {
            default:
                BasicReset(tag, go, tempPool);
                break;
        }
    }
    public void BasicReset(string tag, GameObject go, Pool tempPool)
    {
        go.transform.rotation = tempPool.poolObjectPrefab.transform.rotation;
        go.transform.localScale = tempPool.poolObjectPrefab.transform.localScale;
        activeObjectPools[tag].Remove(go);
        deActiveObjectPools[tag].Add(go);
        go.SetActive(false);
        go.transform.SetParent(null);
    }

    public void ClearObjectActive(string tag)
    {
        while (activeObjectPools[tag].Count > 0)
        {
            ReturnToPool(tag, activeObjectPools[tag][0]);
        }
    }
    IEnumerator ReturnPoolLate(string tag, GameObject go,float time)
    {
        yield return new WaitForSeconds(time);
        Pool tempPool = new Pool();
        foreach (Pool pool in poolList)
        {
            if (tag == pool.tag)
            {
                tempPool = pool;
                break;
            }
        }
        switch (tag)
        {
            default:
                BasicReset(tag, go, tempPool);
                break;
        }
    }
    public void ReturnToPool(string tag, GameObject go,float time)
    {
        StartCoroutine(ReturnPoolLate(tag, go, time));
    }
}
