using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// 游戏模式基类，承载游戏逻辑，运行时游戏的数据
/// </summary>
public abstract class GameModeBase
{
    //游戏是否在运行
    public bool GameRunning = false;

    //游戏是否暂停
	public bool GamePaused = false;

    //返回游戏模式的名称，用于多态区别游戏模式
    public abstract string Mode { get; }

    /// <summary>
    /// 初始化，在进入游戏场景之后调用
    /// </summary>
    public virtual void Init()
    {
		GameRunning = false;
		GamePaused = false;
	}

    /// <summary>
    /// 开始游戏，进入游戏场景之后第一帧调用
    /// </summary>
    public virtual void StartGame()
    {
		GameRunning = true;
		GamePaused = false;
    }

    /// <summary>
    /// 游戏循环，重载该方法提供游戏逻辑，默认不启动该协程，建议在StartGame的重载方法中启动
    /// </summary>
    /// <returns></returns>
	protected virtual IEnumerator GameLoop()
    {
        yield return 0;
    }

    /// <summary>
    /// 退出游戏场景之前调用,在该方法中清空游戏物体
    /// </summary>
    public virtual void GameOver()
    {
        GameRunning = true;
    }

    /// <summary>
    /// 游戏结束，游戏结束之后调用
    /// </summary>
    public virtual void GameResult()
    {
        GameRunning = false;
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public virtual void PauseGame()
    {
        GamePaused = true;
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public virtual void ResumeGame()
    {
        GamePaused = false;
    }

    /// <summary>
    /// 重新开始游戏，在改方法中重置对象，调用StartGame
    /// </summary>
    public virtual void RestartGame()
    {
		
    }

    /// <summary>
    /// 协程接口，用GameScene启动协程，统一管理
    /// </summary>
    /// <param name="coroutine"></param>
	protected void StartCoroutine(IEnumerator coroutine){
		GameScene.Instance.StartCoroutine (coroutine);
	}

    /// <summary>
    /// 根据游戏模式名称反射得到游戏模式对象
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static GameModeBase CreateGameMode(string mode)
    {
        string typeName = mode + "Mode";
        Type modeType = Type.GetType(typeName);
        if (modeType == null)
        {
            Debug.LogError("找不到类名为" + typeName + "的类");
            return null;
        }
        return (GameModeBase)Activator.CreateInstance(modeType);
    }

}

