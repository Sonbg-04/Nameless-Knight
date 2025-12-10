using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class GeneratingTower : MonoBehaviour
    {
        public List<GameObject> doorInZones;
        public List<GameObject> enemyInZones;

        private Animator m_anim;
        private void Awake()
        {
            m_anim = GetComponent<Animator>();
        }
        private void Start()
        {
            foreach (var enemy in enemyInZones)
            {
                enemy.GetComponent<Enemy>().SetTower(this);
            }
        }
        public void EnemyDied(GameObject enemy)
        {
            if (enemy == null)
            {
                return;
            }
            if (enemyInZones.Contains(enemy))
            {
                enemyInZones.Remove(enemy);
            }
        }    
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Const.PLAYER_TAG))
            {
                if (enemyInZones.Count == 0)
                {
                    m_anim.SetBool("Powering", true);
                    foreach (var item in doorInZones)
                    {
                        Destroy(item);
                    }    
                }
                else
                {
                    Debug.Log("Vẫn còn quái trong khu này!");
                }    
            }
        }
    }
}
