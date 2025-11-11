using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text scoreText;
    private Text livesText;
    private Text instructionText;
    private Text gameOverText;
    private Button restartButton;
    private Canvas canvas;
    
    void Start()
    {
        CreateUI();
    }
    
    void CreateUI()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // 获取字体（使用LegacyRuntime.ttf，Unity不再支持Arial.ttf）
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // 创建分数文本
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(canvasObj.transform, false);
        scoreText = scoreObj.AddComponent<Text>();
        if (defaultFont != null)
        {
            scoreText.font = defaultFont;
        }
        scoreText.fontSize = 36;
        scoreText.color = Color.white;
        scoreText.text = "Score: 0";
        
        RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.pivot = new Vector2(0, 1);
        scoreRect.anchoredPosition = new Vector2(20, -20);
        scoreRect.sizeDelta = new Vector2(300, 50);
        
        // 创建生命数文本
        GameObject livesObj = new GameObject("LivesText");
        livesObj.transform.SetParent(canvasObj.transform, false);
        livesText = livesObj.AddComponent<Text>();
        if (defaultFont != null)
        {
            livesText.font = defaultFont;
        }
        livesText.fontSize = 36;
        livesText.color = Color.yellow;
        livesText.text = "Lives: 3";
        
        RectTransform livesRect = livesObj.GetComponent<RectTransform>();
        livesRect.anchorMin = new Vector2(0, 1);
        livesRect.anchorMax = new Vector2(0, 1);
        livesRect.pivot = new Vector2(0, 1);
        livesRect.anchoredPosition = new Vector2(20, -80);
        livesRect.sizeDelta = new Vector2(300, 50);
        
        // 创建说明文本
        GameObject instructionObj = new GameObject("InstructionText");
        instructionObj.transform.SetParent(canvasObj.transform, false);
        instructionText = instructionObj.AddComponent<Text>();
        if (defaultFont != null)
        {
            instructionText.font = defaultFont;
        }
        instructionText.fontSize = 24;
        instructionText.color = Color.white;
        instructionText.text = "Controls: A/← Left Paddle  D/→ Right Paddle";
        
        RectTransform instructionRect = instructionObj.GetComponent<RectTransform>();
        instructionRect.anchorMin = new Vector2(0, 0);
        instructionRect.anchorMax = new Vector2(0, 0);
        instructionRect.pivot = new Vector2(0, 0);
        instructionRect.anchoredPosition = new Vector2(20, 20);
        instructionRect.sizeDelta = new Vector2(500, 60);
        
        // 创建游戏结束文本（初始隐藏）
        GameObject gameOverObj = new GameObject("GameOverText");
        gameOverObj.transform.SetParent(canvasObj.transform, false);
        gameOverText = gameOverObj.AddComponent<Text>();
        if (defaultFont != null)
        {
            gameOverText.font = defaultFont;
        }
        gameOverText.fontSize = 48;
        gameOverText.color = Color.red;
        gameOverText.text = "游戏结束！";
        gameOverText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform gameOverRect = gameOverObj.GetComponent<RectTransform>();
        gameOverRect.anchorMin = new Vector2(0.5f, 0.5f);
        gameOverRect.anchorMax = new Vector2(0.5f, 0.5f);
        gameOverRect.pivot = new Vector2(0.5f, 0.5f);
        gameOverRect.anchoredPosition = new Vector2(0, 50);
        gameOverRect.sizeDelta = new Vector2(400, 100);
        gameOverObj.SetActive(false);
        
        // 创建重新开始按钮（初始隐藏）
        GameObject buttonObj = new GameObject("RestartButton");
        buttonObj.transform.SetParent(canvasObj.transform, false);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        
        restartButton = buttonObj.AddComponent<Button>();
        
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        Text buttonText = buttonTextObj.AddComponent<Text>();
        if (defaultFont != null)
        {
            buttonText.font = defaultFont;
        }
        buttonText.text = "重新开始 (R)";
        buttonText.fontSize = 32;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0, -50);
        buttonRect.sizeDelta = new Vector2(300, 60);
        buttonObj.SetActive(false);
        
        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {lives}";
        }
    }
    
    public void ShowGameOver(int finalScore)
    {
        if (gameOverText != null)
        {
            gameOverText.text = $"游戏结束！\n最终得分: {finalScore}";
            gameOverText.gameObject.SetActive(true);
        }
        
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            restartButton.onClick.AddListener(() => {
                PinballGameManager gameManager = FindObjectOfType<PinballGameManager>();
                if (gameManager != null)
                {
                    gameManager.RestartGame();
                }
            });
        }
    }
    
    public void HideGameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.RemoveAllListeners();
        }
    }
}

