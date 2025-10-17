using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {
    [SerializeField] TMP_Text _scoreText;
    int _score;

    void Start() {
        UpdateScoreUI();
    }
    public void AddScore(int amount) {
        _score += amount;
        UpdateScoreUI();
    }
    void UpdateScoreUI() {
        _scoreText.text = "Score: " + _score;
    }
}
