using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("플레이어 움직임 설정")]
    private Vector2 moveInput = Vector2.zero;
    public float moveSpeed = 6.0f;
    public float sprintSpeed = 8.0f;
    public float jumpPower = 5.0f;
    private bool isJump = false;
    private Rigidbody rb;
    public float sprintInput = 0;
    private bool isSprint = false;
    private float lastSprintTime = 0;
    private float staminaRecoverCooldown = 3.0f;

    [Header("마우스 설정")]
    private Vector2 cameraMoveInput;
    public float mouseSensitivity = 0.2f;
    private float maxRotationY = 88.0f;
    public GameObject playerCameraX;
    public GameObject playerCameraY;

    [Header("플레이어 스테이터스")] 
    private PlayerStatus playerStatus;

    // 플레이어 스테미나 (체력과는 별개)

    public int maxStamina;
    public float stamina;
    // 체력과는 별개로 만들어둡니다 (필요)
    // 기존 플레이어의 경우 체력 = 스테미나라는 구조라고는 하지만 스테미나가 다 닿을때까지 달렸을 때 플레이어가 죽진 않습니다


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerStatus = GetComponent<PlayerStatus>();

        // 마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Start()
    {
        transform.rotation = Quaternion.identity;       // 시작할 때 항상 정면을 보도록 초기화
        isJump = false;

        maxStamina = playerStatus.maxHp;
        stamina = maxStamina;
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();               // WASD 또는 방향키의 입력을 받고 저장한다
    }

    private void OnLook(InputValue value)
    {
        cameraMoveInput += value.Get<Vector2>() * mouseSensitivity;         // 마우스, 또는 게임패드의 움직임을 받고 저장한다
        cameraMoveInput.y = Mathf.Clamp(cameraMoveInput.y, -maxRotationY, maxRotationY);
    }

    private void OnJump(InputValue value)               // 점프 감지
    {
        if(! isJump)
        {
            rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            isJump = true;
        }
    }

    private void OnSprint(InputValue value)
    {
        sprintInput = value.Get<float>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sprintInput > 0)
        {
            if (stamina > 0)
            {
            isSprint = true;
            lastSprintTime = Time.time;
            }
            else
            {
                isSprint = false;
            }
        }
        else
        {
            isSprint = false;
            if (Time.time > staminaRecoverCooldown + lastSprintTime)
            {
                stamina += (Time.deltaTime * 4);
                if(stamina > maxStamina)
                {
                    stamina = maxStamina;
                }    
            }
        }

        if (isSprint)
        {
            transform.Translate(moveInput.x * sprintSpeed * Time.deltaTime, 0, moveInput.y * sprintSpeed * Time.deltaTime);
            stamina -= (Time.deltaTime * 16);
        }
        else
            transform.Translate(moveInput.x * moveSpeed * Time.deltaTime, 0, moveInput.y * moveSpeed * Time.deltaTime);

        playerCameraX.transform.localRotation = Quaternion.AngleAxis(cameraMoveInput.x, Vector3.up);
        playerCameraY.transform.localRotation = Quaternion.AngleAxis(cameraMoveInput.y, Vector3.left);

        if(maxStamina > playerStatus.GetCurrentHP())
        {
            maxStamina = playerStatus.GetCurrentHP();
            if( maxStamina < stamina)
            {
                stamina = maxStamina;
            }
        }

        //Debug.Log("최대 스테미나 : " + maxStamina + " // 현재 스테미나 : " + stamina);
    }
}
