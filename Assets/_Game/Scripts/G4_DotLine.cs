using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class G4_DotLine : MonoBehaviour
{
    [SerializeField] GameObject dotPrefab;
    public List<GameObject> active_Dots = new List<GameObject>();
    public List<GameObject> deactive_Dots = new List<GameObject>();
    public List<Vector2> pos;
    public GameObject storage;
    public float distance = 0.5f;
    public GameObject GetDot()
    {
        GameObject dot;
        if (deactive_Dots.Count == 0)
        {
            dot = Instantiate(dotPrefab);
            dot.transform.parent = storage.transform;

        }
        else
        {
            dot = deactive_Dots[0];
            dot.gameObject.SetActive(true);
            deactive_Dots.RemoveAt(0);
        }
        active_Dots.Add(dot);
        return dot;
    }
    public void ReturnDot(GameObject dot)
    {
        active_Dots.Remove(dot);
        deactive_Dots.Add(dot);
        dot.gameObject.SetActive(false);
    }
    public void DrawLineDot(Vector3 start, Vector3 end, float distance, BallColor ballColor)
    {

        Vector3 direct = (end - start).normalized;
        float num = ((end - start).magnitude / distance);
        int numDot;

        if (num - (int)num >= 0.5f)
        {
            numDot = (int)num + 1;
        }
        else
        {
            numDot = (int)num;
        }

        for (int i = 0; i < numDot; i++)
        {
            GameObject dot = GetDot();
            dot.transform.position = start + direct * (distance) * i;
        }
    }
    public void DrawManyLine(List<Vector2> positions, BallColor ballColor)
    {
        float space1 = 0;
        float space2 = 0;
        Vector2 direct = Vector2.zero;
        storage.SetActive(true);
        ReturnAllDot();
        pos = positions;

        if (positions.Count >= 2)
        {
            direct = (positions[1] - positions[0]).normalized;
            space2 = ((positions[1] - positions[0]).magnitude % distance);
            DrawLineDot(positions[0], positions[1], distance, ballColor);
        }

        for (int m = 1; m < positions.Count - 1; m++)
        {
            direct = (positions[m + 1] - positions[m]).normalized;
            space1 = space2;
            space2 = ((positions[m + 1] - positions[m]).magnitude % distance);
            DrawLineDot(positions[m] + direct * space1, positions[m + 1] - direct * space2, distance, ballColor);

        }
    }
    public void ReturnAllDot()
    {
        for (int i = 0; i < active_Dots.Count; i++)
        {
            deactive_Dots.Add(active_Dots[i]);
            active_Dots[i].gameObject.SetActive(false);
        }
        active_Dots.Clear();
    }

    public void HideDot()
    {
        storage.SetActive(false);
        //for (int i = 0; i < active_Dots.Count; ++i)
        //{
        //    active_Dots[i].gameObject.SetActive(false);
        //}
    }
}