using UnityEngine;

public class PinballGameSetup : MonoBehaviour
{
    [Header("游戏设置")]
    public Material normalMaterial;
    public Material iceMaterial;
    public Material targetMaterial;
    public Material paddleMaterial;
    public Material ballMaterial;
    
    private ScoreManager scoreManager;
    private UIManager uiManager;
    private PinballGameManager gameManager;
    private float paddleZPosition; // 记录挡板的Z位置，方便在挡板上生成球

    [Header("敌人设置")]
    public float enemyRespawnDelay = 3f; // 敌人被击杀后复活的延迟时间
    
    void Start()
    {
        // 设置相机
        SetupCamera();
        
        // 创建游戏管理器
        CreateGameManager();
        
        // 创建UI管理器
        CreateUIManager();
        
        // 创建计分管理器
        CreateScoreManager();
        
        // 创建弹球台
        CreatePinballTable();
        
        // 创建边界墙（包括开口）
        CreateWalls();
        
        // 创建球掉出检测区域
        CreateBallOutDetector();
        
        // 创建挡板
        CreatePaddles();
        
        // 创建目标区域
        CreateTargets();
        
        // 创建物理区域（正常区域和冰面区域）
        CreatePhysicsZones();

        // 创建敌人
        CreateEnemies();
        
        // 创建球
        CreateBall();
    }
    
    void CreateGameManager()
    {
        GameObject gameManagerObj = new GameObject("GameManager");
        gameManager = gameManagerObj.AddComponent<PinballGameManager>();
    }
    
    void SetupCamera()
    {
        // 查找或创建主相机
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        
        // 设置相机位置和角度（从正上方俯视弹球台）
        mainCamera.transform.position = new Vector3(0, 6, 0); // 进一步降低相机高度
        mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0); // 90度俯视
        
        // 设置相机参数（使用正交相机，俯视角更适合）
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 4f; // 视野大小（减小数值让相机更近）
        mainCamera.nearClipPlane = 0.3f;
        mainCamera.farClipPlane = 100f;
        
        // 设置背景颜色
        mainCamera.backgroundColor = new Color(0.2f, 0.2f, 0.3f);
        
        // 添加相机震动组件
        if (mainCamera.GetComponent<CameraShake>() == null)
        {
            mainCamera.gameObject.AddComponent<CameraShake>();
        }
    }
    
    void CreateUIManager()
    {
        GameObject uiObj = new GameObject("UIManager");
        uiManager = uiObj.AddComponent<UIManager>();
    }
    
    void CreateScoreManager()
    {
        GameObject scoreObj = new GameObject("ScoreManager");
        scoreManager = scoreObj.AddComponent<ScoreManager>();
    }
    
    void CreatePinballTable()
    {
        // 创建弹球台（地面）
        GameObject table = GameObject.CreatePrimitive(PrimitiveType.Plane);
        table.name = "PinballTable";
        table.transform.position = new Vector3(0, 0, 0);
        table.transform.localScale = new Vector3(2, 1, 2);
        
        // 设置材质（使用兼容的内置着色器）
        Renderer renderer = table.GetComponent<Renderer>();
        if (normalMaterial == null)
        {
            // 使用Legacy Diffuse着色器，确保打包时可用
            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            if (shader != null)
            {
                normalMaterial = new Material(shader);
                normalMaterial.color = Color.gray;
            }
            else
            {
                Debug.LogError("无法找到可用的着色器来创建弹球台材质！");
            }
        }
        if (normalMaterial != null)
        {
            renderer.material = normalMaterial;
        }
        
        // 添加物理材质（低摩擦，让球更容易移动）
        PhysicsMaterial normalPhysic = new PhysicsMaterial("NormalMaterial");
        normalPhysic.dynamicFriction = 0.2f; // 降低摩擦
        normalPhysic.staticFriction = 0.2f; // 降低摩擦
        normalPhysic.bounciness = 0.5f; // 增加弹性
        table.GetComponent<Collider>().material = normalPhysic;
    }
    
    void CreateBall()
    {
        GameObject ball = CreateNewBall();
        if (gameManager != null && ball != null)
        {
            gameManager.SetCurrentBall(ball);
        }
    }
    
    public GameObject CreateNewBall()
    {
        // 创建球（在挡板附近出生，初始无速度）
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball";
        SetTagSafely(ball, "Ball");

        // 在右挡板正上方、略微往场内偏一点的位置出生
        // 右挡板中心：x = 1.3, z = paddleZPosition
        float spawnX = 1.3f;
        float spawnZ = paddleZPosition + 0.15f;
        ball.transform.position = new Vector3(spawnX, 0.5f, spawnZ);
        ball.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        
        // 设置材质（使用兼容的内置着色器）
        Renderer renderer = ball.GetComponent<Renderer>();
        if (ballMaterial == null)
        {
            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            if (shader != null)
            {
                ballMaterial = new Material(shader);
                ballMaterial.color = Color.red;
            }
            else
            {
                Debug.LogError("无法找到可用的着色器来创建球材质！");
            }
        }
        if (ballMaterial != null)
        {
            renderer.material = ballMaterial;
        }
        
        // 添加刚体
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 0.5f; // 减小质量，让球更容易被推动
        rb.useGravity = false; // 禁用重力，球只在平面上移动
        rb.linearDamping = 0.1f; // 降低阻力，让球更容易移动
        rb.angularDamping = 0.1f; // 降低角阻力
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 连续动态碰撞检测，确保与Kinematic刚体碰撞正确
        rb.interpolation = RigidbodyInterpolation.Interpolate; // 插值，使移动更平滑
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 冻结Y轴位置和X、Z轴旋转
        
        // 添加物理材质（高弹性）
        PhysicsMaterial ballPhysic = new PhysicsMaterial("BallMaterial");
        ballPhysic.dynamicFriction = 0.05f; // 降低摩擦
        ballPhysic.staticFriction = 0.05f; // 降低摩擦
        ballPhysic.bounciness = 0.9f; // 增加弹性
        ball.GetComponent<Collider>().material = ballPhysic;
        
        // 球初始向下移动（朝向挡板和开口）
        // 现在不再给初速度，等待玩家用挡板击球
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // 添加球控制器，保持最小速度
        ball.AddComponent<BallController>();
        
        return ball;
    }
    
    void CreatePaddles()
    {
        float playAreaSizeZ = 4f;
        float paddleZ = -playAreaSizeZ + 0.5f; // 挡板位置靠近下边墙（后墙），在开口上方
        paddleZPosition = paddleZ;
        
        // 创建左挡板（水平放置，靠近下边墙）
        GameObject leftPaddle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftPaddle.name = "LeftPaddle";
        leftPaddle.transform.position = new Vector3(-1.3f, 0.3f, paddleZ); // 在开口左侧
        leftPaddle.transform.localScale = new Vector3(1.5f, 1f, .3f); // 水平板子（长宽高）
        leftPaddle.transform.rotation = Quaternion.Euler(0, 0, 0); // 水平放置
        
        Renderer leftRenderer = leftPaddle.GetComponent<Renderer>();
        if (paddleMaterial == null)
        {
            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            if (shader != null)
            {
                paddleMaterial = new Material(shader);
                paddleMaterial.color = Color.blue;
            }
            else
            {
                Debug.LogError("无法找到可用的着色器来创建挡板材质！");
            }
        }
        if (paddleMaterial != null)
        {
            leftRenderer.material = paddleMaterial;
        }
        
        // 确保碰撞器正确设置
        Collider leftCollider = leftPaddle.GetComponent<Collider>();
        if (leftCollider == null)
        {
            leftCollider = leftPaddle.AddComponent<BoxCollider>();
        }
        leftCollider.isTrigger = false; // 确保不是触发器
        
        // 添加物理材质，确保球能碰撞
        PhysicsMaterial paddlePhysic = new PhysicsMaterial("PaddleMaterial");
        paddlePhysic.dynamicFriction = 0.1f;
        paddlePhysic.staticFriction = 0.1f;
        paddlePhysic.bounciness = 0.8f; // 高弹性
        leftCollider.material = paddlePhysic;
        
        // 使用Kinematic刚体，确保碰撞检测正确
        Rigidbody leftRb = leftPaddle.AddComponent<Rigidbody>();
        leftRb.isKinematic = true; // 改为Kinematic，使用transform移动
        leftRb.useGravity = false;
        leftRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 连续动态碰撞检测，确保与动态物体碰撞正确
        leftRb.interpolation = RigidbodyInterpolation.Interpolate; // 插值，使移动更平滑
        
        PaddleController leftPaddleController = leftPaddle.AddComponent<PaddleController>();
        leftPaddleController.isLeftPaddle = true;
        
        // 创建右挡板（水平放置，靠近下边墙）
        GameObject rightPaddle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightPaddle.name = "RightPaddle";
        rightPaddle.transform.position = new Vector3(1.3f, 0.3f, paddleZ); // 在开口右侧
        rightPaddle.transform.localScale = new Vector3(1.5f, 1f, .3f); // 水平板子（长宽高）
        rightPaddle.transform.rotation = Quaternion.Euler(0, 0, 0); // 水平放置
        
        Renderer rightRenderer = rightPaddle.GetComponent<Renderer>();
        if (paddleMaterial != null)
        {
            rightRenderer.material = paddleMaterial;
        }
        
        // 确保碰撞器正确设置
        Collider rightCollider = rightPaddle.GetComponent<Collider>();
        if (rightCollider == null)
        {
            rightCollider = rightPaddle.AddComponent<BoxCollider>();
        }
        rightCollider.isTrigger = false; // 确保不是触发器
        
        // 添加物理材质，确保球能碰撞
        rightCollider.material = paddlePhysic;
        
        // 使用Kinematic刚体，确保碰撞检测正确
        Rigidbody rightRb = rightPaddle.AddComponent<Rigidbody>();
        rightRb.isKinematic = true; // 改为Kinematic，使用transform移动
        rightRb.useGravity = false;
        rightRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 连续动态碰撞检测，确保与动态物体碰撞正确
        rightRb.interpolation = RigidbodyInterpolation.Interpolate; // 插值，使移动更平滑
        
        PaddleController rightPaddleController = rightPaddle.AddComponent<PaddleController>();
        rightPaddleController.isLeftPaddle = false;
    }
    
    void CreateTargets()
    {
        // 创建12个较小的圆柱形目标区域用于得分
        int targetCount = 12;
        float playAreaSizeX = 5f; // 左右游戏区域大小
        float playAreaSizeZ = 4f; // 前后游戏区域大小
        
        // 定义得分块的布局位置（在游戏区域内均匀分布）
        Vector3[] targetPositions = new Vector3[]
        {
            // 第一排（靠近后墙）
            new Vector3(-3f, 0.5f, -0.5f),
            new Vector3(-1.5f, 0.5f, -0f),
            new Vector3(0f, 0.5f, -0.5f),
            new Vector3(1.5f, 0.5f, -0f),
            new Vector3(3f, 0.5f, -0.5f),
            
            // 第二排（中间）
            new Vector3(-2.5f, 0.5f, 1.5f),
            new Vector3(-1f, 0.5f, 1f),
            new Vector3(1f, 0.5f, 1f),
            new Vector3(2.5f, 0.5f, 1.5f),
            
            // 第三排（靠近前墙）
            new Vector3(-2f, 0.5f, 3f),
            new Vector3(0f, 0.5f, 2.5f),
            new Vector3(2f, 0.5f, 3f)
        };
        
        for (int i = 0; i < targetCount && i < targetPositions.Length; i++)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            target.name = $"Target_{i}";
            SetTagSafely(target, "Target");
            
            // 使用预定义的位置（Y=0.5与球的高度对齐）
            target.transform.position = targetPositions[i];
            
            // 设置为较小的圆柱形（半径0.25，高度0.4）
            // 圆柱体默认高度为2，所以scale.y=0.2时高度为0.4
            target.transform.localScale = new Vector3(0.25f, 0.2f, 0.25f);
            
            // 圆柱体默认是沿Y轴向上的，在俯视角下看起来是圆形，不需要旋转
            
            Renderer renderer = target.GetComponent<Renderer>();
            if (targetMaterial == null)
            {
                Shader shader = Shader.Find("Legacy Shaders/Diffuse");
                if (shader == null)
                {
                    shader = Shader.Find("Unlit/Color");
                }
                if (shader == null)
                {
                    shader = Shader.Find("Sprites/Default");
                }
                if (shader != null)
                {
                    targetMaterial = new Material(shader);
                    targetMaterial.color = Color.green;
                }
                else
                {
                    Debug.LogError("无法找到可用的着色器来创建目标材质！");
                }
            }
            if (targetMaterial != null)
            {
                renderer.material = targetMaterial;
            }
            
            // 添加刚体（Kinematic）
            Rigidbody rb = target.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            
            // 确保碰撞器不是触发器
            Collider collider = target.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
                
                // 添加物理材质（高弹性，让球能够反弹）
                PhysicsMaterial targetPhysic = new PhysicsMaterial("TargetMaterial");
                targetPhysic.dynamicFriction = 0.1f;
                targetPhysic.staticFriction = 0.1f;
                targetPhysic.bounciness = 0.9f; // 高弹性
                collider.material = targetPhysic;
            }
            
            // 添加粒子效果触发器
            target.AddComponent<ParticleTrigger>();

            // 选取两个奖励柱子：这里选择索引 9 和 11（第三排左、右）
            if (i == 9 || i == 11)
            {
                // 改变颜色以便区分成奖励柱子（青色）
                if (renderer != null)
                {
                    renderer.material.color = Color.cyan;
                }

                // 添加奖励柱子逻辑
                RewardPostZone reward = target.AddComponent<RewardPostZone>();
                reward.bonusKillTime = 0.7f;
            }
        }
    }
    
    void CreatePhysicsZones()
    {
        // 创建正常区域（左侧）- 使用触发器
        GameObject normalZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        normalZone.name = "NormalZone";
        normalZone.transform.position = new Vector3(-3, 0.1f, 0);
        normalZone.transform.localScale = new Vector3(4, 0.1f, 10);
        
        PhysicsZone normalZoneScript = normalZone.AddComponent<PhysicsZone>();
        normalZoneScript.friction = 0.6f;
        normalZoneScript.bounciness = 0.3f;
        normalZoneScript.zoneColor = Color.gray;
        
        // 创建冰面区域（右侧）- 低摩擦
        GameObject iceZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        iceZone.name = "IceZone";
        iceZone.transform.position = new Vector3(3, 0.1f, 0);
        iceZone.transform.localScale = new Vector3(4, 0.1f, 10);
        
        PhysicsZone iceZoneScript = iceZone.AddComponent<PhysicsZone>();
        iceZoneScript.friction = 0.05f; // 非常低的摩擦（冰面效果）
        iceZoneScript.bounciness = 0.2f;
        iceZoneScript.zoneColor = new Color(0.7f, 0.9f, 1f); // 淡蓝色
        
        // 创建重力区域（左侧小区域）
        GameObject gravityZone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        gravityZone.name = "GravityZone";
        gravityZone.transform.position = new Vector3(-3.5f, 0.5f, 1.5f); // 左侧位置
        gravityZone.transform.localScale = new Vector3(1.5f, 1f, 1.5f); // 小区域
        gravityZone.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        // 设置重力区域材质（橙色，半透明）
        Renderer gravityRenderer = gravityZone.GetComponent<Renderer>();
        Shader gravityShader = Shader.Find("Legacy Shaders/Diffuse");
        if (gravityShader == null)
        {
            gravityShader = Shader.Find("Unlit/Color");
        }
        if (gravityShader == null)
        {
            gravityShader = Shader.Find("Sprites/Default");
        }
        if (gravityShader != null)
        {
            Material gravityMaterial = new Material(gravityShader);
            gravityMaterial.color = new Color(1f, 0.5f, 0f, 0.3f); // 橙色，半透明
            gravityRenderer.material = gravityMaterial;
        }
        else
        {
            Debug.LogError("无法找到可用的着色器来创建重力区域材质！");
        }
        
        // 添加重力区域脚本
        GravityZone gravityZoneScript = gravityZone.AddComponent<GravityZone>();
        gravityZoneScript.gravityStrength = 8f; // 引力强度
        gravityZoneScript.maxInfluenceDistance = 2.5f; // 最大影响距离
        gravityZoneScript.zoneColor = new Color(1f, 0.5f, 0f, 0.3f); // 橙色，半透明
        
        // 设置为触发器
        gravityZone.GetComponent<Collider>().isTrigger = true;
        
        // 添加刚体（静态，用于触发器）
        Rigidbody gravityRb = gravityZone.AddComponent<Rigidbody>();
        gravityRb.isKinematic = true;
        gravityRb.useGravity = false;
    }

    // 敌人死亡后由 EnemyBase 调用，安排一定时间后复活
    public void ScheduleEnemyRespawn(EnemyBase deadEnemy)
    {
        if (deadEnemy == null) return;
        StartCoroutine(RespawnEnemyCoroutine(deadEnemy));
    }

    private System.Collections.IEnumerator RespawnEnemyCoroutine(EnemyBase deadEnemy)
    {
        yield return new WaitForSeconds(enemyRespawnDelay);

        // 如果游戏已经结束，就不要复活敌人
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<PinballGameManager>();
        }
        if (gameManager != null && gameManager.IsGameOver())
        {
            yield break;
        }

        // 按类型复活对应敌人（当前只有两种）
        if (deadEnemy is PatrolEnemy)
        {
            CreatePatrolEnemy();
        }
        else if (deadEnemy is ChaseEnemy)
        {
            CreateChaseEnemy();
        }
    }

    public void CreateEnemies()
    {
        // 先清理场景中已有的敌人（用于重新开始游戏刷新敌人）
        EnemyBase[] existingEnemies = FindObjectsOfType<EnemyBase>();
        foreach (var e in existingEnemies)
        {
            if (e != null)
            {
                Destroy(e.gameObject);
            }
        }

        // 左右各生成一只敌人
        CreatePatrolEnemy();
        CreateChaseEnemy();
    }

    // 创建左侧巡逻敌人
    public PatrolEnemy CreatePatrolEnemy()
    {
        GameObject patrolObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        patrolObj.name = "PatrolEnemy";
        // 稍微往下移一点（靠近挡板方向，Z 更小）
        patrolObj.transform.position = new Vector3(-3f, 0.5f, -1.5f);
        patrolObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        Renderer patrolRenderer = patrolObj.GetComponent<Renderer>();
        if (patrolRenderer != null)
        {
            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            if (shader != null)
            {
                Material m = new Material(shader);
                m.color = Color.magenta;
                patrolRenderer.material = m;
            }
        }

        // 确保有碰撞和刚体
        Collider patrolCollider = patrolObj.GetComponent<Collider>();
        if (patrolCollider == null)
        {
            patrolCollider = patrolObj.AddComponent<SphereCollider>();
        }
        patrolCollider.isTrigger = false;
        Rigidbody patrolRb = patrolObj.AddComponent<Rigidbody>();
        patrolRb.isKinematic = true;
        patrolRb.useGravity = false;

        PatrolEnemy patrolEnemy = patrolObj.AddComponent<PatrolEnemy>();
        patrolEnemy.center = new Vector3(-3f, 0.5f, -1.5f);
        patrolEnemy.radius = 1.5f;
        return patrolEnemy;
    }

    // 创建右侧追踪敌人
    public ChaseEnemy CreateChaseEnemy()
    {
        GameObject chaseObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        chaseObj.name = "ChaseEnemy";
        chaseObj.transform.position = new Vector3(3f, 0.5f, 0f);
        chaseObj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        Renderer chaseRenderer = chaseObj.GetComponent<Renderer>();
        if (chaseRenderer != null)
        {
            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            if (shader != null)
            {
                Material m = new Material(shader);
                m.color = Color.yellow;
                chaseRenderer.material = m;
            }
        }

        Collider chaseCollider = chaseObj.GetComponent<Collider>();
        if (chaseCollider == null)
        {
            chaseCollider = chaseObj.AddComponent<CapsuleCollider>();
        }
        chaseCollider.isTrigger = false;
        Rigidbody chaseRb = chaseObj.AddComponent<Rigidbody>();
        chaseRb.isKinematic = true;
        chaseRb.useGravity = false;

        ChaseEnemy chaseEnemy = chaseObj.AddComponent<ChaseEnemy>();
        // 进一步微调追踪参数（可以根据手感再改）
        chaseEnemy.detectRadius = 2.5f;
        chaseEnemy.chaseSpeed = 1.8f;
        chaseEnemy.chaseDuration = 2.0f;
        return chaseEnemy;
    }
    
    void CreateWalls()
    {
        // 创建边界墙（弹球机风格）
        float wallHeight = 2f;
        float wallThickness = 0.2f;
        float playAreaSizeX = 5f; // 左右游戏区域大小
        float playAreaSizeZ = 4f; // 前后游戏区域大小
        float openingWidth = 2.5f; // 下边墙开口宽度
        
        // 设置墙的材质（使用兼容的内置着色器）
        Shader wallShader = Shader.Find("Legacy Shaders/Diffuse");
        if (wallShader == null)
        {
            wallShader = Shader.Find("Unlit/Color");
        }
        if (wallShader == null)
        {
            wallShader = Shader.Find("Sprites/Default");
        }
        Material wallMaterial = null;
        if (wallShader != null)
        {
            wallMaterial = new Material(wallShader);
            wallMaterial.color = Color.white;
        }
        else
        {
            Debug.LogError("无法找到可用的着色器来创建墙壁材质！");
        }
        
        // 添加物理材质（高弹性，确保球能反弹）
        PhysicsMaterial wallPhysic = new PhysicsMaterial("WallMaterial");
        wallPhysic.dynamicFriction = 0.1f;
        wallPhysic.staticFriction = 0.1f;
        wallPhysic.bounciness = 0.9f; // 高弹性
        
        // 左墙
        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "LeftWall";
        leftWall.transform.position = new Vector3(-playAreaSizeX, wallHeight / 2, 0);
        leftWall.transform.localScale = new Vector3(wallThickness, wallHeight, playAreaSizeZ * 2);
        if (wallMaterial != null)
        {
            leftWall.GetComponent<Renderer>().material = wallMaterial;
        }
        leftWall.GetComponent<Collider>().material = wallPhysic;
        leftWall.AddComponent<Rigidbody>().isKinematic = true;
        
        // 右墙
        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "RightWall";
        rightWall.transform.position = new Vector3(playAreaSizeX, wallHeight / 2, 0);
        rightWall.transform.localScale = new Vector3(wallThickness, wallHeight, playAreaSizeZ * 2);
        if (wallMaterial != null)
        {
            rightWall.GetComponent<Renderer>().material = wallMaterial;
        }
        rightWall.GetComponent<Collider>().material = wallPhysic;
        rightWall.AddComponent<Rigidbody>().isKinematic = true;
        
        // 前墙（顶部）
        GameObject frontWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontWall.name = "FrontWall";
        frontWall.transform.position = new Vector3(0, wallHeight / 2, playAreaSizeZ);
        frontWall.transform.localScale = new Vector3(playAreaSizeX * 2, wallHeight, wallThickness);
        if (wallMaterial != null)
        {
            frontWall.GetComponent<Renderer>().material = wallMaterial;
        }
        frontWall.GetComponent<Collider>().material = wallPhysic;
        frontWall.AddComponent<Rigidbody>().isKinematic = true;
        
        // 后墙（下边）- 分成两段，中间留开口
        float backWallZ = -playAreaSizeZ;
        float leftWallEndX = -openingWidth / 2;
        float rightWallStartX = openingWidth / 2;
        float wallSegmentLength = (playAreaSizeX * 2 - openingWidth) / 2;
        
        // 左段后墙
        GameObject backWallLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backWallLeft.name = "BackWallLeft";
        backWallLeft.transform.position = new Vector3(-playAreaSizeX + wallSegmentLength / 2, wallHeight / 2, backWallZ);
        backWallLeft.transform.localScale = new Vector3(wallSegmentLength, wallHeight, wallThickness);
        if (wallMaterial != null)
        {
            backWallLeft.GetComponent<Renderer>().material = wallMaterial;
        }
        backWallLeft.GetComponent<Collider>().material = wallPhysic;
        backWallLeft.AddComponent<Rigidbody>().isKinematic = true;
        
        // 右段后墙
        GameObject backWallRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backWallRight.name = "BackWallRight";
        backWallRight.transform.position = new Vector3(playAreaSizeX - wallSegmentLength / 2, wallHeight / 2, backWallZ);
        backWallRight.transform.localScale = new Vector3(wallSegmentLength, wallHeight, wallThickness);
        if (wallMaterial != null)
        {
            backWallRight.GetComponent<Renderer>().material = wallMaterial;
        }
        backWallRight.GetComponent<Collider>().material = wallPhysic;
        backWallRight.AddComponent<Rigidbody>().isKinematic = true;
    }
    
    void CreateBallOutDetector()
    {
        // 创建球掉出检测区域（在开口下方）
        float playAreaSizeZ = 4f;
        float playAreaSizeX = 5f;
        
        GameObject detector = new GameObject("BallOutDetector");
        detector.transform.position = new Vector3(0, 0.5f, -playAreaSizeZ - 1f); // 在开口下方
        
        // 添加盒子碰撞器作为触发器
        BoxCollider collider = detector.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(playAreaSizeX, 2f, 2f); // 足够大的检测区域
        
        // 添加检测脚本
        detector.AddComponent<BallOutDetector>();
        
        // 可选：添加可见的视觉指示器（用于调试）
        GameObject visualIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visualIndicator.name = "BallOutVisual";
        visualIndicator.transform.SetParent(detector.transform);
        visualIndicator.transform.localPosition = Vector3.zero;
        visualIndicator.transform.localScale = Vector3.one;
        
        // 设置为半透明红色
        Renderer renderer = visualIndicator.GetComponent<Renderer>();
        Material detectorMaterial = null;
        
        // 尝试多个着色器，确保打包时可用
        Shader shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        if (shader == null)
        {
            shader = Shader.Find("Legacy Shaders/Diffuse");
        }
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }
        
        if (shader != null)
        {
            detectorMaterial = new Material(shader);
            detectorMaterial.color = new Color(1f, 0f, 0f, 0.3f); // 半透明红色
            renderer.material = detectorMaterial;
        }
        else
        {
            Debug.LogError("无法找到可用的着色器来创建检测器材质！");
        }
        
        // 移除视觉指示器的碰撞器（只保留触发器的碰撞器）
        Collider visualCollider = visualIndicator.GetComponent<Collider>();
        if (visualCollider != null)
        {
            Destroy(visualCollider);
        }
    }
    
    // 安全地设置标签（如果标签不存在，使用名称作为备用）
    void SetTagSafely(GameObject obj, string tagName)
    {
        try
        {
            // 尝试设置标签
            if (IsTagDefined(tagName))
            {
                obj.tag = tagName;
            }
            else
            {
                // 如果标签不存在，使用名称作为标识
                Debug.LogWarning($"标签 '{tagName}' 未定义，使用名称 '{obj.name}' 作为标识");
            }
        }
        catch
        {
            // 如果设置标签失败，使用名称作为标识
            Debug.LogWarning($"无法设置标签 '{tagName}'，使用名称 '{obj.name}' 作为标识");
        }
    }
    
    // 检查标签是否已定义
    bool IsTagDefined(string tagName)
    {
        try
        {
            GameObject.FindGameObjectWithTag(tagName);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

