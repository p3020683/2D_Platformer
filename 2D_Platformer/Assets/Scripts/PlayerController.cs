using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float m_Speed = 10f;
    public float m_Accel = 20f;
    public float m_Drag = 5f;
    public float m_JumpPwr = 5f;
    public int m_JumpsMax = 2;
    public float m_DashPwr = 40f;
    public float m_DashCdMax = 1f;
    [System.NonSerialized] public bool m_CaptureInput = true;

    Rigidbody2D m_Rb;
    PhysicsCache m_Phys;
    int m_Jumps;
    float m_DashCd;

    struct PhysicsCache {
        public float xVel;
        public bool jump;
        public bool sprint;
        public Vector2 knockback;
    }

    public int Jumps { get => m_Jumps; }
    public float DashCd { get => Mathf.Clamp(m_DashCdMax - m_DashCd, 0f, m_DashCdMax); }
    public Vector2 Knockback { set => m_Phys.knockback = value; }

    void Start() {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Jumps = m_JumpsMax;
    }
    void FixedUpdate() {
        m_Rb.AddForce(new((m_Phys.xVel * m_Speed - m_Rb.velocity.x) * m_Accel, 0f));
        if (Mathf.Approximately(m_Phys.xVel, 0f)) {
            m_Rb.velocity = new(Mathf.MoveTowards(m_Rb.velocity.x, 0f, m_Drag * Time.fixedDeltaTime), m_Rb.velocity.y);
        }

        if (m_Phys.jump) {
            if (m_Jumps > 0) {
                m_Rb.velocity = new(m_Rb.velocity.x, Mathf.Max(m_JumpPwr, m_JumpPwr + m_Rb.velocity.y));
                --m_Jumps;
            }
            m_Phys.jump = false;
        }

        if ((m_DashCd <= 0 || (m_DashCd -= Time.fixedDeltaTime) <= 0) && m_Phys.sprint && m_Phys.xVel != 0) {
            m_Rb.AddForce(new(m_DashPwr * Mathf.Sign(m_Phys.xVel) + m_Rb.velocity.x, 0f), ForceMode2D.Impulse);
            m_DashCd = m_DashCdMax;
            m_Phys.sprint = false;
        }

        if (m_Phys.knockback != Vector2.zero) {
            m_Rb.AddForce(new(m_Phys.knockback.x, Mathf.Clamp(m_Phys.knockback.y, -7.5f, 7.5f)), ForceMode2D.Impulse);
            m_Phys.knockback = Vector2.zero;
        }
    }
    void Update() {
        if (m_CaptureInput) {
            m_Phys.xVel = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump")) { m_Phys.jump = true; }
            if (Input.GetButtonDown("Sprint")) { m_Phys.sprint = true; }
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (m_Jumps < m_JumpsMax && collision.gameObject.CompareTag("Ground")) {
            m_Jumps = m_JumpsMax;
        }
    }
    public void ResetPhysicsCache() {
        m_Phys = new();
    }
}
