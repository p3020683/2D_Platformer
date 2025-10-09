using UnityEngine;

public class Hazard : MonoBehaviour {
    public int m_Damage = 1;
    public int m_KnockbackPwr = 0;
    public int m_Durability = 0;
    public bool m_Instakill = false;

    GameObject m_Player;
    PlayerHealth m_PlayerHealth;
    Rigidbody2D m_PlayerRb;

    void Start() {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerHealth = m_Player.GetComponent<PlayerHealth>();
        m_PlayerRb = m_Player.GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (m_KnockbackPwr > 0) {
                m_PlayerRb.AddForce(
                    (m_Player.transform.position - transform.position).normalized * m_KnockbackPwr,
                    ForceMode2D.Impulse);
            }
            if (m_Instakill) { m_PlayerHealth.Kill(); }
            else { m_PlayerHealth.Damage(m_Damage); }
            if (m_Durability != 0 && --m_Durability <= 0) { Destroy(gameObject); }
        }
    }
}
