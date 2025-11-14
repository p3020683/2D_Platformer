using TMPro;
using UnityEngine;

public class Answer : MonoBehaviour {
    [SerializeField] float _number = 0f;
    [SerializeField] TMP_Text _text;
    [SerializeField] EquationManager _equationManager;

    void Start() {
        UpdateNumber();
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            _equationManager.OnAnswer(_number);
        }
    }
    void UpdateNumber() {
        _text.text = _number.ToString();
    }
    public void SetNumber(float number) {
        _number = number;
        UpdateNumber();
    }
}
