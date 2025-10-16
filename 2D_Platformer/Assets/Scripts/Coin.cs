using UnityEngine;

public class Coin : MonoBehaviour {
    [SerializeField] int _value = 1;

    private void OnTriggerEnter2D(Collider2D collision) {
        // Only trigger if the player touches the coin
        if (collision.gameObject.CompareTag("Player")) {
            // Find the ScoreManager and add points
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

            if (scoreManager != null) {
                scoreManager.AddScore(_value);
            }

            // Destroy the coin so it disappears
            Destroy(gameObject);
        }
    }
}