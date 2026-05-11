using UnityEngine;

// Monster 추적 하려고 만든 player 코드입니다. 삭제 하지 말아주세요
public class PlayerMove : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    [Header("HP / 무적 설정")]
    public int maxHP = 100;
    public float invincibleDuration = 1.5f;  // 피격 후 무적 지속 시간 (초)

    private Rigidbody rb;
    private Camera cam;
    private float xRotation = 0f;

    private int currentHP;
    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        currentHP = maxHP;

        // 마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ── 무적 타이머 ──────────────────────────────────
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                Debug.Log("무적 해제");
            }
        }

        // ── 마우스 시점 ───────────────────────────────────
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    void FixedUpdate()
    {
        // ── WASD 이동 ──────────────────────────────────
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * h + transform.forward * v) * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

    // ── 피격 처리 (MonsterChase에서 호출) ─────────────────
    public void TakeDamage(int damage)
    {
        // 무적 중이면 데미지 무시
        if (isInvincible)
        {
            Debug.Log("무적 상태 - 데미지 무시");
            return;
        }

        currentHP -= damage;
        Debug.Log($"플레이어 피격! 남은 HP: {currentHP}/{maxHP}");

        // 무적 시간 시작
        isInvincible = true;
        invincibleTimer = invincibleDuration;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망! 게임 오버");
        // TODO: 게임 오버 UI 표시 or 씬 재로드
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ── 외부에서 상태 확인용 ──────────────────────────────
    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => maxHP;
    public bool IsInvincible() => isInvincible;
}