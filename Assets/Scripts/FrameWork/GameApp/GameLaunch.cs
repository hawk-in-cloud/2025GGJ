using UnityEngine;
using Framework;
public class GameLaunch : SingletonMono<GameLaunch>
{
    private void Awake()
    {
        // ��ʼ����Ϸ���: ��Դ����,UI����,�������,������־����,��������....
        // end

        // ��ʼ����Ϸ���ģ��
        this.gameObject.AddComponent<GameApp>();
        // end
    }

    private void Start()
    {
        // �����Դ����
        // end

        // �����ҵ���Ϸ
        GameApp.Instance.GameStart();
        // end
    }
}
