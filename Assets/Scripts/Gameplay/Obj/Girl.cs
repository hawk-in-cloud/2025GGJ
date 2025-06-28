using Gameplay.BaseItem;
using UnityEngine;

namespace Gameplay.Obj
{
    public class Girl : MonoBehaviour
    {
        public int health = 4;
        public void Injured(BaseMonster monster)
        {
            health--;
            
            if (health <= 0)
            {
                Destroy(gameObject);
                
                // TODO: Add game over
            }
        }
    }
}