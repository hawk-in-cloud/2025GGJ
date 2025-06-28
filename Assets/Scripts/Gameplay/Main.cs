using UnityEngine;

namespace Gameplay
{
    public class Main : MonoBehaviour
    {
        public void Start()
        {
            MonsterMgr.Instance.GenerateMonster(10);
        }
    }
}