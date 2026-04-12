using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("플레이어 움직임 설정")]
    private Vector2 moveInput = Vector2.zero;
    public float moveSpeed = 6.0f;
    public float jumpPower = 5.0f;
    private bool isJump = false;
    private Rigidbody rb;

    [Header("마우스 설정")]
    private Vector2 cameraMoveInput;
    public float mouseSensitivity = 0.2f;
    private float maxRotationY = 88.0f;
    public GameObject playerCameraX;
    public GameObject playerCameraY;

    //[Header("플레이어 체력 설정")]
    //public int maxHp = 100;
    //private int currentHp;

    [Header("배고픔 데미지 설정")] //
    public int hungerDamage = 1; // 배고픔 데미지
    public float hungerInterval = 20f; //20초마다 배고픔 닳음

    private PlayerStatus playerStatus;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerStatus = GetComponent<PlayerStatus>();
    }
    void Start()
    {
        transform.rotation = Quaternion.identity;       // 시작할 때 항상 정면을 보도록 초기화
        isJump = false;
        //currentHp = maxHp;

        
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

    //public void TakeDamage(int damage)
    //{
       // currentHp -= damage;

        // 체력 최소값 제한
        //if (currentHp <= 0)
       // {
        //    currentHp = 0;
        //    GameOver();
        //}
   // }
    // 배고픔 데미지
    void HungerDamage()
    {
        playerStatus.TakeDamage(hungerDamage);
    }
   // public void GameOver()
    //{
     //   Debug.Log("게임 오버");
        
     //   Time.timeScale = 0f; // 게임 정지
   // }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }

        if(collision.gameObject.CompareTag("Monster"))
        {
            playerStatus.TakeDamage(10);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveInput.x * moveSpeed * Time.deltaTime, 0, moveInput.y * moveSpeed * Time.deltaTime);

        playerCameraX.transform.localRotation = Quaternion.AngleAxis(cameraMoveInput.x, Vector3.up);
        playerCameraY.transform.localRotation = Quaternion.AngleAxis(cameraMoveInput.y, Vector3.left);


        // rb.AddRelativeForce(moveInput.x * moveSpeed * Time.deltaTime, 0, moveInput.y * moveSpeed * Time.deltaTime);
        // 이동 후 바로 멈추지 않고 미끄러짐

        //if(currentHp <= 0)
        //{
        //    currentHp = 0;
        //    GameOver();
        //}
    }
}
