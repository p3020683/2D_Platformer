using UnityEngine;

public class Hazard : MonoBehaviour {
    public int m_Damage = 1;
    public bool m_OneShot;
    PlayerHealth m_PlayerHealth;

    void Start() {
        m_PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            m_PlayerHealth.Damage(m_Damage);
            if (m_OneShot) { Destroy(gameObject); }
        }
    }
}
