using UnityEngine;

public class BallOutDetector : MonoBehaviour
{
    private PinballGameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<PinballGameManager>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        // 检测球是否进入掉出区域
        if (IsBall(other.gameObject))
        {
            if (gameManager != null)
            {
                gameManager.OnBallOut();
            }
            else
            {
                Debug.LogWarning("BallOutDetector: 找不到 PinballGameManager");
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

