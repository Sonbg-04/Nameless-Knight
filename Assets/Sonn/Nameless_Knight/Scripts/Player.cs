using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class Player : MonoBehaviour, IComponentChecking, ISingleton
    {
        public static Player Ins;

        public LayerMask groundLayer;
        public Transform groundCheckPoint;
        public float speedMovement, speedShoot, shootCooldown, groundCheckRadius;
        public int jumpForce, playerHealth, attackDamage;
        public GameObject arrowPrefab;
        public Sprite treasureChestOpen;
        public Vector3 offsetArrowSpawn;
        public List<Sprite> bloodStates;
        public Image bloodBar;

        private GameObject m_treasureChest, m_spawnPlayer;
        private int m_currentHealth;
        private Animator m_anim;
        private SpriteRenderer m_sp;
        private Rigidbody2D m_rb;
        private float m_shootTimer = 0;
        private bool m_isAttacking = false, m_isGrounded = false, m_isInvincible = false;
        public bool IsInvincible { get => m_isInvincible; }

        private void Awake()
        {
            MakeSingleton();
            m_anim = GetComponent<Animator>();
            m_sp = GetComponent<SpriteRenderer>();
            m_rb = GetComponent<Rigidbody2D>();
        }
        private void Start()
        {
            m_currentHealth = playerHealth;
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        private void Update()
        {
            CheckGround();
            BloodPlayer();
            PlayerMovement();
            PlayerJump();
            PlayerShoot();
        }
        public void MakeSingleton()
        {
            if (Ins == null)
            {
                Ins = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void SetOriginalPos()
        {
            if (IsComponentNull())
            {
                return;
            }
            transform.position = m_spawnPlayer.transform.position;
            GameManager.Ins.Score = 0;
        }    
        public void SetStateOnLoadScene()
        {
            if (IsComponentNull())
            {
                return;
            }
            m_sp.flipX = false;
            m_currentHealth = playerHealth;
            BloodPlayer();
            GameManager.Ins.Score = 0;
            GameManager.Ins.isPlayerDead = false;
        }    
        private void OnSceneLoaded(Scene scene, LoadSceneMode lcm)
        {
            if (IsComponentNull())
            {
                return;
            }    
            if (scene.buildIndex <= 0)
            {
                if (Ins == this)
                {
                    Destroy(gameObject);
                    Ins = null;
                }
                return;
            }
            else
            {
                var blood = GameObject.Find("Blood_bar");
                if (blood == null)
                {
                    Debug.LogWarning("Không tìm thấy gameobject có tên là Blood_bar");
                    return;
                }
                bloodBar = blood.GetComponent<Image>();
                if (bloodBar == null)
                {
                    return;
                }
                if (m_currentHealth >= 0 && m_currentHealth < bloodStates.Count)
                {
                    bloodBar.sprite = bloodStates[m_currentHealth];
                }

                var spawnPoint = GameObject.Find("SpawnPlayer");
                if (spawnPoint == null)
                {
                    Debug.LogWarning("Không tìm thấy 'SpawnPlayer' ở scene " + scene.buildIndex);
                    return;
                }
                m_spawnPlayer = spawnPoint;
                transform.position = spawnPoint.transform.position;

                SearchForGameObject();
            }
            
        }    
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null || m_anim == null 
                       || m_sp == null || m_rb == null
                       || GameManager.Ins == null || GUIManager.Ins == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        private void CheckGround()
        {
            m_isGrounded = Physics2D.OverlapCircle(
                groundCheckPoint.position,
                groundCheckRadius,
                groundLayer
            );
        }
        private void SearchForGameObject()
        {
            var treasureChest = GameObject.Find("Treasure_chest");
            if (treasureChest == null)
            {
                Debug.LogWarning("Không có GameObject nào có tên Treasure_chest!");
                return;
            }
            m_treasureChest = treasureChest;
            Debug.Log("Đã tìm thấy Treasure_chest!");
        }    
        private void BloodPlayer()
        {
            if (bloodBar == null)
            {
                return;
            }
            if (m_currentHealth >= 0 && m_currentHealth < bloodStates.Count)
            {
                bloodBar.sprite = bloodStates[m_currentHealth];
            }
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

            if (Input.GetKeyDown(KeyCode.UpArrow) && m_isGrounded)
            {
                m_rb.velocity = new(m_rb.velocity.x, jumpForce);
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
            var spawnTime = (20f / 15f) * animLength;
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
                PlayerLogic();
            }

            if (collision.gameObject.CompareTag(Const.VICTORY_TAG))
            {
                var sp = m_treasureChest.GetComponent<SpriteRenderer>();
                sp.sprite = treasureChestOpen;
                GameManager.Ins.GameWin();
                if (SceneManager.GetActiveScene().buildIndex == 4)
                {
                    GUIManager.Ins.ActiveFinalGameGUI();
                }
                else
                {
                    GUIManager.Ins.ActiveWingameGUI();
                }
            }
        }
        private void PlayerLogic()
        {
            if (m_isInvincible)
            {
                return;
            }
            m_currentHealth--;
            Debug.Log("Player Health: " + m_currentHealth);
            StartCoroutine(HitEffect());
            AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[3]);
            if (m_currentHealth <= 0)
            {
                GameManager.Ins.GameOver();
                GameManager.Ins.isPlayerDead = true;
                GUIManager.Ins.ActiveLosegameGUI();
            }
        }    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsComponentNull())
            {
                return;
            }
            if (collision.gameObject.CompareTag(Const.HEART_TAG))
            {
                if (m_currentHealth < playerHealth)
                {
                    m_currentHealth++;
                    Debug.Log("Player Health: " + m_currentHealth);
                    AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[6]);
                    Destroy(collision.gameObject);
                }
            }
            if (collision.gameObject.CompareTag(Const.BOSS_TAG))
            {
                PlayerLogic();
            }    
            if (collision.gameObject.CompareTag(Const.DEAD_TAG))
            {
                if (m_isInvincible)
                {
                    return;
                }
                m_currentHealth = 0;
                GameManager.Ins.GameOver();
                GameManager.Ins.isPlayerDead = true;
                GUIManager.Ins.ActiveLosegameGUI();
            }    
        }
        public void SetInvincible(bool enable)
        {
            m_isInvincible = enable;
            Debug.Log($"Player invincibility set to {enable}");
        }
    }
}
