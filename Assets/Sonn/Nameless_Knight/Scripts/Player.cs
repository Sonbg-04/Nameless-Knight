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
        public List<GameObject> enemyLists;

        private GameObject m_treasureChest, m_spawnPlayer;
        private List<GameObject> m_doorLists = new();
        private int m_currentHealth;
        private Vector3 m_originalPos;
        private Animator m_anim;
        private SpriteRenderer m_sp;
        private Rigidbody2D m_rb;
        private float m_shootTimer = 0;
        private bool m_isAttacking = false, m_isGrounded = false;

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
            m_originalPos = transform.position;
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
            var sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex == 1)
            {
                transform.position = m_originalPos;
                m_currentHealth = playerHealth;
            }
            else if (sceneIndex >= 2)
            {
                transform.position = m_spawnPlayer.transform.position;
                m_currentHealth = playerHealth;
            }
            SetStateOnLoadScene();
        }    
        public void SetStateOnLoadScene()
        {
            if (IsComponentNull())
            {
                return;
            }
            m_sp.flipX = false;
            GameManager.Ins.Score = 0;
        }    
        private void OnSceneLoaded(Scene scene, LoadSceneMode lcm)
        {
            if (IsComponentNull())
            {
                return;
            }    
            var sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex == 0)
            {
                if (Ins != null)
                {
                    Destroy(gameObject);
                    Ins = null;
                }
                return;
            }
            else if (sceneIndex >= 1)
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

                if (sceneIndex >= 2)
                {
                    var spawnPoint = GameObject.Find("SpawnPlayer");
                    if (spawnPoint == null)
                    {
                        Debug.LogWarning("Không tìm thấy 'SpawnPlayer' ở scene " + sceneIndex);
                        return;
                    }
                    m_spawnPlayer = spawnPoint;
                    transform.position = spawnPoint.transform.position;
                }

                SearchForGameObject();
            }
            
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
            enemyLists.Clear();
            var enemies = GameObject.FindGameObjectsWithTag(Const.ENEMY_TAG);
            if (enemies == null)
            {
                Debug.LogWarning("Không tìm thấy kẻ thù!");
                return;
            }    
            foreach (var e in enemies)
            {
                enemyLists.Add(e);
            }
            Debug.Log("Đã tìm thấy " + enemyLists.Count + " kẻ thù trong màn này.");
            
            var treasureChest = GameObject.Find("Treasure_chest");
            if (treasureChest == null)
            {
                Debug.LogWarning("Không có GameObject nào có tên Treasure_chest!");
                return;
            }
            m_treasureChest = treasureChest;
            Debug.Log("Đã tìm thấy Treasure_chest!");

            m_doorLists.Clear();
            var doors = GameObject.FindGameObjectsWithTag(Const.DOOR_TAG);
            if (doors == null)
            {
                Debug.LogWarning("Không tìm thấy cánh cửa nào ở màn này!");
                return;
            }
            foreach (var door in doors)
            {
                m_doorLists.Add(door);
            }
            Debug.Log("Đã tìm thấy " + m_doorLists.Count + " cửa ở màn này!");
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
                m_currentHealth--;
                Debug.Log("Player Health: " + m_currentHealth);
                StartCoroutine(HitEffect());
                AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[3]);
                if (m_currentHealth <= 0)
                {
                    GameManager.Ins.GameOver();
                    var sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    if (sceneIndex == 1)
                    {
                        FindObjectOfType<GUIManager_1>()?.ActiveLosegameGUI();
                    }
                    else if (sceneIndex == 2)
                    {
                        FindObjectOfType<GUIManager_2>()?.ActiveLosegameGUI();
                    }    
                }    
            }

            if (collision.gameObject.CompareTag(Const.GENERATING_TOWER_TAG))
            {
                if (enemyLists.Count == 0)
                {
                    foreach (var door in m_doorLists)
                    {
                        Destroy(door);
                    }    
                }
                else
                {
                    Debug.Log("Hãy tiêu diệt tất cả quái rồi mở cửa!");
                }    
            }

            if (collision.gameObject.CompareTag(Const.VICTORY_TAG))
            {
                var sp = m_treasureChest.GetComponent<SpriteRenderer>();
                sp.sprite = treasureChestOpen;
                GameManager.Ins.GameWin();

                var sceneIndex = SceneManager.GetActiveScene().buildIndex;
                if (sceneIndex == 1)
                {
                    FindObjectOfType<GUIManager_1>()?.ActiveWingameGUI();
                }
                else if (sceneIndex == 2)
                {
                    FindObjectOfType<GUIManager_2>()?.ActiveWingameGUI();
                }
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
        }
        public void EnemyDied(GameObject enemy)
        {
            if (!enemy)
            {
                return;
            }
            if (enemyLists.Contains(enemy))
            {
                enemyLists.Remove(enemy);
            }
        }
    }
}
