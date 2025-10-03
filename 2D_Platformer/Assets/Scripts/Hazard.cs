using UnityEngine;

public class Hazard : MonoBehaviour {
    public int m_Damage = 1;
    public bool m_KnockBack = true;
    public bool m_InstaKill = false;
    public bool m_OneShot = false;

    GameObject m_Player;
    PlayerHealth m_PlayerHealth;
    Rigidbody2D m_PlayerRb;

    void Start() {
        GameObject m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerHealth = m_Player.GetComponent<PlayerHealth>();
        if (m_KnockBack) { m_PlayerRb = m_Player.GetComponent<Rigidbody2D>(); }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (m_KnockBack) {
                m_PlayerRb.AddForce(
                    (m_Player.transform.position - transform.position).normalized * 10f,
                    ForceMode2D.Impulse);
            }
            if (m_InstaKill) { m_PlayerHealth.Kill(); }
            else { m_PlayerHealth.Damage(m_Damage); }
            if (m_OneShot) { Destroy(gameObject); }
        }
    }
}
