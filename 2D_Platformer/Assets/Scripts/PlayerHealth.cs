using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] Vector3 _respawnPoint = Vector3.zero;
    [SerializeField] TMP_Text _deathText;
    [SerializeField] QuestionManager _questionManager;
    [Header("NOTE: MaxHP is evaluated as\nhpImgs.Length * (hpStates.Length - 1)")]
    [SerializeField] Image[] _hpImgs;
    [SerializeField] Texture2D[] _hpTexs;

    Sprite[] _hpSprites;
    int _stateCount;
    int _hp, _maxHp;
    PlayerController _controller;

    public int Hp { get => _hp; private set { _hp = value; UpdateHealthUI(); } }

    void Start() {
        _hpSprites = new Sprite[_hpTexs.Length];
        for (int i = 0; i < _hpTexs.Length; i++) {
            Texture2D tex = _hpTexs[i];
            _hpSprites[i] = Sprite.Create(tex, new(0f, 0f, tex.width, tex.height), new(0.5f, 0.5f));
        }

        _stateCount = _hpSprites.Length - 1;
        _maxHp = _hpImgs.Length * (_stateCount);
        Hp = _maxHp;

        _controller = GetComponent<PlayerController>();
    }
    void UpdateHealthUI() {
        for (int i = 0; i < _hpImgs.Length; i++) {
            int hpMin = i * _stateCount;
            int relHp = Mathf.Clamp(_hp - hpMin, 0, _stateCount);
            _hpImgs[i].sprite = _hpSprites[relHp];
        }
    }
    public void Damage(int dmg) {
        Hp = Mathf.Clamp(_hp - dmg, 0, _maxHp);
        if (_hp <= 0) { Kill(); }
    }
    public void Kill() {
        if (_hp > 0) { Hp = 0; }
        _deathText.gameObject.SetActive(true);
        _controller.captureInput = false;
        _controller.ResetPhysicsCache();
        Invoke("Respawn", 1);
    }
    void Respawn() {
        gameObject.transform.position = _respawnPoint;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        _controller.captureInput = true;
        _deathText.gameObject.SetActive(false);
        _questionManager.NewQuestion();
        Hp = _maxHp;
    }
    void Reload() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}