using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    public int m_MaxHp = 3;
    public Vector3 m_RespawnPoint = Vector3.zero;
    public Text m_HealthText;
    int m_Hp;
    // bool m_Alive = true;

    void Start() {
        SetHp(m_MaxHp);
    }
    void UpdateHealthUI() {
        m_HealthText.text = m_Hp + " HP";
    }
    void SetHp(int value) {
        m_Hp = value;
        UpdateHealthUI();
    }
    public void Damage(int dmg) {
        SetHp(Mathf.Clamp(m_Hp - dmg, 0, m_MaxHp));
        if (m_Hp <= 0) { Kill(); }
    }
    public void Kill() {
        if (m_Hp > 0) { SetHp(0); }
        // m_Alive = false;
        Debug.Log("Player died"); // should be actual UI
        // should have delay or await user interaction
        Respawn();
    }
    void Respawn() {
        gameObject.transform.position = m_RespawnPoint;
        SetHp(m_MaxHp);
    }
    void Reload() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}