using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] Vector3 _respawnPoint = Vector3.zero;
    [SerializeField] TMP_Text _healthText;
    [SerializeField] TMP_Text _deathText;
    [SerializeField] QuestionManager _questionManager;
    [Header("NOTE: MaxHP is evaluated as\nhpImgs.Length * (hpStates.Length - 1)")]
    [SerializeField] Image[] _hpImgs;
    [SerializeField] Texture2D[] _hpStates;

    int _hp, _maxHp;
    PlayerController _controller;

    void Start() {
        _maxHp = _hpImgs.Length * (_hpStates.Length - 1);
        SetHp(_maxHp);
        _controller = GetComponent<PlayerController>();
    }
    void UpdateHealthUI() {
        _healthText.text = _hp + " HP";
        // add hpimgs logic
    }
    void print<T>(T var) {
        Debug.Log(var);
    }
    void SetHp(int value) {
        _hp = value;
        UpdateHealthUI();
    }
    public void Damage(int dmg) {
        SetHp(Mathf.Clamp(_hp - dmg, 0, _maxHp));
        if (_hp <= 0) { Kill(); }
    }
    public void Kill() {
        if (_hp > 0) { SetHp(0); }
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
        SetHp(_maxHp);
    }
    void Reload() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}