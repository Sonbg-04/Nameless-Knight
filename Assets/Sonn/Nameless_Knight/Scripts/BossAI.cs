using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class BossAI : MonoBehaviour
    {
        public GameObject door;
        private Transform player;
        private float phamViPhatHienNgang = 40f;
        private float phamViPhatHienDoc = 2f;
        private float khoangCachTanCong = 2f; 
        private float khoangCachDungSkill = 5f;

        private float tocDoChay = 6f; 

        private float thoiGianChoiTanCong = 1.5f; 
        private int soLanTanCongTruocSkill = 3;

        private float tocDoLaoSkill = 15f; 
        private float khoangCachLaoSkill = 10f; 

        public BoxCollider2D colliderOther;

        public BoxCollider2D colliderAttack1; 
        private Vector2 offsetAttack1Phai; 
        private Vector2 offsetAttack1Trai; 

        public BoxCollider2D colliderAttack2; 
        private Vector2 offsetAttack2Phai; 
        private Vector2 offsetAttack2Trai; 

        public BoxCollider2D colliderSkill; 
        private Vector2 offsetSkillPhai; 
        private Vector2 offsetSkillTrai; 

        private int health = 100; 
        public HealthBar healthBar; 

        private TrangThai trangThaiHienTai = TrangThai.DungYen;
        private int demSoLanTanCong = 0;
        private float thoiGianTanCongCuoi = -999f;
        private bool dangThucHienSkill = false;
        private bool choPhepLaoSkill = false; 
        private Vector2 viTriBatDauLaoSkill;
        private float huongLaoSkill;
        private int currentHealth;

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private bool huongPhai = true;

        private void Start()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentHealth = health;

            if (healthBar != null)
            {
                healthBar.SetTarget(transform);
                healthBar.SetMaxHealth(health);
            }

            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag(Const.PLAYER_TAG);
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }

            if (colliderAttack1 != null)
            {
                colliderAttack1.enabled = false;
                offsetAttack1Phai = colliderAttack1.offset;
                offsetAttack1Trai = new Vector2(-colliderAttack1.offset.x, colliderAttack1.offset.y);
            }
            if (colliderAttack2 != null)
            {
                colliderAttack2.enabled = false;
                offsetAttack2Phai = colliderAttack2.offset;
                offsetAttack2Trai = new Vector2(-colliderAttack2.offset.x, colliderAttack2.offset.y);
            }
            if (colliderSkill != null)
            {
                colliderSkill.enabled = false;
                offsetSkillPhai = colliderSkill.offset;
                offsetSkillTrai = new Vector2(-colliderSkill.offset.x, colliderSkill.offset.y);
            }
            if (colliderOther != null)
            {
                colliderOther.enabled = true;
            }
        }
        private void Update()
        {
            if (player == null)
            {
                return;
            }

            switch (trangThaiHienTai)
            {
                case TrangThai.DungYen:
                    CapNhatDungYen();
                    break;

                case TrangThai.TruyDuoi:
                    CapNhatTruyDuoi();
                    break;

                case TrangThai.TanCong:
                    break;

                case TrangThai.Skill:
                    CapNhatSkill();
                    break;
            }

            huongPhai = !spriteRenderer.flipX;
        }

        private void CapNhatDungYen()
        {
            float khoangCachNgang = Mathf.Abs(player.position.x - transform.position.x);
            float khoangCachDoc = Mathf.Abs(player.position.y - transform.position.y);
            bool trongVungPhatHien = khoangCachNgang <= phamViPhatHienNgang && khoangCachDoc <= phamViPhatHienDoc;

            if (trongVungPhatHien)
            {
                trangThaiHienTai = TrangThai.TruyDuoi;
                animator.SetBool("Run", true);
            }
        }

        private void CapNhatTruyDuoi()
        {
            float khoangCachNgang = Mathf.Abs(player.position.x - transform.position.x);
            float khoangCachDoc = Mathf.Abs(player.position.y - transform.position.y);
            bool trongVungPhatHien = khoangCachNgang <= phamViPhatHienNgang && khoangCachDoc <= phamViPhatHienDoc;
            float khoangCach = Vector2.Distance(transform.position, player.position);

            if (!trongVungPhatHien)
            {
                trangThaiHienTai = TrangThai.DungYen;
                animator.SetBool("Run", false);
                return;
            }
            if (demSoLanTanCong >= soLanTanCongTruocSkill && khoangCach <= khoangCachDungSkill)
            {
                if (Time.time - thoiGianTanCongCuoi >= thoiGianChoiTanCong)
                {
                    BatDauSkill();
                }
            }
            else if (khoangCach <= khoangCachTanCong)
            {
                if (Time.time - thoiGianTanCongCuoi >= thoiGianChoiTanCong)
                {
                    BatDauTanCong();
                }
            }
            else
            {
                float huongX = Mathf.Sign(player.position.x - transform.position.x);
                transform.Translate(huongX * tocDoChay * Time.deltaTime, 0, 0);

                if (huongX > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (huongX < 0)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
        private void BatDauTanCong()
        {
            trangThaiHienTai = TrangThai.TanCong;
            thoiGianTanCongCuoi = Time.time;

            int attackNgauNhien = Random.Range(1, 3);

            if (attackNgauNhien == 1)
            {
                animator.SetTrigger("Attack1");
            }
            else
            {
                animator.SetTrigger("Attack2");
            }

            demSoLanTanCong++;
        }
        private void BatDauSkill()
        {
            trangThaiHienTai = TrangThai.Skill;
            dangThucHienSkill = true;
            choPhepLaoSkill = false;
            demSoLanTanCong = 0; 

            viTriBatDauLaoSkill = transform.position;
            huongLaoSkill = player.position.x > transform.position.x ? 1f : -1f;

            spriteRenderer.flipX = huongLaoSkill < 0;

            animator.SetTrigger("Skill");
        }
        void CapNhatSkill()
        {
            if (!dangThucHienSkill)
            {
                return;
            }
            if (!choPhepLaoSkill)
            {
                return;
            }
            float khoangCachDaLao = Mathf.Abs(transform.position.x - viTriBatDauLaoSkill.x);

            if (khoangCachDaLao < khoangCachLaoSkill)
            {
                transform.Translate(huongLaoSkill * tocDoLaoSkill * Time.deltaTime, 0, 0);
            }
        }
        public void KichHoatColliderAttack1()
        {
            if (colliderAttack1 == null)
            {
                return;
            }
            colliderAttack1.offset = spriteRenderer.flipX ? offsetAttack1Trai : offsetAttack1Phai;
            colliderAttack1.enabled = true;
        }

        public void KichHoatColliderAttack2()
        {
            if (colliderAttack2 == null)
            {
                return;
            }

            colliderAttack2.offset = spriteRenderer.flipX ? offsetAttack2Trai : offsetAttack2Phai;
            colliderAttack2.enabled = true;
        }
        public void KichHoatColliderSkill()
        {
            if (colliderSkill == null)
            {
                return;
            }
            colliderSkill.offset = spriteRenderer.flipX ? offsetSkillTrai : offsetSkillPhai;
            colliderOther.enabled = false;
            colliderSkill.enabled = true;
        }

        public void BatDauLaoSkill()
        {
            choPhepLaoSkill = true;
        }

        public void KetThucTanCong()
        {
            TatCollider();
            trangThaiHienTai = TrangThai.DungYen;
        }
        public void KetThucSkill()
        {
            TatCollider();
            dangThucHienSkill = false;
            trangThaiHienTai = TrangThai.DungYen;
        }

        private void TatCollider()
        {
            if (colliderAttack1 != null)
            {
                colliderAttack1.enabled = false;
            }
            if (colliderAttack2 != null)
            {
                colliderAttack2.enabled = false;
            }
            if (colliderSkill != null)
            {
                colliderSkill.enabled = false;
            }
            if (colliderOther != null)
            {
                colliderOther.enabled = true;
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            StartCoroutine(HitEffect());
            Debug.Log("Boss Health: " + currentHealth);

            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        IEnumerator HitEffect()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }

        private void Die()
        {
            TatCollider();

            if (healthBar != null)
            {
                healthBar.Hide();
            }

            trangThaiHienTai = TrangThai.DungYen;
            animator.SetBool("Run", false);

            animator.SetTrigger("Die");
            Destroy(door, 0.5f);
            Debug.Log("Boss đã chết!");

            Destroy(gameObject, 1.0f);
        }

    }
}
