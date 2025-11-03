using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] float _speed = 10f;
    [SerializeField] float _accel = 20f;
    [SerializeField] float _drag = 5f;
    [SerializeField] float _jumpPwr = 5f;
    [SerializeField] int _jumpsMax = 2;
    [SerializeField] float _dashPwr = 40f;
    [SerializeField] float _dashCdMax = 1f;

    [System.NonSerialized] public bool captureInput = true;
    Rigidbody2D _rb;
    PhysicsCache _phys;
    int _jumps;
    float _dashCd;

    struct PhysicsCache {
        public float velX;
        public bool jump;
        public bool sprint;
        public Vector2 knockback;
    }

    public int Jumps { get => _jumps; }
    public float DashCd { get => Mathf.Clamp(_dashCdMax - _dashCd, 0f, _dashCdMax); }
    public Vector2 Knockback { set => _phys.knockback = value; }

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _jumps = _jumpsMax;
    }
    void FixedUpdate() {
        _rb.AddForceX((_phys.velX * _speed - _rb.linearVelocityX) * _accel);
        if (Mathf.Approximately(_phys.velX, 0f)) {
            _rb.linearVelocityX = Mathf.MoveTowards(_rb.linearVelocityX, 0f, _drag * Time.fixedDeltaTime);
        }

        if (_phys.jump) {
            if (_jumps > 0) {
                _rb.linearVelocityY = Mathf.Max(_jumpPwr, _jumpPwr + _rb.linearVelocityY);
                --_jumps;
            }
            _phys.jump = false;
        }

        if ((_dashCd <= 0 || (_dashCd -= Time.fixedDeltaTime) <= 0) && _phys.sprint && _phys.velX != 0) {
            _rb.AddForceX(_dashPwr * Mathf.Sign(_phys.velX) + _rb.linearVelocityX, ForceMode2D.Impulse);
            _dashCd = _dashCdMax;
            _phys.sprint = false;
        }

        if (_phys.knockback != Vector2.zero) {
            _rb.AddForce(new(_phys.knockback.x, Mathf.Clamp(_phys.knockback.y, -7.5f, 7.5f)), ForceMode2D.Impulse);
            _phys.knockback = Vector2.zero;
        }
    }
    void Update() {
        if (captureInput) {
            _phys.velX = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump")) { _phys.jump = true; }
            if (Input.GetButtonDown("Sprint")) { _phys.sprint = true; }
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground") && _jumps < _jumpsMax
            || collision.gameObject.CompareTag("Hazard")) { _jumps = 0; }
    }
    public void GroundColliderTriggerEnter() => _jumps = _jumpsMax;
    public void GroundColliderTriggerExit() => _jumps -= _jumps == _jumpsMax ? 1 : 0;
    public void ResetPhysicsCache() {
        _phys = new();
    }
}
