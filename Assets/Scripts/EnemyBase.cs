using UnityEngine;

/// <summary>
/// 敌人基础逻辑：
/// - 默认被球击中一次就消失（不再需要多次命中）
/// - 通过回调把加分和扣命交给 GameManager/ScoreManager 处理
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    [Header("敌人设置")]
    public int hitsToDestroy = 1; // 当前逻辑下一次命中即销毁，保留字段便于将来扩展
    public int scorePerKill = 50;

    protected int currentHits = 0;

    /// <summary>
    /// 当球在“可伤害时间窗口”内碰到敌人时调用
    /// </summary>
    public virtual void OnHitByBall(ScoreManager scoreManager)
    {
        // 现在只要在可击杀时间内被球碰到一次，就直接死亡并加分
        if (scoreManager != null)
        {
            scoreManager.AddScore(scorePerKill);
        }

        // 通知关卡重新生成敌人（延迟一段时间，且游戏未结束才会复活）
        PinballGameSetup setup = FindObjectOfType<PinballGameSetup>();
        if (setup != null)
        {
            setup.ScheduleEnemyRespawn(this);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 当球已过安全时间，再碰到敌人时调用（球被吃掉）
    /// 实际减少生命与重新生成球的逻辑在 PinballGameManager 中处理，
    /// 这里只负责通知。
    /// </summary>
    public virtual void OnEatBall(PinballGameManager gameManager, GameObject ball)
    {
        if (gameManager != null)
        {
            gameManager.OnBallEaten(ball);
        }
    }
}


