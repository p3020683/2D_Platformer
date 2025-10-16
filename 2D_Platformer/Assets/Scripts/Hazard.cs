using UnityEngine;

public class Hazard : MonoBehaviour {
    [SerializeField] int _damage = 1;
    [SerializeField] int _knockbackPwr = 0;
    [SerializeField] int _durability = 0;
    [SerializeField] bool _instakill = false;

    GameObject m_Player;
    PlayerHealth m_PlayerHealth;
    PlayerController m_PlayerController;

    void Start() {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerHealth = m_Player.GetComponent<PlayerHealth>();
        m_PlayerController = m_Player.GetComponent<PlayerController>();
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (_knockbackPwr > 0) {
                m_PlayerController.Knockback = (m_Player.transform.position - transform.position).normalized * _knockbackPwr;
            }
            if (_instakill) { m_PlayerHealth.Kill(); }
            else { m_PlayerHealth.Damage(_damage); }
            if (_durability != 0 && --_durability <= 0) { Destroy(gameObject); }
        }
    }
}
