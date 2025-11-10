using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int currentScore = 0;
    private UIManager uiManager;
    
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
        }
        Debug.Log($"得分: {currentScore}");
    }
    
    public int GetScore()
    {
        return currentScore;
    }
}




