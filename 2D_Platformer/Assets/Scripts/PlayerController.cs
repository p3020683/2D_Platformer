using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [System.NonSerialized] public bool captureInput = true;

    [SerializeField] float _speed = 10f;
    [SerializeField] float _accel = 20f;
    [SerializeField] float _drag = 5f;
    [SerializeField] float _jumpPwr = 5f;
    [SerializeField] float _dashPwr = 40f;

    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Animator _animator;
    PhysicsCache _phys;
    [SerializeField] Abilities _abil;
    int _jumps;
    float _dash;
    bool _wallJumped;
    bool _updateSpriteFlipX = true;

    public int JumpState { get => _jumps; }
    public float DashState { get => Mathf.Clamp(_abil.DashCd - _dash, 0f, _abil.DashCd); }
    public Vector2 Knockback { set => _phys.Knockback = value; }
    public bool Grounded { get => _jumps == _abil.MaxJumps;
        private set { if (value) {
            _jumps = _abil.MaxJumps; _wallJumped = false; _animator.SetTrigger("onGrounded");
        } }
    }

    struct PhysicsCache {
        public float VelX;
        public bool Jump;
        public bool Sprint;
        public Vector2 Knockback;
    }
    [Serializable]
    struct Abilities {
        public int MaxJumps;
        public float DashCd;
        public bool CanWallJump;
    }

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _abil.MaxJumps = 1;
        Grounded = true;
    }
    void FixedUpdate() {
        _rb.AddForceX((_phys.VelX * _speed - _rb.linearVelocityX) * _accel);
        if (Mathf.Approximately(_phys.VelX, 0f)) {
            _rb.linearVelocityX = Mathf.MoveTowards(_rb.linearVelocityX, 0f, _drag * Time.fixedDeltaTime);
            _animator.SetBool("isRunning", false);
        }
        else { _animator.SetBool("isRunning", _rb.linearVelocityX != 0f); }

        if (_phys.Jump) {
            if (_jumps > 0) {
                _rb.linearVelocityY = Mathf.Max(_jumpPwr, _jumpPwr + _rb.linearVelocityY);
                _jumps--;
                _animator.SetTrigger("onJump");
            }
            _phys.Jump = false;
        }

        if (_abil.DashCd > 0) {
            if ((_dash <= 0 || (_dash -= Time.fixedDeltaTime) <= 0) && _phys.Sprint && _phys.VelX != 0) {
                _rb.AddForceX(_dashPwr * Mathf.Sign(_phys.VelX) + _rb.linearVelocityX, ForceMode2D.Impulse);
                _dash = _abil.DashCd;
            }
            _phys.Sprint = false;
        }

        if (_phys.Knockback != Vector2.zero) {
            _rb.AddForce(new(_phys.Knockback.x, Mathf.Clamp(_phys.Knockback.y, -7.5f, 7.5f)), ForceMode2D.Impulse);
            _phys.Knockback = Vector2.zero;
            _animator.SetTrigger("onHit");
        }
    }
    void Update() {
        if (captureInput) {
            _phys.VelX = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump")) { _phys.Jump = true; }
            if (_abil.DashCd > 0 && Input.GetButtonDown("Sprint")) { _phys.Sprint = true; }
        }
        if (_updateSpriteFlipX && _phys.VelX != 0f) { _spriteRenderer.flipX = Mathf.Sign(_phys.VelX) < 0f; }
        _animator.SetBool("isFalling", _rb.linearVelocityY < -0.1f);
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (!Grounded) {
            if (collision.gameObject.CompareTag("Ground")) { _jumps = 0; }
            else if (collision.gameObject.CompareTag("WallJump")) {
                if ( _abil.CanWallJump && !_wallJumped) {
                    _jumps = 1;
                    _wallJumped = true;
                    _updateSpriteFlipX = false;
                    _spriteRenderer.flipX = Mathf.Sign(collision.GetContact(0).normal.x) == -1f;
                    _animator.SetBool("isWallSliding", true);
                }
                else { _jumps = 0; }
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("WallJump") && _abil.CanWallJump && _wallJumped) {
            _rb.AddForce(new(_jumpPwr * (Mathf.Sign(collision.GetContact(0).normal.x) == -1f ? -1f : 1f), _jumpPwr));
            _updateSpriteFlipX = true;
            _animator.SetBool("isWallSliding", false);
        }
    }
    public void GroundColliderEnter() => Grounded = true;
    public void GroundColliderExit() => _jumps -= Grounded ? _abil.MaxJumps - 1 : 0;
    public void ResetPhysicsCache() => _phys = new();
}
