using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text scoreText;
    private Text instructionText;
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
        
        // 创建分数文本
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(canvasObj.transform, false);
        scoreText = scoreObj.AddComponent<Text>();
        
        // 获取字体（使用LegacyRuntime.ttf，Unity不再支持Arial.ttf）
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        
        // 创建说明文本
        GameObject instructionObj = new GameObject("InstructionText");
        instructionObj.transform.SetParent(canvasObj.transform, false);
        instructionText = instructionObj.AddComponent<Text>();
        instructionText.font = defaultFont;
        instructionText.fontSize = 24;
        instructionText.color = Color.white;
        instructionText.text = "Controls: A/← Left Paddle  D/→ Right Paddle";
        
        RectTransform instructionRect = instructionObj.GetComponent<RectTransform>();
        instructionRect.anchorMin = new Vector2(0, 0);
        instructionRect.anchorMax = new Vector2(0, 0);
        instructionRect.pivot = new Vector2(0, 0);
        instructionRect.anchoredPosition = new Vector2(20, 20);
        instructionRect.sizeDelta = new Vector2(500, 40);
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}

