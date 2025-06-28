using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShockTrigger : SingletonMono<CircleShockTrigger>
{
    
    public SpriteRenderer BackgroundSR;//����ͼ��renderer
    public GameObject BackgroundObj;//����ͼ��gameobject
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

    public void TriggerCircleShock(Transform Target)//�����ܵ�transform����ɱ�������Ч��
    {
        float BearX, BearY;
        BearX = Target.position.x;
        BearY = Target.position.y;

        Vector2 BearPos = new Vector2(BearX, BearY); //�ܵ�2ά��������
        //Vector2 spriteSize = BackgroundSR.sprite.bounds.size; // ���ؿռ��С����λ�ǡ���λ����
        //Vector2 scale = BackgroundObj.transform.lossyScale;   // ��������
        //Vector2 SizeOfBackground = new Vector2(spriteSize.x * scale.x, spriteSize.y * scale.y); // �õ�����ͼʵ�ʴ�С     
        if (BackgroundSR == null)
        {
            GameObject obj = GameObject.Find("BackGround");
            BackgroundSR = obj.GetComponent<SpriteRenderer>();
        }
        Vector2 BearUV = GetUVCoord(BearPos, BackgroundSR); // �õ����ڱ���ͼ�ϵ�UV����

        BackgroundSR.material.SetVector("_RingSpawnPosition", BearUV); // ���ò��ʵ���UV����
        
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

    public Vector2 GetUVCoord(Vector2 worldPos, SpriteRenderer bg) //�������������Լ�����ͼ��SpriteRenderer�����ر���ͼUV����
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
