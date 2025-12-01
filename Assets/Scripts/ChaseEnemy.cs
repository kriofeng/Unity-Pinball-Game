using UnityEngine;

/// <summary>
/// 追踪型敌人：当球靠近到一定距离时，短时间追踪球
/// </summary>
public class ChaseEnemy : EnemyBase
{
    [Header("追踪参数")]
    public float detectRadius = 2.5f;   // 中等范围：需要靠近但不是贴脸
    public float chaseSpeed = 1.8f;     // 追逐速度更慢，压迫感更弱
    public float chaseDuration = 2.0f;  // 追逐时间保持适中

    private float chaseTimer = 0f;
    private Transform targetBall;

    void Update()
    {
        // 如果当前没有追踪目标，尝试寻找最近的球
        if (targetBall == null)
        {
            FindBallInRange();
        }

        if (targetBall != null)
        {
            chaseTimer += Time.deltaTime;

            // 移动到球方向（仅XZ平面）
            Vector3 dir = targetBall.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                dir.Normalize();
                transform.position += dir * chaseSpeed * Time.deltaTime;
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            }

            if (chaseTimer >= chaseDuration)
            {
                // 停止追踪，等待下一次靠近再重新开启
                targetBall = null;
                chaseTimer = 0f;
            }
        }
        else
        {
            // 没有目标时，计时器清零
            chaseTimer = 0f;
        }
    }

    void FindBallInRange()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        float closestDistSqr = detectRadius * detectRadius;
        Transform closest = null;

        foreach (GameObject ball in balls)
        {
            Vector3 diff = ball.transform.position - transform.position;
            diff.y = 0f;
            float distSqr = diff.sqrMagnitude;
            if (distSqr <= closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = ball.transform;
            }
        }

        targetBall = closest;
    }
}


