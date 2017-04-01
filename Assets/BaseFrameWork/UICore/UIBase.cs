using UnityEngine;

/// <summary>
/// UI界面基类，每一个新的UI界面都应该继承该类，在子类中实现UI的逻辑
/// </summary>
public class UIBase : MonoBehaviour
{

    /// <summary>
    /// UI的名称
    /// </summary>
    [System.NonSerialized]
    public string UIName;

    /// <summary>
    /// 跨界面传递的参数
    /// </summary>
    public object[] Args;

    /// <summary>
    /// 实例化之后会调用改方法
    /// </summary>
    public virtual void OnInit()
    {

    }

    /// <summary>
    /// UI动画开始的时候调用
    /// </summary>
    public virtual void OnBeginOpen()
    {

    }

    /// <summary>
    /// 每次打开界面都会调用
    /// </summary>
    public virtual void OnRefresh()
    {

    }

    /// <summary>
    /// UI动画结束的时候调用
    /// </summary>
    public virtual void OnEndOpen()
    {

    }

    /// <summary>
    /// 关闭动画开始时调用
    /// </summary>
    public virtual void OnBeginClose()
    {

    }

    /// <summary>
    /// 关闭动画结束之后调用
    /// </summary>
    public virtual void OnEndClose()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void OnResume()
    {

    }

    /// <summary>
    /// UI销毁之前调用
    /// </summary>
    public virtual void OnStop()
    {

    }

    /// <summary>
    /// 关闭UI自身
    /// </summary>
    public void ClosePanel()
    {
        UIManager.ClosePanel(UIName);
    }

    /// <summary>
    /// 发送UI消息
    /// </summary>
    /// <param name="msgType">消息类型</param>
    /// <param name="msg">消息</param>
    protected void DispatchMsg(string msgType, params object[] args)
    {
        UIManager.DispatchMsg(msgType,args);
    }

    /// <summary>
    /// 添加UI消息监听
    /// </summary>
    /// <param name="msgType"></param>
    /// <param name="callBack"></param>
    protected void AddMsgListener(string msgType,UIMsgCallback callBack)
    {
        UIManager.AddListener(msgType,callBack);
    }
}
