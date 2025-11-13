using TMPro;
using UnityEngine;

public class Answer : MonoBehaviour {
    [SerializeField] int _number = 0;
    [SerializeField] TMP_Text _text;
    [SerializeField] QuestionManager _questionManager;

    void Start() {
        UpdateNumber();
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            _questionManager.OnAnswer(_number);
        }
    }
    void UpdateNumber() {
        _text.text = _number.ToString();
    }
    public void SetNumber(int number) {
        _number = number;
        UpdateNumber();
    }
}
