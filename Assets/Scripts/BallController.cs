using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("速度设置")]
    public float minSpeed = 5f; // 最小速度（增加速度）
    public float maxSpeed = 15f; // 最大速度（防止球飞出屏幕）
    
    [Header("碰撞设置")]
    public float deflectionAngle = 10f; // 垂直碰撞时的偏斜角度（度，增加以避免死循环）
    
    [Header("拖尾设置")]
    public bool alwaysShowTrail = false; // 测试选项：始终显示拖尾
    
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private int consecutive90DegreeCollisions = 0; // 连续90度碰撞计数
    private TrailRenderer trailRenderer; // 拖尾组件
    private bool isOnIce = false; // 是否在冰面上
    private CameraShake cameraShake; // 相机震动组件
    private PinballGameManager gameManager;
    private ScoreManager scoreManager;

    // 是否已经被挡板正式击出（用于控制出生时不要自动加速度）
    private bool hasBeenLaunched = false;

    // 记录这颗球上次被挡板击出的时间（由 PinballGameManager 统一管理）
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 创建拖尾组件
        trailRenderer = gameObject.AddComponent<TrailRenderer>();
        SetupTrailRenderer();
        // 默认禁用，只有在冰面上才启用（或测试模式下始终启用）
        trailRenderer.enabled = alwaysShowTrail;
        
        // 查找相机震动组件
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraShake = mainCamera.GetComponent<CameraShake>();
            if (cameraShake == null)
            {
                cameraShake = mainCamera.gameObject.AddComponent<CameraShake>();
            }
        }

        // 获取 GameManager 和 ScoreManager 引用
        gameManager = FindObjectOfType<PinballGameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    // 挡板击出球时由 PaddleController 调用
    public void OnLaunchedFromPaddle()
    {
        hasBeenLaunched = true;
    }

    // 提供只读访问，用于其他脚本判断当前是否已经被击出
    public bool HasBeenLaunched()
    {
        return hasBeenLaunched;
    }
    
    void SetupTrailRenderer()
    {
        if (trailRenderer == null) return;
        // 设置拖尾参数
        trailRenderer.time = 1f; // 拖尾持续时间
        trailRenderer.startWidth = 0.3f; // 起始宽度
        trailRenderer.endWidth = 0.1f; // 结束宽度
        trailRenderer.minVertexDistance = 0.1f; // 最小顶点距离
        trailRenderer.autodestruct = false; // 不自动销毁
        
        // 使用默认材质（不设置材质，使用Unity默认的拖尾材质）
        // 这样可以确保拖尾能正常显示
        
        // 使用Gradient设置拖尾颜色（默认用于冰面&击杀模式）
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(new Color(0.5f, 0.8f, 1f), 0f); // 起始颜色（更亮的蓝色）
        colorKeys[1] = new GradientColorKey(new Color(0.7f, 0.9f, 1f), 1f); // 结束颜色
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f); // 起始透明度（完全不透明）
        alphaKeys[1] = new GradientAlphaKey(0f, 1f); // 结束透明度（渐变消失）
        gradient.SetKeys(colorKeys, alphaKeys);
        trailRenderer.colorGradient = gradient;
        
        // 设置渲染参数
        trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trailRenderer.receiveShadows = false;
        
        // 使用默认材质，不设置自定义材质（避免材质问题）
        trailRenderer.material = null;
    }
    
    // 设置是否在冰面上（由PhysicsZone调用）
    public void SetOnIce(bool onIce)
    {
        isOnIce = onIce;
        if (trailRenderer != null)
        {
            UpdateTrailState();
        }
    }

    void UpdateTrailState()
    {
        if (trailRenderer == null) return;

        bool canDamageEnemies = gameManager != null && gameManager.IsInEnemyHitGraceTime();

        // 测试模式始终显示；否则在冰面上或可击杀敌人时显示拖尾
        bool shouldEnable = alwaysShowTrail || isOnIce || canDamageEnemies;

        if (trailRenderer.enabled != shouldEnable)
        {
            trailRenderer.enabled = shouldEnable;
            if (shouldEnable)
            {
                // 每次重新启用时清一次，让效果更干净
                trailRenderer.Clear();
            }
        }
    }
    
    // 应用重力区域的影响
    void ApplyGravityZones()
    {
        if (rb == null) return;
        
        // 查找所有重力区域
        GravityZone[] gravityZones = FindObjectsOfType<GravityZone>();
        
        Vector3 totalGravityForce = Vector3.zero;
        
        foreach (GravityZone zone in gravityZones)
        {
            if (zone == null) continue;
            
            Vector3 ballPosition = transform.position;
            
            // 检查球是否在重力区域的影响范围内
            if (zone.IsInInfluenceRange(ballPosition))
            {
                // 获取引力方向和强度
                Vector3 gravityDirection = zone.GetGravityDirection(ballPosition);
                float gravityForce = zone.GetGravityForceAtDistance(ballPosition);
                
                // 计算引力向量（在XZ平面上）
                Vector3 gravityVector = new Vector3(
                    gravityDirection.x * gravityForce * Time.fixedDeltaTime,
                    0,
                    gravityDirection.z * gravityForce * Time.fixedDeltaTime
                );
                
                totalGravityForce += gravityVector;
            }
        }
        
        // 应用总引力到球的速度（在XZ平面上）
        if (totalGravityForce.magnitude > 0.01f)
        {
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 flatVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
            Vector3 newVelocity = flatVelocity + totalGravityForce;
            
            // 确保新速度在XZ平面上
            rb.linearVelocity = new Vector3(newVelocity.x, 0, newVelocity.z);
        }
    }
    
    
    void FixedUpdate()
    {
        if (rb == null) return;
        
        // 检查球是否在游戏区域内（Z > -4.5表示还在场地内，考虑开口位置）
        float playAreaMinZ = -4.5f;
        bool isInPlayArea = transform.position.z > playAreaMinZ;
        
        if (isInPlayArea)
        {
            // 只在游戏区域内限制球的高度和速度
            // 限制球的高度（保持在平面上）- 先限制位置，避免碰撞产生Y轴速度
            Vector3 position = transform.position;
            position.y = 0.5f; // 固定高度
            transform.position = position;
            
            // 保存当前速度，用于碰撞检测（在限制Y轴之前）
            lastVelocity = rb.linearVelocity;
            
            // 限制球只在XZ平面上移动（Y轴速度为0）- 强制限制，确保不会飞出
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f; // 强制Y轴速度为0
            rb.linearVelocity = velocity;
            
            // 同时限制角速度的Y轴分量（防止球旋转产生Y轴速度）
            Vector3 angularVelocity = rb.angularVelocity;
            angularVelocity.x = 0f;
            angularVelocity.z = 0f;
            rb.angularVelocity = angularVelocity;
            
            // 处理重力区域的影响
            ApplyGravityZones();
            
            // 重新获取当前速度（应用重力后，只在XZ平面）
            Vector3 currentVelocity = rb.linearVelocity;
            currentVelocity.y = 0f; // 确保Y轴速度为0
            rb.linearVelocity = currentVelocity;
            float currentSpeed = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
            
            // 如果速度低于最小值，增加速度
            if (currentSpeed < minSpeed && currentSpeed > 0.1f)
            {
                // 保持方向，但增加速度到最小值（只在XZ平面）
                Vector3 direction = new Vector3(currentVelocity.x, 0, currentVelocity.z).normalized;
                rb.linearVelocity = new Vector3(direction.x * minSpeed, 0, direction.z * minSpeed);
            }
            
            // 如果速度超过最大值，限制速度
            if (currentSpeed > maxSpeed)
            {
                Vector3 direction = new Vector3(currentVelocity.x, 0, currentVelocity.z).normalized;
                rb.linearVelocity = new Vector3(direction.x * maxSpeed, 0, direction.z * maxSpeed);
            }
            
            // 如果球几乎停止（可能是卡住了），给它一个小的随机速度
            // 只在球已经被挡板击出之后才这样做，避免出生时自动动起来
            if (hasBeenLaunched && currentSpeed < 0.1f)
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    0,
                    Random.Range(0.5f, 1f)
                ).normalized;
                rb.linearVelocity = new Vector3(randomDirection.x * minSpeed, 0, randomDirection.z * minSpeed);
            }

            // 更新拖尾显示状态（包括“可击杀敌人”时间窗口）
            UpdateTrailState();
        }
        else
        {
            // 球已经掉出场地，允许它继续移动（用于触发检测）
            // 保持Y轴位置固定，但允许Z轴继续移动以触发检测器
            Vector3 position = transform.position;
            position.y = 0.5f; // 保持高度固定
            transform.position = position;
            
            // 保存速度用于碰撞检测
            lastVelocity = rb.linearVelocity;
            
            // 限制Y轴速度，但允许X和Z轴继续移动
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (rb == null) return;
        
        // 只有碰到绿色目标时才触发相机震动
        if (IsTarget(collision.gameObject) && cameraShake != null)
        {
            float impactSpeed = new Vector3(lastVelocity.x, 0, lastVelocity.z).magnitude;
            if (impactSpeed < 0.1f)
            {
                impactSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            }
            cameraShake.ShakeByImpact(impactSpeed);
        }

        // 与敌人的特殊交互
        HandleEnemyCollision(collision);
        
        // 只在垂直入射时才进行偏斜处理，其他情况让Unity物理引擎自然处理
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;
            
            // 确保法线在XZ平面上（俯视角）
            normal = new Vector3(normal.x, 0, normal.z).normalized;
            if (normal.magnitude < 0.1f)
            {
                return; // 如果法线无效，退出，让物理引擎处理
            }
            
            // 使用碰撞前的速度（lastVelocity）来计算入射角
            Vector3 incomingDirection = lastVelocity.normalized;
            if (incomingDirection.magnitude < 0.1f)
            {
                // 如果lastVelocity无效，使用当前速度的反方向
                incomingDirection = -rb.linearVelocity.normalized;
            }
            
            // 确保入射方向在XZ平面上
            Vector3 flatIncoming = new Vector3(incomingDirection.x, 0, incomingDirection.z).normalized;
            if (flatIncoming.magnitude < 0.1f)
            {
                return; // 如果方向无效，退出，让物理引擎处理
            }
            
            // 计算入射角（在XZ平面上）
            // 入射角是入射方向与法线的夹角
            // 垂直入射时，入射方向与法线垂直，点积=0，角度=90度
            // 平行入射时，入射方向与法线平行，点积=±1，角度=0或180度
            float dotProduct = Vector3.Dot(flatIncoming, normal);
            float angle = Mathf.Acos(Mathf.Clamp(Mathf.Abs(dotProduct), 0f, 1f)) * Mathf.Rad2Deg;
            
            // 计算反射方向（真实物理反射，在XZ平面上）
            Vector3 reflectDirection = Vector3.Reflect(-flatIncoming, normal);
            reflectDirection = new Vector3(reflectDirection.x, 0, reflectDirection.z).normalized;
            
            // 检查反射方向是否与入射方向太接近（几乎相反，会导致来回弹）
            // 在XZ平面上计算
            float reflectDotIncoming = Vector3.Dot(reflectDirection, -flatIncoming);
            bool isReflectingBack = reflectDotIncoming > 0.7f; // 如果反射方向与入射方向夹角小于45度，认为是垂直反射
            
            // 检测垂直入射（在XZ平面上）
            // 条件1：入射角接近90度（入射方向与法线垂直）
            // 条件2：反射方向与入射方向太接近（会导致来回弹）
            // 条件3：入射方向与法线几乎平行（在俯视角下也是垂直入射到墙面）
            bool isPerpendicularAngle = angle > 80f && angle < 100f; // 入射角接近90度
            bool isParallelAngle = angle < 10f || angle > 170f; // 入射角接近0或180度（平行入射，在俯视角下也是垂直入射）
            bool isVerticalIncident = isPerpendicularAngle || isParallelAngle || isReflectingBack;
            
            // 如果入射角接近90°或垂直反射，强制偏斜
            if (isVerticalIncident)
            {
                consecutive90DegreeCollisions++;
                
                // 如果连续多次90度碰撞，增加偏斜角度避免死循环
                float currentDeflection = deflectionAngle;
                if (consecutive90DegreeCollisions > 1)
                {
                    currentDeflection = deflectionAngle * 2f;
                }
                if (consecutive90DegreeCollisions > 2)
                {
                    currentDeflection = deflectionAngle * 3f;
                }
                if (consecutive90DegreeCollisions > 3)
                {
                    currentDeflection = deflectionAngle * 5f;
                    consecutive90DegreeCollisions = 0;
                }
                
                // 强制最小偏斜角度，确保不会回到原方向
                float minDeflection = 45f; // 最小偏斜45度
                
                // 如果反射方向与入射方向太接近，强制更大的偏斜
                if (isReflectingBack)
                {
                    minDeflection = 60f; // 垂直反射时最小偏斜60度
                    currentDeflection = Mathf.Max(currentDeflection, minDeflection);
                }
                
                // 添加偏斜（随机选择左右偏斜，确保偏斜足够大）
                float deflection = Random.Range(-currentDeflection, currentDeflection);
                
                // 确保偏斜足够大
                if (Mathf.Abs(deflection) < minDeflection)
                {
                    deflection = deflection > 0 ? minDeflection : -minDeflection;
                }
                
                // 在俯视角下，偏斜应该在XZ平面上（绕Y轴旋转）
                Quaternion deflectionRotation = Quaternion.AngleAxis(deflection, Vector3.up);
                Vector3 deflectedDirection = deflectionRotation * reflectDirection;
                
                // 确保方向在XZ平面上
                deflectedDirection = new Vector3(deflectedDirection.x, 0, deflectedDirection.z).normalized;
                
                // 再次检查：如果偏斜后的方向与入射方向还是太接近，强制一个完全不同的方向
                float finalDot = Vector3.Dot(deflectedDirection, -flatIncoming);
                if (finalDot > 0.5f) // 如果还是太接近，强制一个垂直方向
                {
                    // 使用法线的垂直方向作为新方向
                    Vector3 perpendicular = new Vector3(-normal.z, 0, normal.x).normalized;
                    if (perpendicular.magnitude < 0.1f)
                    {
                        perpendicular = new Vector3(normal.z, 0, -normal.x).normalized;
                    }
                    if (perpendicular.magnitude < 0.1f)
                    {
                        // 如果还是无效，使用随机方向
                        perpendicular = new Vector3(
                            Random.Range(-1f, 1f),
                            0,
                            Random.Range(0.5f, 1f)
                        ).normalized;
                    }
                    deflectedDirection = perpendicular;
                }
                
                // 如果方向无效，使用一个随机方向
                if (deflectedDirection.magnitude < 0.1f)
                {
                    deflectedDirection = new Vector3(
                        Random.Range(-1f, 1f),
                        0,
                        Random.Range(0.5f, 1f)
                    ).normalized;
                }
                
                // 保持速度大小，但改变方向（只在XZ平面）
                float currentSpeed = new Vector3(lastVelocity.x, 0, lastVelocity.z).magnitude;
                if (currentSpeed < minSpeed)
                {
                    currentSpeed = minSpeed;
                }
                else if (currentSpeed > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
                
                rb.linearVelocity = new Vector3(deflectedDirection.x * currentSpeed, 0, deflectedDirection.z * currentSpeed);
            }
            else
            {
                // 如果不是垂直入射，让Unity物理引擎自然处理，只重置计数
                consecutive90DegreeCollisions = 0;
                // 确保Y轴速度为0（即使物理引擎处理，也要限制在XZ平面）
                Vector3 currentVel = rb.linearVelocity;
                currentVel.y = 0f;
                rb.linearVelocity = currentVel;
            }
        }
        else
        {
            // 即使没有接触点，也要确保Y轴速度为0
            Vector3 currentVel = rb.linearVelocity;
            currentVel.y = 0f;
            rb.linearVelocity = currentVel;
        }
    }
    
    // 检查是否是目标（绿色板子）
    bool IsTarget(GameObject obj)
    {
        try
        {
            return obj.CompareTag("Target");
        }
        catch
        {
            return obj.name.StartsWith("Target");
        }
    }

    void HandleEnemyCollision(Collision collision)
    {
        GameObject other = collision.gameObject;

        // 使用标签或组件来识别敌人，这里通过组件 EnemyBase 更安全
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy == null)
        {
            // 有可能碰到的是带有碰撞器的子物体，往父级找
            enemy = other.GetComponentInParent<EnemyBase>();
        }

        if (enemy == null)
        {
            return;
        }

        if (gameManager != null && gameManager.IsInEnemyHitGraceTime())
        {
            // 安全时间内：球伤害敌人
            enemy.OnHitByBall(scoreManager);
        }
        else
        {
            // 超出时间：球被敌人吃掉
            enemy.OnEatBall(gameManager, gameObject);
        }
    }
}

