using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float m_Speed = 10f;
    public float m_JumpPwr = 5f;
    public int m_JumpsMax = 2;
    public float m_DashPwr = 10f;
    public float m_DashCdMax = 1f;

    Rigidbody2D m_Rb;
    int m_Jumps;
    float m_DashCd;

    public int Jumps { get => m_Jumps; }
    public float DashCd { get => Mathf.Clamp(m_DashCdMax - m_DashCd, 0f, m_DashCdMax); }

    void Start() {
        m_DashPwr *= 10;
        m_Rb = GetComponent<Rigidbody2D>();
        m_Jumps = m_JumpsMax;
    }
    void Update() {
        float xInput = Input.GetAxisRaw("Horizontal");
        m_Rb.AddForce(new(xInput * m_Speed - m_Rb.velocity.x, 0f));

        if (Input.GetButtonDown("Jump") && m_Jumps > 0) {
            m_Rb.velocity = new(m_Rb.velocity.x, Mathf.Max(m_JumpPwr, m_JumpPwr + m_Rb.velocity.y));
            --m_Jumps;
        }

        if ((m_DashCd <= 0 || (m_DashCd -= Time.deltaTime) <= 0) && Input.GetButtonDown("Sprint") && xInput != 0) {
            m_Rb.AddForce(new(m_DashPwr * Mathf.Sign(xInput) + m_Rb.velocity.x, 0f), ForceMode2D.Impulse);
            m_DashCd = m_DashCdMax;
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (m_Jumps < m_JumpsMax && collision.gameObject.CompareTag("Ground")) {
            m_Jumps = m_JumpsMax;
        }
        Debug.Log(DashCd);
    }
}
