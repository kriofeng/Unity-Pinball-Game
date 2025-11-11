using UnityEngine;

public class GravityZone : MonoBehaviour
{
    [Header("重力设置")]
    public float gravityStrength = 5f; // 引力强度
    public float maxInfluenceDistance = 3f; // 最大影响距离
    public Color zoneColor = new Color(1f, 0.5f, 0f, 0.3f); // 橙色，半透明
    
    private Vector3 centerPosition; // 区域中心位置
    private float zoneRadius; // 区域半径
    
    void Start()
    {
        // 获取区域中心位置（transform.position）
        centerPosition = transform.position;
        
        // 计算区域半径（基于Collider的bounds）
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Bounds bounds = col.bounds;
            // 使用XZ平面的最大尺寸作为半径
            zoneRadius = Mathf.Max(bounds.size.x, bounds.size.z) * 0.5f;
        }
        else
        {
            // 如果没有Collider，使用默认值
            zoneRadius = 1f;
        }
        
        // 设置为触发器
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        // 设置区域颜色（用于可视化）
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            Material mat = new Material(shader);
            mat.color = zoneColor;
            renderer.material = mat;
        }
    }
    
    // 获取区域中心位置（供BallController调用）
    public Vector3 GetCenterPosition()
    {
        return centerPosition;
    }
    
    // 获取区域半径（供BallController调用）
    public float GetRadius()
    {
        return zoneRadius;
    }
    
    // 获取最大影响距离
    public float GetMaxInfluenceDistance()
    {
        return maxInfluenceDistance;
    }
    
    // 获取引力强度
    public float GetGravityStrength()
    {
        return gravityStrength;
    }
    
    // 检查点是否在影响范围内
    public bool IsInInfluenceRange(Vector3 position)
    {
        // 在XZ平面上计算距离
        Vector3 flatPosition = new Vector3(position.x, 0, position.z);
        Vector3 flatCenter = new Vector3(centerPosition.x, 0, centerPosition.z);
        float distance = Vector3.Distance(flatPosition, flatCenter);
        return distance <= maxInfluenceDistance;
    }
    
    // 计算引力方向（从位置指向中心）
    public Vector3 GetGravityDirection(Vector3 position)
    {
        // 在XZ平面上计算方向
        Vector3 flatPosition = new Vector3(position.x, 0, position.z);
        Vector3 flatCenter = new Vector3(centerPosition.x, 0, centerPosition.z);
        Vector3 direction = (flatCenter - flatPosition).normalized;
        return direction;
    }
    
    // 根据距离计算引力强度（距离越近，引力越大）
    public float GetGravityForceAtDistance(Vector3 position)
    {
        // 在XZ平面上计算距离
        Vector3 flatPosition = new Vector3(position.x, 0, position.z);
        Vector3 flatCenter = new Vector3(centerPosition.x, 0, centerPosition.z);
        float distance = Vector3.Distance(flatPosition, flatCenter);
        
        if (distance > maxInfluenceDistance)
        {
            return 0f; // 超出影响范围，无引力
        }
        
        if (distance < 0.1f)
        {
            return 0f; // 太近，避免无限大
        }
        
        // 使用反平方定律：引力与距离的平方成反比
        // 距离越近，引力越大
        float normalizedDistance = distance / maxInfluenceDistance; // 归一化距离（0-1）
        float forceMultiplier = 1f - normalizedDistance; // 距离越近，乘数越大（1到0）
        forceMultiplier = Mathf.Pow(forceMultiplier, 2f); // 平方，使引力变化更明显
        
        return gravityStrength * forceMultiplier;
    }
}

