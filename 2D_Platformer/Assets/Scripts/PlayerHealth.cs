using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    public int m_MaxHp = 3;
    int m_Hp;
    bool m_Alive = true;

    void Start() {
        m_Hp = m_MaxHp;
    }
    void Damage(int dmg) {
        m_Hp = Mathf.Clamp(m_Hp - dmg, 0, m_MaxHp);
        if (m_Hp <= 0) { Kill(); }
    }
    void Kill() {
        m_Alive = false;
        Debug.Log("Player died"); // should be actual UI
        // should have delay or await user interaction
        Respawn();
    }
    void Respawn() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}