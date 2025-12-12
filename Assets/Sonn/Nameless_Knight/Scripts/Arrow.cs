using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class Arrow : MonoBehaviour
    {
        public int damage;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(Const.ENEMY_TAG))
            {
                var enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            if (collision.gameObject.CompareTag(Const.BOSS_TAG))
            {
                var boss = collision.gameObject.GetComponent<BossAI>();
                if (boss != null)
                {
                    boss.TakeDamage(damage);
                }
            }
        }
    }
}
