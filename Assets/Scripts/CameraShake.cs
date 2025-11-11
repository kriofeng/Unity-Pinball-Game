using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("震动设置")]
    public float shakeDuration = 0.15f; // 震动持续时间（减小）
    public float shakeIntensity = 0.03f; // 震动强度（减小）
    public float shakeDecreaseFactor = 1.5f; // 震动衰减速度
    
    private Vector3 originalPosition; // 原始位置
    private float currentShakeDuration = 0f; // 当前震动剩余时间
    private float currentShakeIntensity = 0f; // 当前震动强度
    private Camera cam; // 相机组件
    private bool isInitialized = false; // 是否已初始化
    
    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        if (isInitialized) return;
        
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        if (cam != null)
        {
            // 使用position而不是localPosition，因为相机可能没有父对象
            originalPosition = cam.transform.position;
            isInitialized = true;
        }
    }
    
    void Update()
    {
        if (!isInitialized)
        {
            Initialize();
        }
        
        if (cam == null) return;
        
        if (currentShakeDuration > 0)
        {
            // 随机偏移相机位置（只在XZ平面上震动，因为这是俯视角）
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * currentShakeIntensity,
                0,
                Random.Range(-1f, 1f) * currentShakeIntensity
            );
            cam.transform.position = originalPosition + shakeOffset;
            
            // 减少震动时间和强度
            currentShakeDuration -= Time.deltaTime * shakeDecreaseFactor;
            currentShakeIntensity = Mathf.Lerp(0f, shakeIntensity, currentShakeDuration / shakeDuration);
        }
        else
        {
            // 震动结束，恢复原始位置
            currentShakeDuration = 0f;
            if (cam != null)
            {
                cam.transform.position = originalPosition;
            }
        }
    }
    
    // 触发震动
    public void Shake(float intensity = -1f, float duration = -1f)
    {
        if (!isInitialized)
        {
            Initialize();
        }
        
        if (cam == null) return;
        
        // 更新原始位置（防止相机位置改变后震动位置不对）
        originalPosition = cam.transform.position;
        
        // 使用参数值，如果没有提供则使用默认值
        currentShakeIntensity = intensity > 0 ? intensity : shakeIntensity;
        currentShakeDuration = duration > 0 ? duration : shakeDuration;
    }
    
    // 根据碰撞强度触发震动（速度越大，震动越强）
    public void ShakeByImpact(float impactSpeed)
    {
        // 根据碰撞速度计算震动强度（速度越大，震动越强）
        // 减小震动强度，使用更小的系数
        float speedFactor = Mathf.Clamp01(impactSpeed / 15f); // 15是最大速度
        float intensity = shakeIntensity * (0.3f + speedFactor * 0.4f); // 最小30%，最大70%（减小范围）
        Shake(intensity, shakeDuration);
    }
}

