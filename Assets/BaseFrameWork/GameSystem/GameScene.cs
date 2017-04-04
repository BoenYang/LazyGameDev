using UnityEngine;

/// <summary>
/// 游戏场景的第一个运行的脚本，用来作为启动游戏逻辑的第一个脚本,
/// 提供一系列的重载函数接口，便于构建Debug场景的假数据加载等
/// </summary>
public class GameScene : MonoBehaviour
{
    public string GameMode = "Normal";

    public GameModeBase Game;

    public static GameScene Instance;

    void Awake()
    {
        Instance = this;
        Game = GameModeBase.CreateGameMode(GameMode);
        OnInit();
        InitGame();
    }

    void Start()
    {
        OnStart();
        StartGame();
    }

    protected virtual void OnInit()
    {

    }

    protected void InitGame()
    {
        Game.Init();
    }

    protected virtual void OnStart()
    {

    }

    protected virtual void StartGame()
    {
        Game.StartGame();
    }

    public T GetGameMode<T>() where T : GameModeBase
    {
        return Game as T;
    }
}
