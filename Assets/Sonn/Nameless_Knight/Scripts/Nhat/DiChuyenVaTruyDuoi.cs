using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiChuyenVaTruyDuoi : MonoBehaviour
{
    [Header("Di chuyển tuần hoàn")]
    public float start; // Điểm bắt đầu tuần tra
    public float end; // Điểm kết thúc tuần tra
    public float speedTuanTra = 1f; // Tốc độ tuần tra
    private bool movingRight = true; // Hướng di chuyển tuần tra

    [Header("Truy đuổi")]
    [SerializeField] private Transform player; // Nhân vật cần truy đuổi
    [SerializeField] private string tenAnimationTruyDuoi = "DangTruyDuoi"; // Tên animation khi truy đuổi
    [SerializeField] private float phamViPhatHienNgang = 5f; // Phạm vi phát hiện theo chiều ngang (X)
    [SerializeField] private float phamViPhatHienDoc = 0.5f; // Phạm vi phát hiện theo chiều dọc (Y)
    [SerializeField] private float tocDoTruyDuoi = 3f; // Tốc độ truy đuổi
    [SerializeField] private float khoangCachDung = 0.5f; // Khoảng cách dừng

    private bool dangTruyDuoi = false;
    private SpriteRenderer spriteRenderer;
    private bool huongPhai = true;
    private Animator animator;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Tự động tìm player nếu chưa gán
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    private void Update()
    {
        KiemTraTruyDuoi();

        if (dangTruyDuoi)
        {
            TruyDuoiPlayer();
        }
        else
        {
            // Kiểm tra xem có đang ở ngoài phạm vi tuần tra không
            if (transform.position.x < start || transform.position.x > end)
            {
                QuayVePhamViTuanTra();
            }
            else
            {
                DiChuyenTuanTra();
            }
        }

        // Cập nhật hướng dựa trên sprite
        huongPhai = !spriteRenderer.flipX;

        // Cập nhật animation state
        if (animator != null && !string.IsNullOrEmpty(tenAnimationTruyDuoi))
        {
            animator.SetBool(tenAnimationTruyDuoi, dangTruyDuoi);
        }
    }

    void KiemTraTruyDuoi()
    {
        if (player == null)
        {
            dangTruyDuoi = false;
            return;
        }

        // Lấy bounds của player (từ Collider)
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        Collider2D quaiCollider = GetComponent<Collider2D>();

        Vector3 playerCenter = player.position;
        Vector3 quaiCenter = transform.position;

        // Nếu có collider, dùng center của bounds
        if (playerCollider != null)
        {
            playerCenter = playerCollider.bounds.center;
        }
        if (quaiCollider != null)
        {
            quaiCenter = quaiCollider.bounds.center;
        }

        // Tính khoảng cách theo phương ngang và dọc (từ center của hitbox)
        float khoangCachNgang = Mathf.Abs(playerCenter.x - quaiCenter.x);
        float khoangCachDoc = Mathf.Abs(playerCenter.y - quaiCenter.y);

        // Kiểm tra hướng của player so với quái
        float hinhChieuX = playerCenter.x - quaiCenter.x;
        bool playerOBenPhai = hinhChieuX > 0;
        bool dangHuongVePlayer = (huongPhai && playerOBenPhai) || (!huongPhai && !playerOBenPhai);

        // Kiểm tra player có trong phạm vi tuần tra không
        bool playerTrongPhamViTuanTra = playerCenter.x >= start && playerCenter.x <= end;

        // Kiểm tra player có trong vùng phát hiện hình chữ nhật không
        bool trongVungPhatHien = khoangCachNgang <= phamViPhatHienNgang && khoangCachDoc <= phamViPhatHienDoc;

        // Phát hiện player nếu: trong vùng hình chữ nhật + đang hướng về player + player trong phạm vi tuần tra
        if (trongVungPhatHien && dangHuongVePlayer && playerTrongPhamViTuanTra)
        {
            dangTruyDuoi = true;
        }
        else
        {
            dangTruyDuoi = false;
        }
    }

    void TruyDuoiPlayer()
    {
        float khoangCachNgang = Mathf.Abs(player.position.x - transform.position.x);

        if (khoangCachNgang > khoangCachDung)
        {
            // CHỈ di chuyển theo trục X (ngang), GIỮ NGUYÊN trục Y
            float huongX = Mathf.Sign(player.position.x - transform.position.x);
            float newX = transform.position.x + huongX * tocDoTruyDuoi * Time.deltaTime;

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            // Lật sprite theo hướng di chuyển
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = huongX < 0;
            }
        }
    }

    void DiChuyenTuanTra()
    {
        // Tính toán vị trí mới dựa trên vị trí hiện tại, tốc độ và thời gian
        float newPosition = transform.position.x + (speedTuanTra * (movingRight ? 1 : -1) * Time.deltaTime);

        // Kiểm tra xem đối tượng đã đạt đến các điểm cuối chưa
        if (newPosition >= end)
        {
            newPosition = end;
            movingRight = false;
            spriteRenderer.flipX = true;
        }
        else if (newPosition <= start)
        {
            newPosition = start;
            movingRight = true;
            spriteRenderer.flipX = false;
        }

        // Cập nhật vị trí mới cho đối tượng
        transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
    }

    void QuayVePhamViTuanTra()
    {
        // Xác định điểm gần nhất trong phạm vi tuần tra
        float diemGanNhat;
        if (transform.position.x < start)
        {
            diemGanNhat = start;
            movingRight = true;
        }
        else
        {
            diemGanNhat = end;
            movingRight = false;
        }

        // Di chuyển về điểm gần nhất
        float huongVe = Mathf.Sign(diemGanNhat - transform.position.x);
        float newPosition = transform.position.x + huongVe * speedTuanTra * Time.deltaTime;

        // Lật sprite
        spriteRenderer.flipX = huongVe < 0;

        // Cập nhật vị trí
        transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
    }

    // Vẽ phạm vi trong Scene view
    void OnDrawGizmosSelected()
    {
        // Vẽ phạm vi tuần tra
        Gizmos.color = Color.green;
        Vector3 startPos = new Vector3(start, transform.position.y, transform.position.z);
        Vector3 endPos = new Vector3(end, transform.position.y, transform.position.z);
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireSphere(startPos, 0.2f);
        Gizmos.DrawWireSphere(endPos, 0.2f);

        // Vẽ hình chữ nhật phạm vi phát hiện
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

        // Vẽ phạm vi phát hiện hình chữ nhật
        if (Application.isPlaying)
        {
            // Vẽ đường đến player nếu đang truy đuổi
            if (dangTruyDuoi && player != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }
}
