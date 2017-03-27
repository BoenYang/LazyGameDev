
public class UICommon{

    public enum UIState
    {

        Opened = 1,
        Closed = 2,
        Opening = 3,
        Closing = 4,
    }

    public class UIMsg
    {
        public object[] args;
    }
    

    public delegate void UIMsgCallback(UIMsg msg);
      
}
