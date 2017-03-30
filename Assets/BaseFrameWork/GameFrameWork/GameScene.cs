using UnityEngine;

/// <summary>
/// 游戏场景的第一个运行的脚本，用来作为启动游戏逻辑的第一个脚本
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
        Game.Init();
    }

    void Start()
    {
        Game.StartGame();
    }

    public T GetGameMode<T>() where T : GameModeBase
    {
        return Game as T;
    }
}
