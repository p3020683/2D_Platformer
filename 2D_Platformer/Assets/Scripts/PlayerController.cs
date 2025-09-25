using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float m_Speed = 10.0f;
    public float m_JumpPwr = 5.0f;
    public float m_DashPwr = 10.0f;
    Rigidbody2D m_Rb;
    int m_Jumps, m_MaxJumps = 2;

    void Start() {
        m_DashPwr *= 10;
        m_Rb = GetComponent<Rigidbody2D>();
        ReplenishJumps();
    }
    void Update() {
        float xInput = Input.GetAxisRaw("Horizontal");
        m_Rb.AddForceX(xInput * m_Speed - m_Rb.linearVelocityX);

        if (Input.GetButtonDown("Jump") && m_Jumps-- > 0) {
            m_Rb.AddForceY(m_JumpPwr, ForceMode2D.Impulse);
        }

        if (xInput != 0 && Input.GetButtonDown("Sprint")) {
            m_Rb.linearVelocityX = m_DashPwr * Mathf.Sign(xInput);
            Debug.Log("dash");
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
}