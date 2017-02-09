using System.Collections.Generic;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    private static List<UIPanel> uiStack = new List<UIPanel>();

    private static UILoader loader = new UILoader();

    private static UIManager instance;

    void Awake()
    {
        instance = this;
    }

    public static void OpenPanel(string name)
    {
        UIPanel panel = uiStack.Find((p) => p.name == name);

        if (panel == null)
        {
            GameObject uiPrefab = loader.GetUIByName(name);
            GameObject ui = Instantiate(uiPrefab);
            ui.name = name;
            //ui.transform.SetParent();
            ui.transform.localPosition = Vector3.zero;
            ui.transform.localScale = Vector3.one;

            panel = ui.GetComponent<UIPanel>();
            uiStack.Add(panel);
        }
    }

    public static void ClosePanel(string name)
    {

    }

    public static void CloseTop()
    {
        
    }
}
