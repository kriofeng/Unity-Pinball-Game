using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [Header("控制设置")]
    public bool isLeftPaddle = true;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    
    [Header("挡板设置")]
    public float paddleForce = 15f;
    public float rotationSpeed = 400f; // 旋转速度（增加速度）
    public float maxRotationAngle = 45f; // 最大旋转角度
    
    private Rigidbody rb;
    private float initialRotationY; // 初始Y轴旋转
    private float currentRotationY = 0f; // 当前旋转角度
    private float previousRotationY = 0f; // 上一帧的旋转角度
    private float rotationVelocity = 0f; // 当前旋转速度（度/秒）
    private GameObject ballOnPaddle; // 在板子上的球
    private Vector3 pivotPoint; // 旋转轴点（左板子在左边，右板子在右边）
    private Vector3 initialPosition; // 初始位置
    private float paddleHalfLength; // 板子长度的一半
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialRotationY = transform.rotation.eulerAngles.y;
        initialPosition = transform.position;
        
        // 获取板子长度的一半（scale.x是长度）
        paddleHalfLength = transform.localScale.x * 0.5f;
        
        // 计算旋转轴点
        if (isLeftPaddle)
        {
            // 左板子：旋转轴在左边
            pivotPoint = new Vector3(initialPosition.x - paddleHalfLength, initialPosition.y, initialPosition.z);
        }
        else
        {
            // 右板子：旋转轴在右边
            pivotPoint = new Vector3(initialPosition.x + paddleHalfLength, initialPosition.y, initialPosition.z);
        }
        
        // 如果未设置，根据X坐标自动判断
        if (transform.position.x < 0)
        {
            isLeftPaddle = true;
            pivotPoint = new Vector3(initialPosition.x - paddleHalfLength, initialPosition.y, initialPosition.z);
        }
        else
        {
            isLeftPaddle = false;
            pivotPoint = new Vector3(initialPosition.x + paddleHalfLength, initialPosition.y, initialPosition.z);
        }
    }
    
    void Update()
    {
        // 检测输入
        bool shouldActivate = false;
        
        if (isLeftPaddle)
        {
            shouldActivate = Input.GetKey(leftKey) || Input.GetKey(KeyCode.LeftArrow);
        }
        else
        {
            shouldActivate = Input.GetKey(rightKey) || Input.GetKey(KeyCode.RightArrow);
        }
        
        // 保存上一帧的旋转角度
        previousRotationY = currentRotationY;
        
        // 控制挡板旋转（往上旋转）
        if (shouldActivate)
        {
            // 往上旋转（绕Y轴旋转，俯视角下上下旋转）
            // 左板子需要反向旋转
            float rotationDirection = isLeftPaddle ? -1f : 1f;
            float newRotation = currentRotationY + rotationSpeed * Time.deltaTime * rotationDirection;
            
            // 限制角度范围
            if (isLeftPaddle)
            {
                currentRotationY = Mathf.Max(newRotation, -maxRotationAngle); // 左板子负角度
            }
            else
            {
                currentRotationY = Mathf.Min(newRotation, maxRotationAngle); // 右板子正角度
            }
            
            // 计算旋转速度（度/秒）
            rotationVelocity = (currentRotationY - previousRotationY) / Time.deltaTime;
            
            // 检查球是否在板子上，如果是，给球一个力（初始发射）
            if (ballOnPaddle != null)
            {
                Rigidbody ballRb = ballOnPaddle.GetComponent<Rigidbody>();
                if (ballRb != null)
                {
                    // 根据是左挡板还是右挡板，给球一个方向（只在XZ平面）
                    Vector3 direction;
                    if (isLeftPaddle)
                    {
                        direction = new Vector3(0.3f, 0, 1f).normalized; // 向右前方（只在XZ平面）
                    }
                    else
                    {
                        direction = new Vector3(-0.3f, 0, 1f).normalized; // 向左前方（只在XZ平面）
                    }
                    
                    // 直接设置速度（只在XZ平面），而不是使用AddForce，避免产生Y轴速度
                    Vector3 newVelocity = new Vector3(direction.x * paddleForce, 0, direction.z * paddleForce);
                    ballRb.velocity = newVelocity;
                    ballOnPaddle = null; // 清除引用
                }
            }
        }
        else
        {
            // 板子回到初始位置
            currentRotationY = Mathf.Lerp(currentRotationY, 0f, Time.deltaTime * 5f);
            // 计算旋转速度（度/秒）
            rotationVelocity = (currentRotationY - previousRotationY) / Time.deltaTime;
        }
        
        // 应用旋转（绕Y轴旋转，绕边缘点旋转）
        float rotationAngle = initialRotationY + currentRotationY;
        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationAngle, transform.rotation.eulerAngles.z);
        
        // 计算旋转后的位置（绕pivotPoint旋转）
        Vector3 offsetFromPivot = initialPosition - pivotPoint;
        Vector3 rotatedOffset = rotation * offsetFromPivot;
        transform.position = pivotPoint + rotatedOffset;
        transform.rotation = rotation;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // 当球碰撞挡板时，使用物理反射
        if (IsBall(collision.gameObject))
        {
            HandleBallCollision(collision);
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        // 持续检测球是否在板子上（用于初始发射）
        if (IsBall(collision.gameObject))
        {
            ballOnPaddle = collision.gameObject;
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        // 球离开板子时清除引用
        if (collision.gameObject == ballOnPaddle)
        {
            ballOnPaddle = null;
        }
    }
    
    void HandleBallCollision(Collision collision)
    {
        Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
        if (ballRb == null) return;
        
        // 获取碰撞点信息
        if (collision.contacts.Length == 0) return;
        ContactPoint contact = collision.contacts[0];
        Vector3 normal = contact.normal;
        
        // 确保法线在XZ平面上（俯视角）
        normal = new Vector3(normal.x, 0, normal.z).normalized;
        if (normal.magnitude < 0.1f) return;
        
        // 获取球的入射速度（在XZ平面上）
        Vector3 incomingVelocity = ballRb.velocity;
        Vector3 flatIncoming = new Vector3(incomingVelocity.x, 0, incomingVelocity.z);
        if (flatIncoming.magnitude < 0.1f) return;
        
        // 计算物理反射方向（在XZ平面上）
        Vector3 reflectDirection = Vector3.Reflect(-flatIncoming.normalized, normal);
        reflectDirection = new Vector3(reflectDirection.x, 0, reflectDirection.z).normalized;
        
        // 获取板子的旋转速度（角速度，度/秒）
        float paddleAngularVelocity = rotationVelocity;
        
        // 计算出射速度（基础速度 = 入射速度）
        float outgoingSpeed = flatIncoming.magnitude;
        
        // 如果板子有旋转速度（向上旋转），给球额外的速度
        if (Mathf.Abs(paddleAngularVelocity) > 0.1f)
        {
            // 将角速度转换为线速度（在板子边缘）
            // 板子边缘的线速度 = 角速度（度/秒转弧度/秒） * 半径（板子长度的一半）
            float paddleEdgeSpeed = (Mathf.Abs(paddleAngularVelocity) * Mathf.Deg2Rad) * paddleHalfLength;
            
            // 计算板子边缘的速度方向（板子向上旋转时，边缘向前移动）
            // 板子表面法线是normal，板子边缘速度方向是垂直于法线，沿着板子表面向前
            Vector3 paddleForward = new Vector3(-normal.z, 0, normal.x).normalized;
            if (isLeftPaddle)
            {
                // 左板子向上旋转时，边缘向右前方移动
                paddleForward = new Vector3(normal.z, 0, -normal.x).normalized;
            }
            
            // 如果板子向上旋转（paddleAngularVelocity > 0 对于右板子，< 0 对于左板子），给球加速
            bool isRotatingUp = (isLeftPaddle && paddleAngularVelocity < 0) || (!isLeftPaddle && paddleAngularVelocity > 0);
            
            if (isRotatingUp)
            {
                // 将板子的速度添加到反射方向
                Vector3 paddleVelocity = paddleForward * paddleEdgeSpeed;
                reflectDirection = (reflectDirection * flatIncoming.magnitude + paddleVelocity).normalized;
                
                // 增加出射速度（板子速度的一部分传递给球）
                outgoingSpeed += paddleEdgeSpeed * 0.5f;
            }
        }
        
        // 应用反射速度（只在XZ平面）
        ballRb.velocity = new Vector3(reflectDirection.x * outgoingSpeed, 0, reflectDirection.z * outgoingSpeed);
    }
    
    // 检查是否是球（使用标签或名称）
    bool IsBall(GameObject obj)
    {
        try
        {
            return obj.CompareTag("Ball");
        }
        catch
        {
            return obj.name == "Ball";
        }
    }
}

