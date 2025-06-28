using Gameplay.BaseItem;

namespace Gameplay.Monsters
{
    public class Monster2 : BaseMonster
    {
        protected override void Init()
        {
            print("Monster2 Init");
            Invoke("Die", 0.5f);
        }
    }
}