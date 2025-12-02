using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class Enemy : MonoBehaviour
    {
        public float start, end, speedMovement,
                     speedChase, chaseRange, stopChaseRange,
                     groundCheckDistance;
        public int health;
        public Transform player;
        public LayerMask groundLayer;
        public Vector2 groundCheckOffset;

        private Animator m_anim;
        private SpriteRenderer m_sR;
        private int m_currentHealth;
        private bool m_isMovingRight = true, m_isChasing = false, m_isReturning = false;
        private Vector3 m_originalPos;

        private void Awake()
        {
            m_sR = GetComponent<SpriteRenderer>();
            m_anim = GetComponent<Animator>();
            m_currentHealth = health;
            m_originalPos = transform.position;
        }
        private void Update()
        {
            ChasePlayer();
        }
        private void ChasePlayer()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= chaseRange)
            {
                m_isChasing = true;
                m_isReturning = false;
            }
            else if (distanceToPlayer >= stopChaseRange)
            {
                m_isChasing = false;
                m_isReturning = true;
            }
            if (m_isChasing)
            {
                var Dir = player.position.x > transform.position.x ? 1 : -1;
                if (IsGrounded(Dir))
                {
                    EnemyChasing(Dir);
                }
                else
                {
                    m_isChasing = false;
                    m_isReturning = true;
                }    
            }
            else if (m_isReturning)
            {
                ReturnToOriginalPosition();
            }
            else
            {
                EnemyMovement();
            } 
                
        }    
        private void EnemyMovement()
        {
            var dir = m_isMovingRight ? 1 : -1;
            if (!IsGrounded(dir))
            {
                m_isMovingRight = !m_isMovingRight;
                dir = m_isMovingRight ? 1 : -1;
            }
            var newPosX = transform.position.x + (speedMovement * dir) * Time.deltaTime;
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
        private void EnemyChasing(int direction)
        {
            transform.position += new Vector3(direction * speedChase * Time.deltaTime, 0, 0);
            m_sR.flipX = direction < 0;
        }    
        private void ReturnToOriginalPosition()
        {
            if (Vector2.Distance(transform.position, m_originalPos) > 0.1f)
            {
                var direction = m_originalPos.x > transform.position.x ? 1 : -1;
                if (IsGrounded(direction))
                {
                    transform.position += new Vector3(direction * speedMovement * Time.deltaTime, 0, 0);
                    m_sR.flipX = direction < 0;
                }
                else
                {
                    m_isReturning = false;
                }    
            }
            else
            {
                m_isReturning = false;
            }
        }
        private bool IsGrounded(int dir)
        {
            Vector2 origin = (Vector2)transform.position + new Vector2(groundCheckOffset.x * dir, groundCheckOffset.y);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
            return hit.collider != null;
        }    
        public void TakeDamage(int damage)
        {
            m_currentHealth -= damage;
            StartCoroutine(HitEffect());
            if (m_currentHealth < 0)
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
            Destroy(gameObject);
        }    
    }
}
