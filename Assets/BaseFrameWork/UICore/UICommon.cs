
public enum UIState
{
    Opened = 1,
    Closed = 2,
    Opening = 3,
    Closing = 4,
}

public class UIMsg
{
    public string MsgType;

    public object[] args;
}

/// <summary>
/// UI消息回调委托
/// </summary>
/// <param name="msg">消息对象</param>
public delegate void UIMsgCallback(UIMsg msg);

