using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI管理提，负责UI的关闭，打开相关调度
/// </summary>
public class UIManager : MonoBehaviour
{

    public static UIBase CurrentUIPanel;

    public static Camera UICamera;

    public static Canvas UICanvas;

    private static List<UIBase> uiStack = new List<UIBase>();

    private static UILoader loader = new UILoader();

    private static Dictionary<string,List<UIMsgCallback>> uiMsgDict = new Dictionary<string, List<UIMsgCallback>>(); 

    private static Dictionary<string,UIBase> uiCache = new Dictionary<string, UIBase>(); 

    private static UIManager instance;

	private static CanvasScaler canvasScaler;

    void Awake()
    {
        instance = this;
		canvasScaler = GetComponent<CanvasScaler> ();
        UICanvas = GetComponent<Canvas>();
        GameObject camera = GameObject.Find("UICamera");
        GameObject eventSystem = GameObject.Find("EventSystem");

        UICamera = camera.GetComponent<Camera>();

        DontDestroyOnLoad(camera);
        DontDestroyOnLoad(eventSystem);
        DontDestroyOnLoad(gameObject);
    }

    public static void OpenPanel(string uiName,bool closeBottom = false,params object[] args)
    {
        if (instance == null)
        {
            return;
        }

        UIBase panel = uiStack.Find((p) => p.UIName.Equals(uiName));

        if (panel == null)
        {

            if (uiCache.ContainsKey(uiName))
            {
                panel = uiCache[uiName];
				uiCache.Remove (uiName);
            }
            else
            {
                GameObject uiPrefab = loader.GetUIByName(uiName);

                GameObject ui = Instantiate(uiPrefab);
                panel = ui.GetComponent<UIBase>();
                panel.UIName = uiName;
                panel.OnInit();
            }

            CurrentUIPanel = panel;
            panel.Args = args;
            panel.gameObject.SetActive(true);
            panel.transform.SetParent(instance.transform);
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.one;
			RectTransform rtf = panel.GetComponent<RectTransform> ();
            rtf.anchorMax = new Vector2(1,1);
            rtf.anchorMin = new Vector2(0, 0);
            rtf.offsetMax = new Vector2(0,0);
            rtf.offsetMin = new Vector2(0, 0);
            panel.OnRefresh();

			if (uiStack.Count > 0 && closeBottom)
            {
                UIBase lastPanel = uiStack[uiStack.Count - 1];
                lastPanel.gameObject.SetActive(false);
            }

            uiStack.Add(panel);
        }
        else
        {
            Debug.LogError(uiName + "已经打开");
        }

    }

    public static void ClosePanel(string name)
    {
        UIBase uiPanel = uiStack.Find((p) => p.UIName.Equals(name));

        if (uiPanel != null)
        {
            uiPanel.gameObject.SetActive(false);

            uiStack.Remove(uiPanel);

            uiCache.Add(uiPanel.UIName, uiPanel);

            if (uiStack.Count > 0)
            {
                uiStack[uiStack.Count - 1].gameObject.SetActive(true);
                uiStack[uiStack.Count - 1].OnRefresh();
            }
        }
        else
        {
            Debug.LogError("没有找到要" + name);
        }
    }

    public static void CloseTop()
    {
        if (uiStack.Count > 0)
        {
            UIBase uiPanel = uiStack[uiStack.Count - 1];

            uiPanel.OnBeginClose();
            uiPanel.gameObject.SetActive(false);
            uiPanel.OnEndClose();

            uiStack.Remove(uiPanel);
            uiCache.Add(uiPanel.UIName, uiPanel);

			if (uiStack.Count > 0)
			{
				uiStack[uiStack.Count - 1].gameObject.SetActive(true);
                uiStack[uiStack.Count - 1].OnRefresh();
            }
        }
    }

    public static void DispatchMsg(string msgType,params object[] args)
    {
        if (instance == null)
        {
            return;
        }

        UIMsg m = new UIMsg();

        m.MsgType = msgType;
        m.args = args;

        if (uiMsgDict.ContainsKey(msgType))
        {
            List<UIMsgCallback> listenerList = uiMsgDict[msgType];
            for (int i = 0; i < listenerList.Count; i++)
            {
                UIMsgCallback listener = listenerList[i];
                if (listener.Method.IsStatic)
                {
                    listener.Invoke(m);
                }
                else if(listener.Target != null)
                {
                    listener.Invoke(m);
                }
            }
        }
    }

    public static void AddListener(string msgType,UIMsgCallback callback)
    {
        if (uiMsgDict.ContainsKey(msgType))
        {
            if (callback != null)
            {
                if (!uiMsgDict[msgType].Contains(callback))
                {
                    uiMsgDict[msgType].Add(callback);
                }
                else
                {
                    Debug.LogError("消息" + msgType + "已经注册过了");
                }
            }
        }
        else
        {
            uiMsgDict.Add(msgType,new List<UIMsgCallback>());
            uiMsgDict[msgType].Add(callback);
        }
    }
}
