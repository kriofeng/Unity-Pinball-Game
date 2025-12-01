using UnityEngine;

/// <summary>
/// 奖励柱子：球碰到后，会给当前回合额外增加一段击杀时间
/// </summary>
public class RewardPostZone : MonoBehaviour
{
    [Header("奖励设置")]
    public float bonusKillTime = 0.7f; // 额外增加的击杀时间（秒）

    private PinballGameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<PinballGameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsBall(collision.gameObject)) return;

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<PinballGameManager>();
        }

        if (gameManager != null)
        {
            gameManager.AddEnemyHitBonus(bonusKillTime);
        }
    }

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


