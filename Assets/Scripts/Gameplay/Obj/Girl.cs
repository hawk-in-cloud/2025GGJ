using DG.Tweening;
using Framework;
using Gameplay.BaseItem;
using UnityEngine;

namespace Gameplay.Obj
{
    public class Girl : MonoBehaviour
    {
        public int health = 4;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        public Sprite idle;
        public Sprite inj;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
                _originalColor = _spriteRenderer.color;
        }
        public void Injured(BaseMonster monster)
        {
            Debug.Log($"INJ{health}");
            health--;
            MonsterMgr.Instance.DestroyAllMonsters();
            CircleShockTrigger.Instance.TriggerCircleShock(transform);
            MonsterMgr.Instance.StopAllMonsters();
            EventManager.Instance.EventTrigger(E_EventType.E_GirlInjured);

            _spriteRenderer.sprite = inj;

            // ÅòÕÍ¶¯»­
            transform.DOScale(1.3f, 0.1f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() =>
                     {
                         transform.DOScale(1f, 0.15f)
                                  .SetEase(Ease.InQuad);
                     });

            // ºìÉ«ÉÁË¸¶¯»­
            if (_spriteRenderer != null)
            {
                _spriteRenderer.DOKill(); // ±ÜÃâÑÕÉ«¶¯»­µþ¼Ó
                _spriteRenderer.color = _originalColor;

                _spriteRenderer.DOColor(Color.red, 0.05f)
                               .SetEase(Ease.OutQuad)
                               .OnComplete(() =>
                               {
                                   _spriteRenderer.DOColor(_originalColor, 0.1f)
                                                  .SetEase(Ease.InQuad);
                                   _spriteRenderer.sprite = idle;
                               });
            }

            if (health <= 0)
            {
                Destroy(gameObject);
                // TODO: Add game over
            }
        }
    }
}