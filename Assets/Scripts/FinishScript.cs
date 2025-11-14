using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScript : MonoBehaviour {
    [SerializeField] TMP_Text _endText;
    [SerializeField] PlayerController _playerController;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            _endText.text = "Level Complete!";
            _playerController.captureInput = false;
            _endText.gameObject.SetActive(true);
        }
    }
    public void NextLevel() {
        // ideally load next level; as placeholder reload active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
