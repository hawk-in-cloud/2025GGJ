using System.Linq;
using Framework;
using Gameplay.BaseItem;
using Gameplay.Other;
using UnityEngine;

namespace Gameplay
{
    public class MonsterMgr : SingletonMono<MonsterMgr>
    {
        //Props的父物体
        public Transform propsParent;
        //怪物的父物体
        public Transform monsterParent;
        //怪物的当前数量
        public int monsterCount;
        void Awake()
        {
            EventManager.Instance.AddEventListener(E_EventType.E_Monster_Dead, () =>
            {
                monsterCount--;
            });
            EventManager.Instance.AddEventListener(E_EventType.E_Monster_Generate, () =>
            {
                monsterCount++;
            });
        }

        void Start()
        {
            for (int i = 0; i < 50; i++)
            {
                var monster = PoolManager.Instance.GetObj("GameObject", RandomGetMonster());
                monster.transform.position = GetRandomSpawnPosition();
                monster.transform.parent = transform;
            }
        }
        /// <summary>
        /// 获取屏幕外 +2f 的随机怪物出生点（世界坐标）
        /// </summary>
        public Vector3 GetRandomSpawnPosition()
        {
            // 获取主摄像机
            Camera cam = Camera.main;

            // 屏幕边界（像素）
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 四个方向的屏幕外坐标（加偏移 2f）
            Vector3[] possiblePoints = new Vector3[]
            {
                cam.ScreenToWorldPoint(new Vector3(-100f, Random.Range(0, screenHeight), cam.nearClipPlane + 10f)), // 左
                cam.ScreenToWorldPoint(new Vector3(screenWidth + 100f, Random.Range(0, screenHeight), cam.nearClipPlane + 10f)), // 右
                cam.ScreenToWorldPoint(new Vector3(Random.Range(0, screenWidth), screenHeight + 100f, cam.nearClipPlane + 10f)), // 上
                cam.ScreenToWorldPoint(new Vector3(Random.Range(0, screenWidth), -100f, cam.nearClipPlane + 10f)), // 下
            };

            // 随机一个方向
            return possiblePoints[Random.Range(0, possiblePoints.Length)];
        }
        
        /// <summary>
        /// 获取动画长度
        /// </summary>
        /// <param name="animator">动画器</param>
        /// <param name="targetName">对应的动画名称</param>
        /// <returns>动画名称的动画时间长度</returns>
        public float GetAnimLength(Animator animator, string targetName)
        {
            RuntimeAnimatorController controller = animator.runtimeAnimatorController;
            for (int i = 0; i < controller.animationClips.Length; i++)
            {
                if (controller.animationClips[i].name == targetName)
                {
                   return controller.animationClips[i].length;
                }
            }
            Debug.LogError("Can't find animation " + targetName);
            return 0;
        }
        
        /// <summary>
        /// 随机获取怪物名称
        /// </summary>
        /// <returns></returns>
        private string RandomGetMonster()
        {
            string[] monsterNames = { "monster1", "monster2", "monster3" };
            int index = Random.Range(0, monsterNames.Length);
            return monsterNames[index];
        }

        /// <summary>
        /// 清理
        /// </summary>
        private void OnDestroy()
        {
            EventManager.Instance.RemoveEventListener(E_EventType.E_Monster_Dead, () =>
            {
                monsterCount--;
            });
            EventManager.Instance.RemoveEventListener(E_EventType.E_Monster_Generate, () =>
            {
                monsterCount++;
            });
        }

        /// <summary>
        /// 返回距离 monster 最近的 Props（道具）Transform
        /// </summary>
        /// <param name="monster">怪物自身</param>
        /// <returns>最近的 Props 的 Transform，没有返回 null</returns>
        public Transform ReturnNearestProps(MonsterAI monster)
        {
            // 查找场景中所有带标签的 Props 对象
            GameObject[] propsList = GameObject.FindGameObjectsWithTag("Player");
            if (propsList.Length == 0)
                return null;

            Transform nearest = null;
            float minDistance = float.MaxValue;
            Vector3 monsterPos = monster.transform.position;

            foreach (GameObject prop in propsList)
            {
                float dist = Vector3.Distance(monsterPos, prop.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = prop.transform;
                }
            }

            return nearest;
        }

    }
}