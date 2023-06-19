using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class G4_UIManager : G4_GOSingleton<G4_UIManager>
{
    //dict for quick query UI prefab
    //dict dung de lu thong tin prefab canvas truy cap cho nhanh
    private Dictionary<System.Type, G4_UICanvas> uiCanvasPrefab = new Dictionary<System.Type, G4_UICanvas>();

    //list from resource
    //list load ui resource
    [SerializeField]private G4_UICanvas[] uiResources;

    //dict for UI active
    //dict luu cac ui dang dung
    private Dictionary<System.Type, G4_UICanvas> uiCanvas = new Dictionary<System.Type, G4_UICanvas>();

    //canvas container, it should be a canvas - root
    //canvas chua dung cac canvas con, nen la mot canvas - root de chua cac canvas nay
    public Transform CanvasParentTF;

    #region Canvas

    //open UI
    //mo UI canvas
    public T OpenUI<T>() where T : G4_UICanvas
    {
        G4_UICanvas canvas = GetUI<T>();

        canvas.Setup();
        canvas.Open();

        return canvas as T;
    }

    //close UI directly
    //dong UI canvas ngay lap tuc
    public void CloseUI<T>() where T : G4_UICanvas
    {
        if (IsOpened<T>())
        {
            GetUI<T>().CloseDirectly();
        }
    }

    //close UI with delay time
    //dong ui canvas sau delay time
    public void CloseUI<T>(float delayTime) where T : G4_UICanvas
    {
        if (IsOpened<T>())
        {
            GetUI<T>().Close(delayTime);
        }
    }

    //check UI is Opened
    //kiem tra UI dang duoc mo len hay khong
    public bool IsOpened<T>() where T : G4_UICanvas
    {
        return IsLoaded<T>() && uiCanvas[typeof(T)].gameObject.activeInHierarchy;
    }

    //check UI is loaded
    //kiem tra UI da duoc khoi tao hay chua
    public bool IsLoaded<T>() where T : G4_UICanvas
    {
        System.Type type = typeof(T);
        return uiCanvas.ContainsKey(type) && uiCanvas[type] != null;
    }

    //Get component UI 
    //lay component cua UI hien tai
    public T GetUI<T>() where T : G4_UICanvas
    {
        if (!IsLoaded<T>())
        {
            G4_UICanvas canvas = Instantiate(GetUIPrefab<T>(), CanvasParentTF);
            uiCanvas[typeof(T)] = canvas;
        }

        return uiCanvas[typeof(T)] as T;
    }

    //Close all UI
    //dong tat ca UI ngay lap tuc -> tranh truong hop dang mo UI nao dong ma bi chen 2 UI cung mot luc
    public void CloseAll()
    {
        foreach (var item in uiCanvas)
        {
            if (item.Value != null && item.Value.gameObject.activeInHierarchy)
            {
                item.Value.CloseDirectly();
            }
        }
    }

    //Get prefab from resource
    //lay prefab tu Resources/UI 
    private T GetUIPrefab<T>() where T : G4_UICanvas
    {
        if (!uiCanvasPrefab.ContainsKey(typeof(T)))
        {
            if (uiResources == null)
            {
                uiResources = Resources.LoadAll<G4_UICanvas>("UI/");
            }

            for (int i = 0; i < uiResources.Length; i++)
            {
                if (uiResources[i] is T)
                {
                    uiCanvasPrefab[typeof(T)] = uiResources[i];
                    break;
                }
            }
        }

        return uiCanvasPrefab[typeof(T)] as T;
    }


    #endregion

    #region Back Button

    private Dictionary<G4_UICanvas, UnityAction> BackActionEvents = new Dictionary<G4_UICanvas, UnityAction>();
    private List<G4_UICanvas> backCanvas = new List<G4_UICanvas>();
    G4_UICanvas BackTopUI
    {
        get
        {
            G4_UICanvas canvas = null;
            if (backCanvas.Count > 0)
            {
                canvas = backCanvas[backCanvas.Count - 1];
            }

            return canvas;
        }
    }


    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Escape) && BackTopUI != null)
        {
            BackActionEvents[BackTopUI]?.Invoke();
        }
    }

    public void PushBackAction(G4_UICanvas canvas, UnityAction action)
    {
        if (!BackActionEvents.ContainsKey(canvas))
        {
            BackActionEvents.Add(canvas, action);
        }
    }

    public void AddBackUI(G4_UICanvas canvas)
    {
        if (!backCanvas.Contains(canvas))
        {
            backCanvas.Add(canvas);
        }
    }

    public void RemoveBackUI(G4_UICanvas canvas)
    {
        backCanvas.Remove(canvas);
    }

    /// <summary>
    /// CLear backey when comeback index UI canvas
    /// </summary>
    public void ClearBackKey()
    {
        backCanvas.Clear();
    }

    #endregion
}
