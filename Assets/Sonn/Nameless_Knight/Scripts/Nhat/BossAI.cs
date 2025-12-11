using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Phát hiện Player")]
     private Transform player;
     private float phamViPhatHienNgang = 40f; // Phạm vi phát hiện theo chiều ngang (X)
     private float phamViPhatHienDoc = 2f; // Phạm vi phát hiện theo chiều dọc (Y)
     private float khoangCachTanCong = 2f; // Khoảng cách để bắt đầu tấn công
     private float khoangCachDungSkill = 5f; // Khoảng cách để bắt đầu dùng skill

    [Header("Di chuyển")]
     private float tocDoChay = 6f; // Tốc độ chạy về phía m_playerTransform

    [Header("Tấn công")]
     private float thoiGianChoiTanCong = 1.5f; // Thời gian chờ giữa các lần tấn công
     private int soLanTanCongTruocSkill = 3; // Số lần tấn công trước khi dùng skill

    [Header("Skill")]
     private float tocDoLaoSkill = 15f; // Tốc độ lao khi dùng skill
     private float khoangCachLaoSkill = 10f; // Khoảng cách lao khi dùng skill

    [Header("Collider other")]
     private BoxCollider2D colliderOther; // Collider khác (như phát hiện chân đất, tường, v.v.)

    [Header("Collider Attack 1")]
     private BoxCollider2D colliderAttack1; // Collider để gây sát thương
     private Vector2 offsetAttack1Phai; // Offset khi hướng phải
     private Vector2 offsetAttack1Trai; // Offset khi hướng trái

    [Header("Collider Attack 2")]
     private BoxCollider2D colliderAttack2; // Collider để gây sát thương
     private Vector2 offsetAttack2Phai; // Offset khi hướng phải
     private Vector2 offsetAttack2Trai; // Offset khi hướng trái

    [Header("Collider Skill")]
     private BoxCollider2D colliderSkill; // Collider để gây sát thương
     private Vector2 offsetSkillPhai; // Offset khi hướng phải
     private Vector2 offsetSkillTrai; // Offset khi hướng trái

    // Trạng thái
    private enum TrangThai
    {
        DungYen,    // Đứng yên
        TruyDuoi,   // Chạy đến m_playerTransform
        TanCong,    // Đang tấn công
        Skill       // Đang dùng skill
    }

    private TrangThai trangThaiHienTai = TrangThai.DungYen;
    private int demSoLanTanCong = 0; // Đếm số lần tấn công
    private float thoiGianTanCongCuoi = -999f;
    private bool dangThucHienSkill = false;
    private bool choPhepLaoSkill = false; // Được kích hoạt bởi Animation Event tại 20%
    private Vector2 viTriBatDauLaoSkill;
    private float huongLaoSkill; // 1 = phải, -1 = trái

    // Components
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool huongPhai = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Tự động tìm m_playerTransform nếu chưa gán
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("m_playerTransform");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Tắt collider ban đầu và lưu offset
        if (colliderAttack1 != null)
        {
            colliderAttack1.enabled = false;
            // Nếu chưa set offset, lấy offset hiện tại
            if (offsetAttack1Phai == Vector2.zero && offsetAttack1Trai == Vector2.zero)
            {
                offsetAttack1Phai = colliderAttack1.offset;
                offsetAttack1Trai = new Vector2(-colliderAttack1.offset.x, colliderAttack1.offset.y);
            }
        }
        if (colliderAttack2 != null)
        {
            colliderAttack2.enabled = false;
            if (offsetAttack2Phai == Vector2.zero && offsetAttack2Trai == Vector2.zero)
            {
                offsetAttack2Phai = colliderAttack2.offset;
                offsetAttack2Trai = new Vector2(-colliderAttack2.offset.x, colliderAttack2.offset.y);
            }
        }
        if (colliderSkill != null)
        {
            colliderSkill.enabled = false;
            if (offsetSkillPhai == Vector2.zero && offsetSkillTrai == Vector2.zero)
            {
                offsetSkillPhai = colliderSkill.offset;
                offsetSkillTrai = new Vector2(-colliderSkill.offset.x, colliderSkill.offset.y);
            }
        }
        if (colliderOther != null)
        {
            colliderOther.enabled = true;
        }
    }

    void Update()
    {
        if (player == null) return;

        switch (trangThaiHienTai)
        {
            case TrangThai.DungYen:
                CapNhatDungYen();
                break;

            case TrangThai.TruyDuoi:
                CapNhatTruyDuoi();
                break;

            case TrangThai.TanCong:
                // Chờ animation tấn công kết thúc
                break;

            case TrangThai.Skill:
                CapNhatSkill();
                break;
        }

        // Cập nhật hướng
        huongPhai = !spriteRenderer.flipX;
    }

    void CapNhatDungYen()
    {
        // Kiểm tra phát hiện theo hình chữ nhật
        float khoangCachNgang = Mathf.Abs(player.position.x - transform.position.x);
        float khoangCachDoc = Mathf.Abs(player.position.y - transform.position.y);
        bool trongVungPhatHien = khoangCachNgang <= phamViPhatHienNgang && khoangCachDoc <= phamViPhatHienDoc;

        // Phát hiện m_playerTransform trong phạm vi
        if (trongVungPhatHien)
        {
            trangThaiHienTai = TrangThai.TruyDuoi;
            animator.SetBool("DangChay", true);
        }
    }

    void CapNhatTruyDuoi()
    {
        float khoangCachNgang = Mathf.Abs(player.position.x - transform.position.x);
        float khoangCachDoc = Mathf.Abs(player.position.y - transform.position.y);
        bool trongVungPhatHien = khoangCachNgang <= phamViPhatHienNgang && khoangCachDoc <= phamViPhatHienDoc;
        float khoangCach = Vector2.Distance(transform.position, player.position);

        // Nếu m_playerTransform ra khỏi phạm vi phát hiện hình chữ nhật
        if (!trongVungPhatHien)
        {
            trangThaiHienTai = TrangThai.DungYen;
            animator.SetBool("DangChay", false);
            return;
        }

        // Kiểm tra xem có dùng skill không
        if (demSoLanTanCong >= soLanTanCongTruocSkill && khoangCach <= khoangCachDungSkill)
        {
            if (Time.time - thoiGianTanCongCuoi >= thoiGianChoiTanCong)
            {
                BatDauSkill();
            }
        }
        // Nếu đến gần m_playerTransform đủ để tấn công
        else if (khoangCach <= khoangCachTanCong)
        {
            // Kiểm tra cooldown tấn công
            if (Time.time - thoiGianTanCongCuoi >= thoiGianChoiTanCong)
            {
                BatDauTanCong();
            }
        }
        else
        {
            // Di chuyển về phía m_playerTransform
            float huongX = Mathf.Sign(player.position.x - transform.position.x);
            transform.Translate(huongX * tocDoChay * Time.deltaTime, 0, 0);

            // Lật sprite theo hướng di chuyển
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

    void BatDauTanCong()
    {
        trangThaiHienTai = TrangThai.TanCong;
        thoiGianTanCongCuoi = Time.time;

        // Chọn ngẫu nhiên attack 1 hoặc 2
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

    void BatDauSkill()
    {
        trangThaiHienTai = TrangThai.Skill;
        dangThucHienSkill = true;
        choPhepLaoSkill = false; // Chưa được phép lao
        demSoLanTanCong = 0; // Reset đếm

        // Lưu vị trí bắt đầu và hướng lao
        viTriBatDauLaoSkill = transform.position;
        huongLaoSkill = player.position.x > transform.position.x ? 1f : -1f;

        // Lật sprite theo hướng m_playerTransform
        spriteRenderer.flipX = huongLaoSkill < 0;

        animator.SetTrigger("Skill");
    }

    void CapNhatSkill()
    {
        if (!dangThucHienSkill) return;

        if (!choPhepLaoSkill) return;

        // Lao về phía m_playerTransform
        float khoangCachDaLao = Mathf.Abs(transform.position.x - viTriBatDauLaoSkill.x);

        if (khoangCachDaLao < khoangCachLaoSkill)
        {
            transform.Translate(huongLaoSkill * tocDoLaoSkill * Time.deltaTime, 0, 0);
        }
    }

    // ===== Animation Events =====
    // Được gọi bởi Animation Event tại 50% của Attack1 và Attack2
    public void KichHoatColliderAttack1()
    {
        if (colliderAttack1 == null) return;

        // Điều chỉnh offset theo hướng
        colliderAttack1.offset = spriteRenderer.flipX ? offsetAttack1Trai : offsetAttack1Phai;
        colliderAttack1.enabled = true;
    }

    public void KichHoatColliderAttack2()
    {
        if (colliderAttack2 == null) return;

        // Điều chỉnh offset theo hướng
        colliderAttack2.offset = spriteRenderer.flipX ? offsetAttack2Trai : offsetAttack2Phai;
        colliderAttack2.enabled = true;
    }

    public void KichHoatColliderSkill()
    {
        if (colliderSkill == null) return;

        // Điều chỉnh offset theo hướng
        colliderSkill.offset = spriteRenderer.flipX ? offsetSkillTrai : offsetSkillPhai;
        colliderOther.enabled = false;
        colliderSkill.enabled = true;
    }

    public void BatDauLaoSkill()
    {
        choPhepLaoSkill = true;
    }

    // Được gọi khi animation Attack1, Attack2 kết thúc
    public void KetThucTanCong()
    {
        TatCollider();
        trangThaiHienTai = TrangThai.DungYen;
    }

    // Được gọi khi animation Skill kết thúc
    public void KetThucSkill()
    {
        TatCollider();
        dangThucHienSkill = false;
        trangThaiHienTai = TrangThai.DungYen;
    }

    void TatCollider()
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

    // Vẽ Gizmos để debug
    void OnDrawGizmosSelected()
    {
        // Vẽ phạm vi phát hiện hình chữ nhật
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        Vector3 topLeft = center + new Vector3(-phamViPhatHienNgang, phamViPhatHienDoc, 0);
        Vector3 topRight = center + new Vector3(phamViPhatHienNgang, phamViPhatHienDoc, 0);
        Vector3 bottomLeft = center + new Vector3(-phamViPhatHienNgang, -phamViPhatHienDoc, 0);
        Vector3 bottomRight = center + new Vector3(phamViPhatHienNgang, -phamViPhatHienDoc, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Vẽ phạm vi tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, khoangCachTanCong);

        // Vẽ đường đến m_playerTransform khi đang truy đuổi
        if (Application.isPlaying && player != null && trangThaiHienTai == TrangThai.TruyDuoi)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // Vẽ khoảng cách lao skill
        if (Application.isPlaying && trangThaiHienTai == TrangThai.Skill)
        {
            Gizmos.color = Color.magenta;
            Vector3 diemCuoi = transform.position + new Vector3(huongLaoSkill * khoangCachLaoSkill, 0, 0);
            Gizmos.DrawLine(transform.position, diemCuoi);
        }
    }
}
