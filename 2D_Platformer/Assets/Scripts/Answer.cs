using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour {
    public int m_Number = 0;
    public Text m_WorldText;
    public QuestionManager m_QuestionManager;

    void Start() {
        UpdateNumber();
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            m_QuestionManager.OnAnswer(m_Number);
        }
    }
    void UpdateNumber() {
        m_WorldText.text = m_Number.ToString();
    }
    public void SetNumber(int number) {
        m_Number = number;
        UpdateNumber();
    }
}
