using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    private ParticleSystem particleEffect;
    private ScoreManager scoreManager;
    
    void Start()
    {
        // 获取或创建粒子系统
        particleEffect = GetComponent<ParticleSystem>();
        if (particleEffect == null)
        {
            particleEffect = gameObject.AddComponent<ParticleSystem>();
        }
        
        // 无论粒子系统是新创建还是已存在，都设置一次
        SetupParticleSystem();
        
        scoreManager = FindObjectOfType<ScoreManager>();
    }
    
    void SetupParticleSystem()
    {
        var main = particleEffect.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 2f;
        main.startSize = 0.1f;
        main.startColor = Color.yellow;
        main.maxParticles = 20;
        
        // 确保粒子使用正确的渲染模式和材质
        var renderer = particleEffect.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            // 使用Billboard渲染模式
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            
            // 创建简单的粒子材质，确保打包时可用
            Shader particleShader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
            if (particleShader == null)
            {
                particleShader = Shader.Find("Sprites/Default");
            }
            if (particleShader == null)
            {
                particleShader = Shader.Find("Unlit/Color");
            }
            
            if (particleShader != null)
            {
                Material particleMaterial = new Material(particleShader);
                particleMaterial.color = Color.yellow;
                renderer.material = particleMaterial;
            }
            else
            {
                // 如果找不到着色器，使用默认材质
                renderer.material = null;
            }
        }
        
        var emission = particleEffect.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, 10)
        });
        
        var shape = particleEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (IsBall(collision.gameObject))
        {
            // 触发粒子效果
            if (particleEffect != null)
            {
                particleEffect.Play();
            }
            
            // 如果是目标区域，增加分数
            if (IsTarget(gameObject))
            {
                if (scoreManager != null)
                {
                    scoreManager.AddScore(10);
                }
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
    
    // 检查是否是目标（使用标签或名称）
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
}

