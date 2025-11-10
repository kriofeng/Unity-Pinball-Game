using UnityEngine;

public class PhysicsZone : MonoBehaviour
{
    [Header("物理设置")]
    public float friction = 0.6f;
    public float bounciness = 0.3f;
    public Color zoneColor = Color.gray;
    
    private PhysicMaterial zoneMaterial;
    
    void Start()
    {
        // 创建物理材质
        zoneMaterial = new PhysicMaterial("ZoneMaterial");
        zoneMaterial.dynamicFriction = friction;
        zoneMaterial.staticFriction = friction;
        zoneMaterial.bounciness = bounciness;
        
        // 设置为触发器
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        // 设置区域颜色（可选，用于可视化）
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            Material mat = new Material(shader);
            mat.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.3f);
            renderer.material = mat;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (IsBall(other.gameObject))
        {
            // 当球进入区域时，改变球的物理材质
            Collider ballCollider = other.GetComponent<Collider>();
            if (ballCollider != null)
            {
                ballCollider.material = zoneMaterial;
            }
            
            // 如果是冰面区域（低摩擦），启用拖尾
            BallController ballController = other.GetComponent<BallController>();
            if (ballController != null && friction < 0.1f) // 冰面区域摩擦很低
            {
                ballController.SetOnIce(true);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (IsBall(other.gameObject))
        {
            // 当球离开区域时，如果是冰面区域，禁用拖尾
            BallController ballController = other.GetComponent<BallController>();
            if (ballController != null && friction < 0.1f) // 冰面区域摩擦很低
            {
                ballController.SetOnIce(false);
            }
        }
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

