using UnityEngine;
using UnityEngine.AI;

public class MonsterChase : MonoBehaviour
{
    [Header("추적 설정")]
    public Transform player;
    public float chaseRange = 10f;      // 인식 범위
    public float sightAngle = 60f;      // 시야각 (좌우 각도)

    [Header("순찰 설정")]
    public Transform[] waypoints;       // 순찰 지점 배열 (Inspector에서 설정)
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float waypointStopDistance = 0.5f;

    [Header("공격 설정")]
    public float attackRange = 1.5f;    // 공격 가능 거리
    public float attackCooldown = 2f;   // 공격 쿨타임 (초)
    public int attackDamage = 10;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float attackTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        // 웨이포인트가 있으면 첫 번째 목적지 설정
        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        // 공격 쿨타임 감소
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (CanSeePlayer())
        {
            // ── 플레이어 인식됨 → 추적 ──
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);

            // 공격 범위 안에 들어오면 공격
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= attackRange && attackTimer <= 0f)
            {
                Attack();
            }
        }
        else
        {
            // ── 플레이어 미인식 → 순찰 ──
            agent.speed = patrolSpeed;
            Patrol();
        }
    }

    // ── 시야 판정 ──────────────────────────────────────────
    // 1) 거리 체크  2) 시야각 체크  3) Raycast로 벽 체크
    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // 1. 인식 범위 밖이면 바로 false
        if (distance > chaseRange) return false;

        // 2. 시야각 체크 (몬스터 정면 기준)
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > sightAngle) return false;

        // 3. Raycast - 벽이 있으면 false
        // 눈높이 위치에서 플레이어 중심을 향해 쏨
        Vector3 eyePos = transform.position + Vector3.up * 1.5f;
        Vector3 playerPos = player.position + Vector3.up * 1.0f;
        Vector3 rayDir = (playerPos - eyePos).normalized;

        if (Physics.Raycast(eyePos, rayDir, out RaycastHit hit, chaseRange))
        {
            // Ray가 플레이어에 맞아야 인식
            if (hit.transform == player)
                return true;
        }

        return false;
    }

    // ── 순찰 ──────────────────────────────────────────────
    void Patrol()
    {
        if (waypoints.Length == 0) return;

        // 목적지에 거의 도착했으면 다음 웨이포인트로
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= waypointStopDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    // ── 공격 ──────────────────────────────────────────────
    void Attack()
    {
        attackTimer = attackCooldown;   // 쿨타임 시작

        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.TakeDamage(attackDamage);
        }

        Debug.Log("몬스터 공격!");
    }

    // ── Scene 뷰 디버깅용 Gizmo ───────────────────────────
    void OnDrawGizmosSelected()
    {
        // 인식 범위 (빨강)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // 공격 범위 (노랑)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 시야각 선 (파랑)
        Gizmos.color = Color.blue;
        Vector3 leftBound = Quaternion.Euler(0, -sightAngle, 0) * transform.forward * chaseRange;
        Vector3 rightBound = Quaternion.Euler(0, sightAngle, 0) * transform.forward * chaseRange;
        Gizmos.DrawRay(transform.position, leftBound);
        Gizmos.DrawRay(transform.position, rightBound);
    }
}