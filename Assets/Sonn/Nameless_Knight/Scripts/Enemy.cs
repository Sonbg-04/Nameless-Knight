using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class Enemy : MonoBehaviour, IComponentChecking
    {
        public float start, end, speedMovement;
        public int health;
        public Transform groundCheckPoint;
        public bool isBeeEnemy, isBoarEnemy;

        private float m_groundCheckDistance = 0.2f;
        private GeneratingTower m_generatingTower;
        private Animator m_anim;
        private SpriteRenderer m_sR;
        private int m_currentHealth;
        private bool m_isMovingRight = true;
        private Transform m_playerTransform;
        private Vector2 m_areaCenter = Vector2.zero, m_randomTargetPos, 
                        m_startPosition, m_attackTargetPos;
        private float m_areaWidth = 5f, m_areaHeight = 3f,
                      m_stopDistance = 0.3f, m_detectRange = 8f, m_chaseSpeed = 4f,
                      m_attackStopDistance = 0.3f, m_verticalRise = 3f, m_attackTriggerDistance = 7f,
                      m_attackDiveSpeed = 12f, m_attackCooldown = 2f, m_lastAttackTime = -999f,
                      m_horizontalDetectionRange = 5f, m_verticalDetectionRange = 0.5f;
        private bool m_isChasing = false, m_isAttacking = false, 
                     m_hasReachedVertical = false, m_hasReachedHorizontal = false;

        private void Awake()
        {
            m_sR = GetComponent<SpriteRenderer>();
            m_anim = GetComponent<Animator>();
            m_currentHealth = health;
        }

        private void Start()
        {
            SearchGameObj();
            BeeInit();
            if (isBeeEnemy)
            {
                groundCheckPoint = null;
            }    
        }
        private void Update()
        {
            if (IsComponentNull())
            {
                return;
            }

            if (!isBeeEnemy)
            {
                EnemyAttackState();
            }
            else
            {
                BeeAttackState();
            }
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

        private void BeeInit()
        {
            if (isBeeEnemy)
            {
                m_startPosition = transform.position;
                m_areaCenter = m_startPosition;
                SelectPointRandom();
            }
        }    
        private void BeeAttackState()
        {
            if (m_isAttacking)
            {
                AttackOfBee();
            }
            else
            {
                CheckPlayerChasing();

                if (m_isChasing)
                {
                    ChasingPlayer();
                }
                else
                {
                    m_hasReachedVertical = false;
                    m_hasReachedHorizontal = false;

                    Vector2 pos = transform.position;
                    float distanceOutsideArea = Vector2.Distance(pos, m_areaCenter);

                    if (distanceOutsideArea > Mathf.Max(m_areaWidth, m_areaHeight) / 2f)
                    {
                        ReturnToArea();
                    }
                    else
                    {
                        FlyBee();
                    }
                }
            }
            m_anim.SetBool("Attacking", m_isAttacking);
        }
        private void EnemyAttackState()
        {
            if (!m_isAttacking)
            {
                CheckPlayerChasing();

                if (m_isChasing)
                {
                    ChasingPlayer();
                }
                else
                {
                    if (transform.position.x < start || transform.position.x > end)
                    {
                        ReturnToArea();
                    }
                    else
                    {
                        EnemyMovement();
                    }
                }
            }
            if (isBoarEnemy)
            {
                m_anim.SetBool("Attacking", m_isAttacking);
            }
        }
        private void CheckPlayerChasing()
        {
            if (m_playerTransform == null)
            {
                m_isChasing = false;
                return;
            }

            if (isBeeEnemy)
            {
                var distance = Vector2.Distance(transform.position, m_playerTransform.position);
                if (distance <= m_detectRange)
                {
                    m_isChasing = true;
                }
                else
                {
                    m_isChasing = false;
                }
            }
            else
            {
                var playerCol = m_playerTransform.GetComponent<Collider2D>();
                var enemyCol = GetComponent<Collider2D>();
                var p = playerCol ? playerCol.bounds.center : m_playerTransform.position;
                var e = enemyCol ? enemyCol.bounds.center : transform.position;
                var horizontalDist = Mathf.Abs(p.x - e.x);
                var verticalDist = Mathf.Abs(p.y - e.y);
                var playerMovingRight = (p.x - e.x) > 0;
                var directionToPlayer = (m_isMovingRight && playerMovingRight) || (!m_isMovingRight && !playerMovingRight);
                var checkPlayerInArea = p.x >= start && p.x <= end;
                var playerInZoneEnemy = horizontalDist <= m_horizontalDetectionRange 
                                      && verticalDist <= m_verticalDetectionRange;    
                m_isChasing = playerInZoneEnemy && checkPlayerInArea && directionToPlayer;
            }    
        }
        private void ChasingPlayer()
        {
            var dist = Vector2.Distance(transform.position, m_playerTransform.position);
            if (isBeeEnemy)
            {
                if (!m_hasReachedVertical)
                {
                    var targetY = m_playerTransform.position.y + m_verticalRise;

                    if (transform.position.y < targetY)
                    {
                        transform.position += m_chaseSpeed * Time.deltaTime * Vector3.up;
                    }
                    else
                    {
                        m_hasReachedVertical = true;
                    }
                }

                if (!m_hasReachedHorizontal)
                {
                    if (dist > m_attackTriggerDistance)
                    {
                        var dirX = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
                        transform.position += Time.deltaTime * m_chaseSpeed * new Vector3(dirX, 0, 0);
                        m_sR.flipX = dirX < 0;
                    }
                    else
                    {
                        m_hasReachedHorizontal = true;
                    }
                }

                if (m_hasReachedVertical && m_hasReachedHorizontal)
                {
                    if (dist <= m_attackTriggerDistance &&
                        Time.time - m_lastAttackTime >= m_attackCooldown)
                    {
                        m_isAttacking = true;
                        m_lastAttackTime = Time.time;
                        m_attackTargetPos = new Vector2(m_playerTransform.position.x, m_playerTransform.position.y - 0.5f);
                    }
                    else
                    {
                        m_isAttacking = false;
                    }
                }
            }
            else
            {
                if (dist > m_stopDistance)
                {
                    var DirHorizontal = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
                    transform.position += new Vector3(DirHorizontal * m_chaseSpeed * Time.deltaTime, 0, 0);
                    m_sR.flipX = DirHorizontal < 0;
                }    
            }    
        }
        private void ResetBeeState()
        {
            m_isAttacking = false;
            m_hasReachedVertical = false;
            m_hasReachedHorizontal = false;
            m_anim.SetBool("Attacking", false);
        }
        private void AttackOfBee()
        {
            Vector2 pos = transform.position;
            Vector2 dir = (m_attackTargetPos - pos).normalized;

            transform.position += (Vector3)(Time.deltaTime * m_attackDiveSpeed * dir);

            if (dir.x != 0)
            {
                m_sR.flipX = dir.x < 0;
            }

            float dist = Vector2.Distance(pos, m_attackTargetPos);
            if (dist <= m_attackStopDistance)
            {
                ResetBeeState();
            }
        }
        private void FlyBee()
        {
            Vector2 pos = transform.position;
            float dist = Vector2.Distance(pos, m_randomTargetPos);

            if (dist > m_stopDistance)
            {
                Vector2 dir = (m_randomTargetPos - pos).normalized;
                transform.position += (Vector3)(Time.deltaTime * speedMovement * dir);

                if (dir.x != 0)
                {
                    m_sR.flipX = dir.x < 0;
                }
            }
            else
            {
                SelectPointRandom();
            }
        }
        private void SelectPointRandom()
        {
            float safe = 0.9f;
            float x = Random.Range(-m_areaWidth * safe / 2f, m_areaWidth * safe / 2f);
            float y = Random.Range(-m_areaHeight * safe / 2f, m_areaHeight * safe / 2f);

            m_randomTargetPos = m_areaCenter + new Vector2(x, y);
        }
        private void ReturnToArea()
        {
            if (isBeeEnemy)
            {
                Vector2 pos = transform.position;
                Vector2 dir = (m_areaCenter - pos).normalized;

                transform.position += (Vector3)(Time.deltaTime * speedMovement * dir);

                if (dir.x != 0)
                {
                    m_sR.flipX = dir.x < 0;
                }
                if (Vector2.Distance(pos, m_areaCenter) < m_areaWidth / 2f)
                {
                    SelectPointRandom();
                }
            }
            else
            {
                var target = transform.position.x < start ? start : end;
                m_isMovingRight = transform.position.x < start;
                var dir = Mathf.Sign(target - transform.position.x);
                transform.position += new Vector3(dir * speedMovement * Time.deltaTime, 0, 0);
                m_sR.flipX = dir < 0;
            }    
        }
        private void SearchGameObj()
        {
            var p = GameObject.FindGameObjectWithTag(Const.PLAYER_TAG);
            if (p)
            {
                m_playerTransform = p.transform;
            }
        }
        public void SetTower(GeneratingTower tower)
        {
            m_generatingTower = tower;
        }
        private void EnemyMovement()
        {
            var dir = m_isMovingRight ? 1 : -1;
            var newPosX = transform.position.x + speedMovement * dir * Time.deltaTime;

            var hasGround = Physics2D.Raycast(
                groundCheckPoint.position + new Vector3(dir * 0.3f, 0, 0),
                Vector2.down,
                m_groundCheckDistance,
                LayerMask.GetMask(Const.GROUND_LAYER));

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
            if (IsComponentNull())
            {
                return;
            }
            m_generatingTower.EnemyDied(gameObject);
            m_anim.SetBool("Dead", true);
            Destroy(gameObject, 0.5f);
            GameManager.Ins.Score++;
        }
    }
}
