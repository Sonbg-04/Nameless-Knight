using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class Enemy : MonoBehaviour, IComponentChecking
    {
        public float start, end, speedMovement, groundCheckDistance;
        public int health;
        public LayerMask groundLayer;
        public Transform groundCheckPoint;

        private Animator m_anim;
        private SpriteRenderer m_sR;
        private int m_currentHealth;
        private bool m_isMovingRight = true;

        private void Awake()
        {
            m_sR = GetComponent<SpriteRenderer>();
            m_anim = GetComponent<Animator>();
            m_currentHealth = health;
        }
        private void Update()
        {
            if (IsComponentNull())
            {
                return;
            }    
            EnemyMovement();
        }
        public bool IsComponentNull()
        {
            bool check = GameManager.Ins == null || Player.Ins == null ||
                         m_anim == null || m_sR == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        private void EnemyMovement()
        {
            var dir = m_isMovingRight ? 1 : -1;
            var newPosX = transform.position.x + speedMovement * dir * Time.deltaTime;
            Vector3 checkPos = new (newPosX, transform.position.y, transform.position.z);
            var hasGround = Physics2D.Raycast(
                groundCheckPoint.position + new Vector3(dir * 0.3f, 0, 0),
                Vector2.down,
                groundCheckDistance,
                groundLayer);

            if (!hasGround)
            {
                m_isMovingRight = !m_isMovingRight;
                return;
            }
            if (newPosX >= end)
            {
                newPosX = end;
                m_isMovingRight = false;
            }
            else if (newPosX <= start)
            {
                newPosX = start;
                m_isMovingRight = true;
            }
            m_sR.flipX = !m_isMovingRight;
            transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
        }
        public void TakeDamage(int damage)
        {
            m_currentHealth -= damage;
            StartCoroutine(HitEffect());
            if (m_currentHealth <= 0)
            {
                Die();
            }    
        }
        IEnumerator HitEffect()
        {
            m_sR.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            m_sR.color = Color.white;
        }    
        private void Die()
        {
            Player.Ins.EnemyDied(gameObject);
            m_anim.SetBool("Dead", true);
            Destroy(gameObject, 0.5f);
            GameManager.Ins.Score++;
        }    
    }
}
