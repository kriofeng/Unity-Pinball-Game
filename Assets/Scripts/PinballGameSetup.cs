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
    
    void Start()
    {
        // 设置相机
        SetupCamera();
        
        // 创建UI管理器
        CreateUIManager();
        
        // 创建计分管理器
        CreateScoreManager();
        
        // 创建弹球台
        CreatePinballTable();
        
        // 创建球
        CreateBall();
        
        // 创建挡板
        CreatePaddles();
        
        // 创建目标区域
        CreateTargets();
        
        // 创建物理区域（正常区域和冰面区域）
        CreatePhysicsZones();
        
        // 创建边界墙
        CreateWalls();
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
        mainCamera.transform.position = new Vector3(0, 5, 0); // 进一步降低相机高度
        mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0); // 90度俯视
        
        // 设置相机参数（使用正交相机，俯视角更适合）
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 4f; // 视野大小（减小数值让相机更近）
        mainCamera.nearClipPlane = 0.3f;
        mainCamera.farClipPlane = 100f;
        
        // 设置背景颜色
        mainCamera.backgroundColor = new Color(0.2f, 0.2f, 0.3f);
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
            normalMaterial = new Material(shader);
            normalMaterial.color = Color.gray;
        }
        renderer.material = normalMaterial;
        
        // 添加物理材质（低摩擦，让球更容易移动）
        PhysicMaterial normalPhysic = new PhysicMaterial("NormalMaterial");
        normalPhysic.dynamicFriction = 0.2f; // 降低摩擦
        normalPhysic.staticFriction = 0.2f; // 降低摩擦
        normalPhysic.bounciness = 0.5f; // 增加弹性
        table.GetComponent<Collider>().material = normalPhysic;
    }
    
    void CreateBall()
    {
        // 创建球（放在板子上）
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball";
        SetTagSafely(ball, "Ball");
        ball.transform.position = new Vector3(-2f, 0.5f, -3f); // 放在左板子上
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
            ballMaterial = new Material(shader);
            ballMaterial.color = Color.red;
        }
        renderer.material = ballMaterial;
        
        // 添加刚体
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 0.5f; // 减小质量，让球更容易被推动
        rb.useGravity = false; // 禁用重力，球只在平面上移动
        rb.drag = 0.1f; // 降低阻力，让球更容易移动
        rb.angularDrag = 0.1f; // 降低角阻力
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 连续碰撞检测，确保与Kinematic刚体碰撞
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 冻结Y轴位置和X、Z轴旋转
        
        // 添加物理材质（高弹性）
        PhysicMaterial ballPhysic = new PhysicMaterial("BallMaterial");
        ballPhysic.dynamicFriction = 0.05f; // 降低摩擦
        ballPhysic.staticFriction = 0.05f; // 降低摩擦
        ballPhysic.bounciness = 0.9f; // 增加弹性
        ball.GetComponent<Collider>().material = ballPhysic;
        
        // 球初始静止在板子上，等待板子击打
        rb.velocity = Vector3.zero; // 初始速度为0
        rb.angularVelocity = Vector3.zero; // 初始角速度为0
        
        // 添加球控制器，保持最小速度
        ball.AddComponent<BallController>();
    }
    
    void CreatePaddles()
    {
        // 创建左挡板（水平放置）
        GameObject leftPaddle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftPaddle.name = "LeftPaddle";
        leftPaddle.transform.position = new Vector3(-2f, 0.3f, -3f); // 调整位置
        leftPaddle.transform.localScale = new Vector3(1.5f, 0.2f, 0.5f); // 水平板子（长宽高）
        leftPaddle.transform.rotation = Quaternion.Euler(0, 0, 0); // 水平放置
        
        Renderer leftRenderer = leftPaddle.GetComponent<Renderer>();
        if (paddleMaterial == null)
        {
            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            paddleMaterial = new Material(shader);
            paddleMaterial.color = Color.blue;
        }
        leftRenderer.material = paddleMaterial;
        
        // 确保碰撞器正确设置
        Collider leftCollider = leftPaddle.GetComponent<Collider>();
        if (leftCollider == null)
        {
            leftCollider = leftPaddle.AddComponent<BoxCollider>();
        }
        leftCollider.isTrigger = false; // 确保不是触发器
        
        // 添加物理材质，确保球能碰撞
        PhysicMaterial paddlePhysic = new PhysicMaterial("PaddleMaterial");
        paddlePhysic.dynamicFriction = 0.1f;
        paddlePhysic.staticFriction = 0.1f;
        paddlePhysic.bounciness = 0.8f; // 高弹性
        leftCollider.material = paddlePhysic;
        
        // 使用Kinematic刚体，确保碰撞检测正确
        Rigidbody leftRb = leftPaddle.AddComponent<Rigidbody>();
        leftRb.isKinematic = true; // 改为Kinematic，使用transform移动
        leftRb.useGravity = false;
        leftRb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 连续碰撞检测
        
        PaddleController leftPaddleController = leftPaddle.AddComponent<PaddleController>();
        leftPaddleController.isLeftPaddle = true;
        
        // 创建右挡板（水平放置）
        GameObject rightPaddle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightPaddle.name = "RightPaddle";
        rightPaddle.transform.position = new Vector3(2f, 0.3f, -3f); // 调整位置
        rightPaddle.transform.localScale = new Vector3(1.5f, 0.2f, 0.5f); // 水平板子（长宽高）
        rightPaddle.transform.rotation = Quaternion.Euler(0, 0, 0); // 水平放置
        
        Renderer rightRenderer = rightPaddle.GetComponent<Renderer>();
        rightRenderer.material = paddleMaterial;
        
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
        rightRb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 连续碰撞检测
        
        PaddleController rightPaddleController = rightPaddle.AddComponent<PaddleController>();
        rightPaddleController.isLeftPaddle = false;
    }
    
    void CreateTargets()
    {
        // 创建几个目标区域用于得分
        for (int i = 0; i < 3; i++)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = $"Target_{i}";
            SetTagSafely(target, "Target");
            
            // 基础位置
            Vector3 basePosition = new Vector3(-2 + i * 2, 0.5f, 2);
            // 添加随机位置偏差（±0.3）
            float positionOffsetX = Random.Range(-0.3f, 0.3f);
            float positionOffsetZ = Random.Range(-0.3f, 0.3f);
            target.transform.position = new Vector3(basePosition.x + positionOffsetX, basePosition.y, basePosition.z + positionOffsetZ);
            
            target.transform.localScale = new Vector3(1, 1, 0.5f);
            
            // 添加随机Y轴旋转角度（-45到45度）
            float randomRotationY = Random.Range(-45f, 45f);
            target.transform.rotation = Quaternion.Euler(0, randomRotationY, 0);
            
            Renderer renderer = target.GetComponent<Renderer>();
            if (targetMaterial == null)
            {
                Shader shader = Shader.Find("Legacy Shaders/Diffuse");
                if (shader == null)
                {
                    shader = Shader.Find("Unlit/Color");
                }
                targetMaterial = new Material(shader);
                targetMaterial.color = Color.green;
            }
            renderer.material = targetMaterial;
            
            // 添加刚体（触发器）
            Rigidbody rb = target.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            
            // 设置为触发器
            target.GetComponent<Collider>().isTrigger = false;
            
            // 添加粒子效果触发器
            target.AddComponent<ParticleTrigger>();
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
    }
    
    void CreateWalls()
    {
        // 创建边界墙（缩小范围，防止球飞出屏幕）
        float wallHeight = 2f;
        float wallThickness = 0.2f;
        float playAreaSizeX = 5f; // 左右游戏区域大小
        float playAreaSizeZ = 4f; // 前后游戏区域大小（缩小，防止超出屏幕）
        
        // 左墙
        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "LeftWall";
        leftWall.transform.position = new Vector3(-playAreaSizeX, wallHeight / 2, 0);
        leftWall.transform.localScale = new Vector3(wallThickness, wallHeight, playAreaSizeZ * 2);
        
        // 右墙
        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "RightWall";
        rightWall.transform.position = new Vector3(playAreaSizeX, wallHeight / 2, 0);
        rightWall.transform.localScale = new Vector3(wallThickness, wallHeight, playAreaSizeZ * 2);
        
        // 后墙
        GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backWall.name = "BackWall";
        backWall.transform.position = new Vector3(0, wallHeight / 2, -playAreaSizeZ);
        backWall.transform.localScale = new Vector3(playAreaSizeX * 2, wallHeight, wallThickness);
        
        // 前墙
        GameObject frontWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontWall.name = "FrontWall";
        frontWall.transform.position = new Vector3(0, wallHeight / 2, playAreaSizeZ);
        frontWall.transform.localScale = new Vector3(playAreaSizeX * 2, wallHeight, wallThickness);
        
        // 设置墙的材质（使用兼容的内置着色器）
        Shader wallShader = Shader.Find("Legacy Shaders/Diffuse");
        if (wallShader == null)
        {
            wallShader = Shader.Find("Unlit/Color");
        }
        Material wallMaterial = new Material(wallShader);
        wallMaterial.color = Color.white;
        
        leftWall.GetComponent<Renderer>().material = wallMaterial;
        rightWall.GetComponent<Renderer>().material = wallMaterial;
        backWall.GetComponent<Renderer>().material = wallMaterial;
        frontWall.GetComponent<Renderer>().material = wallMaterial;
        
        // 添加物理材质（高弹性，确保球能反弹）
        PhysicMaterial wallPhysic = new PhysicMaterial("WallMaterial");
        wallPhysic.dynamicFriction = 0.1f;
        wallPhysic.staticFriction = 0.1f;
        wallPhysic.bounciness = 0.9f; // 高弹性
        
        // 给所有墙添加物理材质
        leftWall.GetComponent<Collider>().material = wallPhysic;
        rightWall.GetComponent<Collider>().material = wallPhysic;
        backWall.GetComponent<Collider>().material = wallPhysic;
        frontWall.GetComponent<Collider>().material = wallPhysic;
        
        // 添加刚体（静态）
        leftWall.AddComponent<Rigidbody>().isKinematic = true;
        rightWall.AddComponent<Rigidbody>().isKinematic = true;
        backWall.AddComponent<Rigidbody>().isKinematic = true;
        frontWall.AddComponent<Rigidbody>().isKinematic = true;
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

