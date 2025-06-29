using Gameplay.BaseItem;
using UnityEngine;

namespace Gameplay.Obj
{
    public class Girl : MonoBehaviour
    {
        public int health = 4;
        public void Injured(BaseMonster monster)
        {
            Debug.Log($"INJ{health}");
            health--;
            MonsterMgr.Instance.DestroyAllMonsters();
            CircleShockTrigger.Instance.TriggerCircleShock(transform);
            
            if (health <= 0)
            {
                Destroy(gameObject);
                // TODO: Add game over
            }
        }
    }
}