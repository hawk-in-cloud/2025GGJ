﻿using System.Collections.Generic;
 using System.Linq;
using Framework;
using Gameplay.BaseItem;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class MonsterMgr : SingletonMono<MonsterMgr>
    {
        
        private Transform _monsterParent;

        private Transform _furParent;
        //所有的怪物
        public List<BaseMonster> allMonsters = new List<BaseMonster>();
        
        void Awake()
        {
            EventManager.Instance.AddEventListener<BaseMonster>(E_EventType.E_Monster_Dead, (monster) =>
            {
                allMonsters.Remove(monster);
            });
            EventManager.Instance.AddEventListener<BaseMonster>(E_EventType.E_Monster_Generate, (monster) =>
            {
                allMonsters.Add(monster);
            });
            EventManager.Instance.AddEventListener(E_EventType.E_GameItem_Dead, () =>
            {
                foreach (var monster in allMonsters)
                {
                    monster.target = ReturnNearestProps(monster);
                }
            });
        }

        void Start()
        {
            _monsterParent = new GameObject("MonsterParent").transform;
            _furParent = new GameObject("FurParent").transform;
        }

        /// <summary>
        /// 生成怪物
        /// </summary>
        /// <param name="count"></param>
        public void GenerateMonster(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var monster = PoolManager.Instance.GetObj("GameObject/Monster", RandomGetMonster());
                monster.transform.position = GetRandomSpawnPosition();
                monster.transform.parent = _monsterParent;
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
            Vector3 randomPoint = possiblePoints[Random.Range(0, possiblePoints.Length)];
            randomPoint.z = 0;
            return randomPoint;
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
        /// 返回距离 monster 最近的 Props（道具）Transform
        /// </summary>
        /// <param name="monster">怪物自身</param>
        /// <returns>最近的 Props 的 Transform，没有返回 null</returns>
        public Transform ReturnNearestProps(BaseMonster monster)
        {
            // 查找子物体中所有带标签的 player的对象
            var propsList = GameObject.FindGameObjectsWithTag("Player");
            
            //查找活跃的
            propsList = propsList.Where(obj =>
            {
                if (obj.GetComponent<Fur_Base>())
                {
                    obj.GetComponent<Fur_Base>().isActive = true;
                    return true;
                }

                if (obj.GetComponent<PlayerController>())
                {
                    return true;
                }
                return false;
            }).ToArray();
            //加入player
            var propsListNew = new List<GameObject>(propsList);
            propsListNew.Add(GameObject.Find("Player"));
            propsList = propsListNew.ToArray();
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