using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class Player : MonoBehaviour, IComponentChecking
    {
        public float speedMovement, speedShoot, shootCooldown;
        public int jumpForce, playerHealth, attackDamage;
        public GUIManager_1 canvas_1;
        public GameObject door, treasureChest, arrowPrefab;
        public Sprite treasureChestOpen;
        public Vector3 offsetArrowSpawn;

        private Animator m_anim;
        private SpriteRenderer m_sp;
        private Rigidbody2D m_rb;
        private float m_shootTimer = 0;
        private bool m_isAttacking = false;

        private void Awake()
        {
            m_anim = GetComponent<Animator>();
            m_sp = GetComponent<SpriteRenderer>();
            m_rb = GetComponent<Rigidbody2D>();
        }
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null || m_anim == null 
                       || m_sp == null || m_rb == null
                       || GameManager.Ins == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        private void Update()
        {
            PlayerMovement();
            PlayerJump();
            PlayerShoot();
        }
        private void PlayerMovement()
        {
            if (IsComponentNull())
            {
                return;
            }
            var moveX = Input.GetAxisRaw("Horizontal");
            var velocity = m_rb.velocity;
            velocity.x = moveX * speedMovement;
            m_rb.velocity = velocity;
            if (moveX != 0)
            {
                m_sp.flipX = moveX < 0;
                m_anim.SetBool("Running", true);
            }
            else
            {
                m_anim.SetBool("Running", false);
            }
        }    
        private void PlayerJump()
        {
            if (IsComponentNull())
            {
                return;
            }    
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                m_rb.velocity = new (m_rb.velocity.x, jumpForce);
            }    
        }
        private void PlayerShoot()
        {
            m_shootTimer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.T) && m_shootTimer >= shootCooldown)
            {
                m_shootTimer = 0;
                if (!m_isAttacking)
                {
                    StartCoroutine(ShootingArrow());
                }    
            }    
        }   
        private void ShootArrow()
        {
            if (IsComponentNull())
            {
                return;
            }
            Vector3 firePoint = transform.position + offsetArrowSpawn;
            var arrow = Instantiate(arrowPrefab, firePoint, Quaternion.identity);
            var arrowCom = arrow.GetComponent<Arrow>();
            if (arrowCom)
            {
                arrowCom.damage = attackDamage;
            }
            var rb_arrow = arrow.GetComponent<Rigidbody2D>();
            var direction = m_sp.flipX ? -1f : 1f;
            rb_arrow.velocity = new(direction * speedShoot, 0);
            var arrow_sp = arrow.GetComponent<SpriteRenderer>();
            if (!arrow_sp)
            {
                return;
            }
            arrow_sp.flipX = m_sp.flipX;
            Destroy(arrow, 1f);
            AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[0]);
        }   
        IEnumerator ShootingArrow()
        {
            m_isAttacking = true;
            m_anim.SetBool("Attacking", true);
            var animLength = m_anim.GetCurrentAnimatorStateInfo(0).length;
            var spawnTime = (11f / 12f) * animLength;
            yield return new WaitForSeconds(spawnTime);
            ShootArrow();
            yield return new WaitForSeconds(animLength - spawnTime);
            m_anim.SetBool("Attacking", false);
            m_isAttacking = false;
        }    
        IEnumerator HitEffect()
        {
            m_sp.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            m_sp.color = Color.white;
        }    
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsComponentNull())
            {
                return;
            }
            if (collision.gameObject.CompareTag(Const.ENEMY_TAG))
            {
                playerHealth--;
                Debug.Log("Player Health: " + playerHealth);
                StartCoroutine(HitEffect());
                AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[3]);
                if (playerHealth == 0)
                {
                    GameManager.Ins.GameOver();
                    canvas_1.ActiveLosegameGUI();
                }    
            }
            if (collision.gameObject.CompareTag(Const.GENERATING_TOWER_TAG))
            {
                Destroy(door);
            }    
            if (collision.gameObject.CompareTag(Const.VICTORY_TAG))
            {
                var sp = treasureChest.GetComponent<SpriteRenderer>();
                sp.sprite = treasureChestOpen;
                GameManager.Ins.GameWin();
                canvas_1.ActiveWingameGUI();
            }    
            if (collision.gameObject.CompareTag(Const.HEART_TAG))
            {
                if (playerHealth < 5)
                {
                    playerHealth++;
                    Debug.Log("Player Health: " + playerHealth);
                    AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[6]);
                    Destroy(collision.gameObject);
                }    
            }
        }
    }
}
