using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScript : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // ideally load next level; as placeholder reload active scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
