using UnityEngine;
using Framework;
public class GameLaunch : SingletonMono<GameLaunch>
{
    private void Awake()
    {
        // 初始化游戏框架: 资源管理,UI管理,网络管理,调试日志管理,声音管理....
        // end

        // 初始化游戏入口模块
        this.gameObject.AddComponent<GameApp>();
        // end
    }

    private void Start()
    {
        // 检查资源更新
        // end

        // 进入我的游戏
        GameApp.Instance.GameStart();
        // end
    }
}
