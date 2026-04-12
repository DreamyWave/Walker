using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHp = 100;
    private int currentHp;

    [Header("배고픔 설정")]
    public int hungerDamage = 1;
    public float hungerInterval = 5f;

    [Header("카메라 흔들림 설정")] 
    public Transform playerCamera; // 카메라 연결
    public float shakeAmount = 0.02f; // 흔들림 강도
    public float shakeSpeed = 5f; // 흔들림 속도

    private Vector3 originalPos; // 원래 위치 저장

    private void Start()
    {
        currentHp = maxHp;

        // 카메라 원래 위치 저장
        originalPos = playerCamera.localPosition;

        // 배고픔 시스템 시작
        InvokeRepeating("HungerDamage", hungerInterval, hungerInterval);
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        Debug.Log("현재 체력 : " + currentHp); 

        if (currentHp <= 0)
        {
            currentHp = 0;
            GameOver();
        }
    }

    // 배고픔 데미지
    void HungerDamage()
    {
        TakeDamage(hungerDamage);
    }

    void Update()
    {
        // 체력 40 이하일 때 흔들림
        if (currentHp <= 40 && currentHp > 0)
        {
            CameraShake();
        }
        else
        {
            // 체력 회복시 원래 위치로
            playerCamera.localPosition = originalPos;
        }
    }

    void CameraShake() // 흔들림 함수
    {
        float x = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        float y = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

        playerCamera.localPosition = originalPos + new Vector3(x, y, 0);
    }

    void GameOver()
    {
        Debug.Log("플레이어 사망");

        // 게임 멈춤
        Time.timeScale = 0f;
    }
}