using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShockTrigger : SingletonMono<CircleShockTrigger>
{
    
    public SpriteRenderer BackgroundSR;//背景图的renderer
    public GameObject BackgroundObj;//背景图的gameobject
    private void Start()
    {
        GameObject obj = GameObject.Find("BackGround");
        if (BackgroundSR == null)
        {
            BackgroundSR = obj.GetComponent<SpriteRenderer>();
        }
        if (BackgroundObj == null)
        {
            BackgroundObj = obj;
        }
    }

    Tween tw;

    public void TriggerCircleShock(Transform Target)//输入熊的transform，造成背景波动效果
    {
        float BearX, BearY;
        BearX = Target.position.x;
        BearY = Target.position.y;

        Vector2 BearPos = new Vector2(BearX, BearY); //熊的2维世界坐标
        //Vector2 spriteSize = BackgroundSR.sprite.bounds.size; // 本地空间大小（单位是“单位”）
        //Vector2 scale = BackgroundObj.transform.lossyScale;   // 世界缩放
        //Vector2 SizeOfBackground = new Vector2(spriteSize.x * scale.x, spriteSize.y * scale.y); // 得到背景图实际大小     
        if (BackgroundSR == null)
        {
            GameObject obj = GameObject.Find("BackGround");
            BackgroundSR = obj.GetComponent<SpriteRenderer>();
        }
        Vector2 BearUV = GetUVCoord(BearPos, BackgroundSR); // 得到熊在背景图上的UV坐标

        BackgroundSR.material.SetVector("_RingSpawnPosition", BearUV); // 设置材质的熊UV坐标
        
        if(tw != null)
        {
            tw.Kill();
            BackgroundSR.material.SetFloat("_WaveDistance", -0.1f);
        }
        tw = BackgroundSR.material.DOFloat(1, "_WaveDistance", 0.5f).OnComplete(() =>
        {
            BackgroundSR.material.SetFloat("_WaveDistance", -0.1f);
        });
    }

    public Vector2 GetUVCoord(Vector2 worldPos, SpriteRenderer bg) //输入世界坐标以及背景图的SpriteRenderer，返回背景图UV坐标
    {
        Vector2 worldMin = bg.bounds.min;
        Vector2 worldMax = bg.bounds.max;
        Vector2 size = worldMax - worldMin;

        return new Vector2(
            Mathf.InverseLerp(worldMin.x, worldMax.x, worldPos.x),
            Mathf.InverseLerp(worldMin.y, worldMax.y, worldPos.y)
        );
    }
}
