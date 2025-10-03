using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    public int m_MaxHp = 3;
    public Text m_HealthText;
    int m_Hp;
    // bool m_Alive = true;

    void Start() {
        m_Hp = m_MaxHp;
        UpdateHealthUI();
    }
    void UpdateHealthUI() {
        m_HealthText.text = m_Hp + " HP";
    }
    public void Damage(int dmg) {
        m_Hp = Mathf.Clamp(m_Hp - dmg, 0, m_MaxHp);
        UpdateHealthUI();
        if (m_Hp <= 0) { Kill(); }
    }
    public void Kill() {
        if (m_Hp > 0) { m_Hp = 0; UpdateHealthUI(); }
        // m_Alive = false;
        Debug.Log("Player died"); // should be actual UI
        // should have delay or await user interaction
        Respawn();
    }
    void Respawn() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}