using System;
using System.IO;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEditor : EditorWindow {


    [MenuItem("Window/UI/CreateFrameWork")]
    public static void CreateUIFrameWork()
    {
        GameObject go = new GameObject("UICanvas");
        go.layer = LayerMask.NameToLayer("UI");
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        GraphicRaycaster raycaster = go.AddComponent<GraphicRaycaster>();
        UIManager uiManager = go.AddComponent<UIManager>();

        GameObject uiCamrea = new GameObject("UICamera");
        Camera camera = uiCamrea.AddComponent<Camera>();

        canvas.worldCamera = camera;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<StandaloneInputModule>();


        string prefabPath = Application.dataPath + "/Resources/UI";
        if (!Directory.Exists(prefabPath))
        {
            Debug.Log("创建资源文件夹" + prefabPath);
            Directory.CreateDirectory(prefabPath);
        }
       
        AssetDatabase.Refresh();
    }

    [MenuItem("GameObject/Create UI")]
    public static void CreateUI()
    {

        GameObject uiParent = Selection.activeGameObject;

        if (uiParent == null)
        {
            Debug.LogError("请选择UI的父节点");
            return;
        }

        string scriptPath = Application.dataPath + "/Scripts/UI";
        if (!Directory.Exists(scriptPath))
        {
            Debug.Log("创建脚本文件夹" + scriptPath);
            Directory.CreateDirectory(scriptPath);
        }

        EditorInputDialog editorInputDialog = EditorWindow.GetWindowWithRect<EditorInputDialog>(new Rect(Input.mousePosition.x, Input.mousePosition.y, 300, 100),true,"创建UI");
        editorInputDialog.Show((input) =>
        {

        });
    }
}

public class EditorInputDialog : EditorWindow
{

    private Action<string> OKCallback;

    private string uiName = "";

    public void Show(Action<string> okCallBack)
    {
        this.OKCallback = okCallBack;
        this.wantsMouseMove = false;
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(30);

        uiName = EditorGUILayout.TextField(uiName);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("OK"))
        {
            if (OKCallback != null)
            {
                OKCallback.Invoke(uiName);
            }
        }

        GUILayout.Space(40);

        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }

        GUILayout.EndVertical();
    }
}
