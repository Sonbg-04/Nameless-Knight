using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BayNgauNhienVaTruyDuoi : MonoBehaviour
{
    [Header("Khu vực bay ngẫu nhiên")]
    [SerializeField] private Vector2 tamKhuVuc = Vector2.zero; // Tâm khu vực bay (để trống = vị trí ban đầu)
    [SerializeField] private float chieuRongKhuVuc = 5f; // Chiều rộng khu vực (trục X)
    [SerializeField] private float chieuCaoKhuVuc = 3f; // Chiều cao khu vực (trục Y)
    [SerializeField] private float tocDoBay = 2f; // Tốc độ bay
    [SerializeField] private float khoangCachDungDoi = 0.3f; // Khoảng cách để coi như đã đến đích

    [Header("Truy đuổi")]
    [SerializeField] private Transform player; // Nhân vật cần truy đuổi
    [SerializeField] private float phamViPhatHien = 8f; // Phạm vi phát hiện
    [SerializeField] private float tocDoTruyDuoi = 4f; // Tốc độ truy đuổi
    [SerializeField] private float khoangCachDung = 0f; // Khoảng cách dừng lại khi đến gần player
    [SerializeField] private float doCaoBayLen = 4f; // Độ cao bay lên so với player

    [Header("Tấn công")]
    [SerializeField] private float khoangCachKichHoatTanCong = 7f; // Khoảng cách để bắt đầu tấn công
    [SerializeField] private float tocDoLao = 12f; // Tốc độ lao xuống khi tấn công
    [SerializeField] private string tenAnimationTanCong = "DangTanCong"; // Tên animation tấn công
    [SerializeField] private float thoiGianHoiTanCong = 2f; // Thời gian chờ giữa các lần tấn công

    private Vector2 diemDich; // Điểm đích hiện tại khi bay ngẫu nhiên
    private bool dangTruyDuoi = false;
    private bool dangTanCong = false; // Đang trong trạng thái tấn công
    private bool daBayLen = false; // Đã bay lên cao hơn player chưa
    private bool daTiepCanNgang = false; // Đã tiếp cận gần player theo phương ngang chưa
    private float thoiGianTanCongCuoi = -999f; // Thời điểm tấn công lần cuối
    private Vector2 viTriMucTieuTanCong; // Vị trí của player khi bắt đầu tấn công
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool huongPhai = true;
    private Vector2 viTriBanDau;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        viTriBanDau = new Vector2(transform.position.x, transform.position.y);

        // Nếu không set tâm khu vực, dùng vị trí ban đầu
        if (tamKhuVuc == Vector2.zero)
        {
            tamKhuVuc = viTriBanDau;
        }

        // Tự động tìm player nếu chưa gán
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Chọn điểm đích đầu tiên
        ChonDiemDichNgauNhien();
    }

    void Update()
    {
        // Nếu đang tấn công, tiếp tục tấn công bất kể dangTruyDuoi
        if (dangTanCong)
        {
            ThucHienTanCong();
        }
        else
        {
            KiemTraTruyDuoi();

            if (dangTruyDuoi)
            {
                TruyDuoiPlayer();
            }
            else
            {
                // Reset trạng thái khi không còn truy đuổi
                daBayLen = false;
                daTiepCanNgang = false;

                // Kiểm tra xem có đang ở ngoài khu vực bay không
                Vector2 viTriHienTai = new Vector2(transform.position.x, transform.position.y);
                float khoangCachRaKhoi = Vector2.Distance(viTriHienTai, tamKhuVuc);

                if (khoangCachRaKhoi > (Mathf.Max(chieuRongKhuVuc, chieuCaoKhuVuc) / 2f))
                {
                    QuayVeKhuVuc();
                }
                else
                {
                    BayNgauNhien();
                }
            }
        }

        // Cập nhật hướng dựa trên sprite
        huongPhai = !spriteRenderer.flipX;

        // Cập nhật animation state
        if (animator != null && !string.IsNullOrEmpty(tenAnimationTanCong))
        {
            animator.SetBool(tenAnimationTanCong, dangTanCong);
        }
    }

    void KiemTraTruyDuoi()
    {
        if (player == null)
        {
            dangTruyDuoi = false;
            return;
        }

        // Tính khoảng cách tổng thể (2D)
        Vector2 viTriHienTai = new Vector2(transform.position.x, transform.position.y);
        Vector2 viTriPlayer = new Vector2(player.position.x, player.position.y);
        float khoangCach = Vector2.Distance(viTriHienTai, viTriPlayer);

        // Kiểm tra hướng của player so với quái
        float hinhChieuX = player.position.x - transform.position.x;
        bool playerOBenPhai = hinhChieuX > 0;
        bool dangHuongVePlayer = (huongPhai && playerOBenPhai) || (!huongPhai && !playerOBenPhai);

        // Kiểm tra khoảng cách ra khỏi khu vực bay
        float khoangCachRaKhoi = Vector2.Distance(viTriHienTai, tamKhuVuc);

        // Phát hiện player nếu: trong phạm vi + đang hướng về player
        if (khoangCach <= phamViPhatHien && dangHuongVePlayer)
        {
            dangTruyDuoi = true;
        }
        // Dừng truy đuổi nếu đã ra quá xa khỏi khu vực bay
        else
        {
            dangTruyDuoi = false;
        }
    }

    void TruyDuoiPlayer()
    {
        Vector2 viTriHienTai = new Vector2(transform.position.x, transform.position.y);
        Vector2 viTriPlayer = new Vector2(player.position.x, player.position.y);

        // Bước 1: Bay lên cao hơn player
        if (!daBayLen)
        {
            float doCaoMucTieu = player.position.y + doCaoBayLen;

            if (transform.position.y < doCaoMucTieu)
            {
                // Bay lên
                Vector2 huongBayLen = new Vector2(0, 1); // Bay thẳng lên
                transform.position += new Vector3(huongBayLen.x, huongBayLen.y, 0) * tocDoTruyDuoi * Time.deltaTime;
            }
            else
            {
                daBayLen = true;
            }
        }
        // Bước 2: Tiếp cận player theo phương ngang
        if (!daTiepCanNgang)
        {
            float khoangCach = Vector2.Distance(viTriHienTai, viTriPlayer);

            if (khoangCach > khoangCachKichHoatTanCong)
            {
                // Di chuyển theo phương ngang về phía player
                float huongX = Mathf.Sign(player.position.x - transform.position.x);
                transform.position += new Vector3(huongX, 0, 0) * tocDoTruyDuoi * Time.deltaTime;

                // Lật sprite theo hướng di chuyển
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = huongX < 0;
                }
            }
            else
            {
                daTiepCanNgang = true;
            }
        }
        // Bước 3: Kiểm tra điều kiện tấn công
        if (daBayLen && daTiepCanNgang)
        {
            float khoangCach = Vector2.Distance(viTriHienTai, viTriPlayer);

            // Kiểm tra xem đã đủ thời gian chờ giữa các lần tấn công chưa
            if (khoangCach <= khoangCachKichHoatTanCong &&
                Time.time - thoiGianTanCongCuoi >= thoiGianHoiTanCong)
            {
                // Kích hoạt tấn công - lưu vị trí player lúc này
                dangTanCong = true;
                thoiGianTanCongCuoi = Time.time;
                viTriMucTieuTanCong = new Vector2(player.position.x, player.position.y - 0.5f); // Hạ thấp chút để tấn công chính xác hơn
            }
            else
            {
                // Nếu xa quá, reset lại trạng thái để bay lên và tiếp cận lại
                daBayLen = false;
                daTiepCanNgang = false;
                dangTanCong = false;
            }
        }
    }

    void ThucHienTanCong()
    {
        // Lao nhanh về phía player
        Vector2 viTriHienTai = new Vector2(transform.position.x, transform.position.y);
        Vector2 huongLao = (viTriMucTieuTanCong - viTriHienTai).normalized;

        transform.position += new Vector3(huongLao.x, huongLao.y, 0) * tocDoLao * Time.deltaTime;

        // Lật sprite theo hướng lao
        if (spriteRenderer != null && huongLao.x != 0)
        {
            spriteRenderer.flipX = huongLao.x < 0;
        }

        // Kiểm tra nếu đã đến vị trí mục tiêu
        float khoangCach = Vector2.Distance(viTriHienTai, viTriMucTieuTanCong);
        if (khoangCach <= khoangCachDung)
        {
            // Kết thúc tấn công, reset để chuẩn bị tấn công lần sau
            dangTanCong = false;
            daBayLen = false;
            daTiepCanNgang = false;
        }
    }

    void BayNgauNhien()
    {
        // Di chuyển về phía điểm đích
        Vector2 viTriHienTai = new Vector2(transform.position.x, transform.position.y);
        float khoangCach = Vector2.Distance(viTriHienTai, diemDich);

        if (khoangCach > khoangCachDungDoi)
        {
            // Di chuyển
            Vector2 huongDi = (diemDich - viTriHienTai).normalized;
            transform.position += new Vector3(huongDi.x, huongDi.y, 0) * tocDoBay * Time.deltaTime;

            // Lật sprite theo hướng bay
            if (spriteRenderer != null && huongDi.x != 0)
            {
                spriteRenderer.flipX = huongDi.x < 0;
            }
        }
        else
        {
            // Đã đến đích, chọn điểm mới ngay lập tức
            ChonDiemDichNgauNhien();
        }
    }

    void ChonDiemDichNgauNhien()
    {
        // Tạo điểm ngẫu nhiên trong khu vực 2D
        // Thu nhỏ vùng chọn xuống 70% để tránh góc và cạnh
        float tyLeAnToan = 0.9f;
        float randomX = Random.Range(-chieuRongKhuVuc * tyLeAnToan / 2f, chieuRongKhuVuc * tyLeAnToan / 2f);
        float randomY = Random.Range(-chieuCaoKhuVuc * tyLeAnToan / 2f, chieuCaoKhuVuc * tyLeAnToan / 2f);

        diemDich = tamKhuVuc + new Vector2(randomX, randomY);
    }

    void QuayVeKhuVuc()
    {
        // Di chuyển về tâm khu vực bay
        Vector2 viTriHienTai = new Vector2(transform.position.x, transform.position.y);
        Vector2 huongVe = (tamKhuVuc - viTriHienTai).normalized;

        transform.position += new Vector3(huongVe.x, huongVe.y, 0) * tocDoBay * Time.deltaTime;

        // Lật sprite
        if (spriteRenderer != null && huongVe.x != 0)
        {
            spriteRenderer.flipX = huongVe.x < 0;
        }

        // Khi về đến gần khu vực, chọn điểm đích mới
        if (Vector2.Distance(viTriHienTai, tamKhuVuc) < chieuRongKhuVuc / 2f)
        {
            ChonDiemDichNgauNhien();
        }
    }

    // Vẽ khu vực trong Scene view
    void OnDrawGizmosSelected()
    {
        Vector2 tam = (tamKhuVuc == Vector2.zero && !Application.isPlaying) ? new Vector2(transform.position.x, transform.position.y) : tamKhuVuc;

        // Vẽ hình chữ nhật khu vực bay (2D)
        Gizmos.color = Color.green;
        Vector3 tam3D = new Vector3(tam.x, tam.y, transform.position.z);

        // Vẽ 4 cạnh của hình chữ nhật
        Vector3 topLeft = tam3D + new Vector3(-chieuRongKhuVuc / 2f, chieuCaoKhuVuc / 2f, 0);
        Vector3 topRight = tam3D + new Vector3(chieuRongKhuVuc / 2f, chieuCaoKhuVuc / 2f, 0);
        Vector3 bottomLeft = tam3D + new Vector3(-chieuRongKhuVuc / 2f, -chieuCaoKhuVuc / 2f, 0);
        Vector3 bottomRight = tam3D + new Vector3(chieuRongKhuVuc / 2f, -chieuCaoKhuVuc / 2f, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, phamViPhatHien);

        // Vẽ phạm vi phát hiện
        if (Application.isPlaying)
        {
            // Vẽ điểm đích hiện tại khi bay ngẫu nhiên
            if (!dangTruyDuoi)
            {
                Gizmos.color = Color.yellow;
                Vector3 diemDich3D = new Vector3(diemDich.x, diemDich.y, transform.position.z);
                Gizmos.DrawWireSphere(diemDich3D, 0.2f);
                Gizmos.DrawLine(transform.position, diemDich3D);
            }

            // Vẽ đường đến player nếu đang truy đuổi
            if (dangTruyDuoi && player != null)
            {
                if (dangTanCong)
                {
                    // Khi đang tấn công, vẽ đường đến vị trí mục tiêu cố định
                    Gizmos.color = Color.red;
                    Vector3 viTriMucTieu = new Vector3(viTriMucTieuTanCong.x, viTriMucTieuTanCong.y, transform.position.z);
                    Gizmos.DrawLine(transform.position, viTriMucTieu);
                    Gizmos.DrawWireSphere(viTriMucTieu, 0.5f); // Vẽ điểm mục tiêu
                }
                else
                {
                    // Khi đang truy đuổi, vẽ đường đến player
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(transform.position, player.position);
                }

                // Vẽ vùng kích hoạt tấn công
                Gizmos.color = Color.yellow;
                Vector3 playerPos = new Vector3(player.position.x, player.position.y, transform.position.z);
                Gizmos.DrawWireSphere(playerPos, khoangCachKichHoatTanCong);

                // Vẽ điểm mục tiêu bay lên
                if (!daBayLen)
                {
                    Gizmos.color = Color.blue;
                    Vector3 diemBayLen = new Vector3(transform.position.x, player.position.y + doCaoBayLen, transform.position.z);
                    Gizmos.DrawWireSphere(diemBayLen, 0.3f);
                }
            }
        }
    }
}
