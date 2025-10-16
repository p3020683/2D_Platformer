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
        public float xVel;
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
        _rb.AddForce(new((_phys.xVel * _speed - _rb.velocity.x) * _accel, 0f));
        if (Mathf.Approximately(_phys.xVel, 0f)) {
            _rb.velocity = new(Mathf.MoveTowards(_rb.velocity.x, 0f, _drag * Time.fixedDeltaTime), _rb.velocity.y);
        }

        if (_phys.jump) {
            if (_jumps > 0) {
                _rb.velocity = new(_rb.velocity.x, Mathf.Max(_jumpPwr, _jumpPwr + _rb.velocity.y));
                --_jumps;
            }
            _phys.jump = false;
        }

        if ((_dashCd <= 0 || (_dashCd -= Time.fixedDeltaTime) <= 0) && _phys.sprint && _phys.xVel != 0) {
            _rb.AddForce(new(_dashPwr * Mathf.Sign(_phys.xVel) + _rb.velocity.x, 0f), ForceMode2D.Impulse);
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
            _phys.xVel = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump")) { _phys.jump = true; }
            if (Input.GetButtonDown("Sprint")) { _phys.sprint = true; }
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (_jumps < _jumpsMax && collision.gameObject.CompareTag("Ground")) {
            _jumps = _jumpsMax;
        }
    }
    public void ResetPhysicsCache() {
        _phys = new();
    }
}
