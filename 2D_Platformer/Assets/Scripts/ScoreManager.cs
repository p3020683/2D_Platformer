using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    [SerializeField] Text _scoreText;
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
