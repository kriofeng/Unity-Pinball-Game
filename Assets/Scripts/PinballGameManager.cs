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
}

