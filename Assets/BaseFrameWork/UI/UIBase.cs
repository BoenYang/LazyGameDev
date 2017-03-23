using UnityEngine;

public class UIBase : MonoBehaviour
{
    public string UIName;

    public virtual void OnInit()
    {

    }

    public virtual void OnBeginOpen()
    {

    }

    public virtual void OnRefresh()
    {

    }

    public virtual void OnEndOpen()
    {

    }

    public virtual void OnBeginClose()
    {

    }

    public virtual void OnEndClose()
    {

    }

    public virtual void OnResume()
    {

    }

    public virtual void OnStop()
    {

    }


    protected void DispatchMsg(string msgType, UICommon.UIMsg msg)
    {

    }

    protected void AddMsgListener(string msgType)
    {

    }
}
