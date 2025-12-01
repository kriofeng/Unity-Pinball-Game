using UnityEngine;

public class PinballGameManager : MonoBehaviour
{
    [Header("游戏设置")]
    public int lives = 3; // 生命数
    public float ballRespawnDelay = 1f; // 球重生延迟
    
    private ScoreManager scoreManager;
    private UIManager uiManager;
    private PinballGameSetup gameSetup;
    private GameObject currentBall;
    private bool isGameOver = false;
    private int currentLives;

    [Header("球与敌人交互")]
    public float enemyHitGraceTime = 1.5f; // 基础击杀时间窗口（秒）

    private float ballLaunchTime = -999f;
    // 由奖励柱子等提供的额外击杀时间（单独的加成窗口，即使基础时间已结束也能重新获得击杀时间）
    private float enemyHitExtraEndTime = -999f;
    
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        uiManager = FindObjectOfType<UIManager>();
        gameSetup = FindObjectOfType<PinballGameSetup>();
        currentLives = lives;
        
        if (uiManager != null)
        {
            uiManager.UpdateLives(currentLives);
        }
    }

    /// <summary>
    /// 当球被挡板“重新打出”时调用，用于记录安全时间窗口开始
    /// </summary>
    public void OnBallLaunched()
    {
        ballLaunchTime = Time.time;
        // 每次重新打出球时，清空之前累积的奖励时间窗口
        enemyHitExtraEndTime = -999f;
    }
    
    public void OnBallOut()
    {
        if (isGameOver) return;
        
        // 减少生命数
        currentLives--;
        
        if (uiManager != null)
        {
            uiManager.UpdateLives(currentLives);
        }
        
        // 销毁当前球
        if (currentBall != null)
        {
            Destroy(currentBall);
            currentBall = null;
        }
        
        // 检查游戏是否结束
        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            // 延迟后重新生成球
            Invoke("RespawnBall", ballRespawnDelay);
        }
    }

    /// <summary>
    /// 敌人吃掉球时调用
    /// </summary>
    public void OnBallEaten(GameObject ball)
    {
        if (isGameOver) return;

        // 销毁这颗球
        if (ball != null)
        {
            Destroy(ball);
        }

        OnBallOut();
    }
    
    void RespawnBall()
    {
        if (isGameOver) return;
        
        // 如果gameSetup为null，尝试重新查找
        if (gameSetup == null)
        {
            gameSetup = FindObjectOfType<PinballGameSetup>();
        }
        
        if (gameSetup != null)
        {
            currentBall = gameSetup.CreateNewBall();
            if (currentBall != null)
            {
                SetCurrentBall(currentBall);
            }
        }
        else
        {
            Debug.LogError("PinballGameManager: 无法找到 PinballGameSetup！");
        }
    }
    
    void GameOver()
    {
        isGameOver = true;
        
        if (uiManager != null)
        {
            uiManager.ShowGameOver(scoreManager != null ? scoreManager.GetScore() : 0);
        }
        
        Debug.Log("游戏结束！最终得分: " + (scoreManager != null ? scoreManager.GetScore() : 0));
    }
    
    public void RestartGame()
    {
        isGameOver = false;
        currentLives = lives;
        
        // 重置分数
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }

        // 重新生成敌人
        if (gameSetup == null)
        {
            gameSetup = FindObjectOfType<PinballGameSetup>();
        }
        if (gameSetup != null)
        {
            gameSetup.CreateEnemies();
        }
        
        // 销毁当前球
        if (currentBall != null)
        {
            Destroy(currentBall);
            currentBall = null;
        }
        
        // 更新UI
        if (uiManager != null)
        {
            uiManager.HideGameOver();
            uiManager.UpdateLives(currentLives);
        }
        
        // 重新生成球
        RespawnBall();
    }
    
    void Update()
    {
        // 按R键重新开始游戏
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isGameOver)
            {
                RestartGame();
            }
        }
    }
    
    public void SetCurrentBall(GameObject ball)
    {
        currentBall = ball;
    }

    /// <summary>
    /// 是否处于可以伤害敌人的安全时间窗口内
    /// </summary>
    public bool IsInEnemyHitGraceTime()
    {
        float elapsed = Time.time - ballLaunchTime;
        bool inBaseWindow = elapsed <= enemyHitGraceTime;
        bool inBonusWindow = Time.time <= enemyHitExtraEndTime;
        return inBaseWindow || inBonusWindow;
    }

    /// <summary>
    /// 增加额外击杀时间（例如奖励柱子提供的加成）
    /// </summary>
    public void AddEnemyHitBonus(float bonusSeconds)
    {
        if (bonusSeconds <= 0f) return;
        // 从当前时间或现有奖励结束时间开始延长奖励窗口
        float start = Mathf.Max(Time.time, enemyHitExtraEndTime);
        enemyHitExtraEndTime = start + bonusSeconds;
    }

    /// <summary>
    /// 提供给其他系统判断当前游戏是否已经结束
    /// </summary>
    public bool IsGameOver()
    {
        return isGameOver;
    }
}

