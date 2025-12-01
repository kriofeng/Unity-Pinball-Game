using UnityEngine;

/// <summary>
/// 巡逻型敌人：在左半边按圆形路线移动
/// </summary>
public class PatrolEnemy : EnemyBase
{
    [Header("巡逻参数")]
    public Vector3 center = new Vector3(-3f, 0.5f, 0f);
    public float radius = 1.5f;
    public float angularSpeed = 1.5f; // 弧度/秒

    private float angle;

    void Start()
    {
        // 以当前初始位置为中心偏移的起始角度
        Vector3 offset = transform.position - center;
        if (offset.sqrMagnitude > 0.0001f)
        {
            angle = Mathf.Atan2(offset.z, offset.x);
        }
        else
        {
            angle = 0f;
        }
    }

    void Update()
    {
        angle += angularSpeed * Time.deltaTime;
        float x = center.x + Mathf.Cos(angle) * radius;
        float z = center.z + Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, 0.5f, z);
    }
}


