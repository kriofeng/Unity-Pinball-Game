using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    private ParticleSystem particleEffect;
    private ScoreManager scoreManager;
    private GameObject particleSystemObject; // 粒子系统对象（可能是子对象或当前对象）
    private bool isInitialized = false; // 标记是否已经初始化完成
    
    void Start()
    {
        // 检查是否已有粒子系统子对象
        Transform particleSystemTransform = transform.Find("ParticleSystem");
        if (particleSystemTransform != null)
        {
            particleSystemObject = particleSystemTransform.gameObject;
            particleEffect = particleSystemObject.GetComponent<ParticleSystem>();
        }
        
        // 如果当前对象上有粒子系统组件，需要移除（无论是否已有子对象）
        ParticleSystem existingParticleSystem = GetComponent<ParticleSystem>();
        if (existingParticleSystem != null)
        {
            // 移除当前对象上的粒子系统组件
            Destroy(existingParticleSystem);
            ParticleSystemRenderer existingRenderer = GetComponent<ParticleSystemRenderer>();
            if (existingRenderer != null)
            {
                Destroy(existingRenderer);
            }
        }
        
        // 如果没有粒子系统子对象，创建一个
        if (particleSystemObject == null)
        {
            particleSystemObject = new GameObject("ParticleSystem");
            particleSystemObject.transform.SetParent(transform);
            particleSystemObject.transform.localPosition = Vector3.zero;
            particleSystemObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // 绕x轴旋转90°
            particleSystemObject.transform.localScale = Vector3.one;
            
            particleEffect = particleSystemObject.AddComponent<ParticleSystem>();
        }
        else if (particleEffect == null)
        {
            // 如果子对象存在但没有粒子系统组件，添加一个
            particleEffect = particleSystemObject.AddComponent<ParticleSystem>();
        }
        
        // 确保粒子系统对象绕x轴旋转90°
        particleSystemObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        
        // 无论粒子系统是新创建还是已存在，都设置一次
        SetupParticleSystem();
        
        // 再次确保粒子系统停止（防止在初始化时自动触发）
        if (particleEffect != null)
        {
            particleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleEffect.Clear();
        }
        
        scoreManager = FindObjectOfType<ScoreManager>();
        
        // 标记初始化完成
        isInitialized = true;
    }
    
    void SetupParticleSystem()
    {
        var main = particleEffect.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 2f;
        main.startSize = 0.1f;
        main.startColor = Color.yellow;
        main.maxParticles = 20;
        main.playOnAwake = false; // 禁止自动播放，只在碰撞时手动触发
        main.loop = false; // 不循环播放
        
        // 确保粒子使用正确的渲染模式和材质
        var renderer = particleEffect.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            // 使用Billboard渲染模式，让粒子始终对着相机
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.alignment = ParticleSystemRenderSpace.View; // 确保粒子对着相机视图
            
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
                // 如果是粒子着色器，设置TintColor
                if (particleShader.name.Contains("Particles"))
                {
                    particleMaterial.SetColor("_TintColor", Color.yellow);
                }
                renderer.material = particleMaterial;
            }
            else
            {
                // 如果找不到着色器，使用默认材质
                renderer.material = null;
            }
        }
        
        var emission = particleEffect.emission;
        emission.rateOverTime = 0; // 不持续发射
        emission.enabled = true;
        // 设置burst，但不在时间0触发，而是通过Play()方法触发
        // 当调用Play()时，burst会在时间0触发，这正是我们想要的
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, 10) // 在Play()调用时立即触发10个粒子
        });
        
        var shape = particleEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;
        
        // 确保粒子系统停止并清除所有粒子
        // 使用StopEmittingAndClear确保清除所有已存在的粒子
        particleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particleEffect.Clear(); // 额外清除一次，确保没有残留粒子
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // 只有在初始化完成后才处理碰撞
        if (!isInitialized)
        {
            return;
        }
        
        if (IsBall(collision.gameObject))
        {
            // 触发粒子效果（每次碰撞都触发）
            if (particleEffect != null)
            {
                // 先停止并清除，然后重新播放，确保每次碰撞都能触发效果
                particleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleEffect.Clear();
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

