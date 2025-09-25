using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public TextMeshProUGUI m_ScoreText;
    int m_Score;

    void Start() {
        UpdateScoreUI();
    }
    public void AddScore(int amount) {
        m_Score += amount;
        UpdateScoreUI();
    }
    void UpdateScoreUI() {
        m_ScoreText.text = "Score: " + m_Score;
    }
}
