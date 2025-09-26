using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float m_Speed = 10f;
    public float m_JumpPwr = 5f;
    public float m_DashPwr = 10f;
    Rigidbody2D m_Rb;
    int m_Jumps, m_MaxJumps = 2;
    bool m_DashCooldown = false;

    void Start() {
        m_DashPwr *= 10;
        m_Rb = GetComponent<Rigidbody2D>();
        ReplenishJumps();
    }
    void Update() {
        float xInput = Input.GetAxisRaw("Horizontal");
        m_Rb.AddForce(new(xInput * m_Speed - m_Rb.velocity.x, 0f));

        if (Input.GetButtonDown("Jump") && m_Jumps-- > 0) {
            m_Rb.velocity = new(m_Rb.velocity.x, Mathf.Max(m_JumpPwr, m_JumpPwr + m_Rb.velocity.y));
        }

        if (xInput != 0 && Input.GetButtonDown("Sprint")) {
            m_Rb.AddForce(new(m_DashPwr * Mathf.Sign(xInput) + m_Rb.velocity.x, 0f), ForceMode2D.Impulse);
            Thread cooldown = new Thread(new ThreadStart(DashCooldown));
            cooldown.Start();
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (m_Jumps < m_MaxJumps && collision.gameObject.CompareTag("Ground")) {
            ReplenishJumps();
        }
    }
    void ReplenishJumps() {
        m_Jumps = m_MaxJumps;
    }
    void DashCooldown() {
        m_DashCooldown = true;
        Thread.Sleep(1000);
        m_DashCooldown = false;
    }
}