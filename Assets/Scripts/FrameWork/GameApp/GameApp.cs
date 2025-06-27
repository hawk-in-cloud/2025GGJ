using UnityEngine;
using Framework;
public class GameApp : SingletonMono<GameApp>
{
    public void GameStart()
    {
        this.EnterGameScene();
    }

    public void EnterGameScene()
    {
        // 释放游戏地图
        // end

        // 释放游戏角色，NPC
        // end

        // 释放游戏UI
        // end
    }
}
