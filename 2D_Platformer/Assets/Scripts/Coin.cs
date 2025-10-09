using UnityEngine;

public class Coin : MonoBehaviour {
    public int m_Value = 1;

    private void OnTriggerEnter2D(Collider2D collision) {
        // Only trigger if the player touches the coin
        if (collision.gameObject.CompareTag("Player")) {
            // Find the ScoreManager and add points
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

            if (scoreManager != null) {
                scoreManager.AddScore(m_Value);
            }

            // Destroy the coin so it disappears
            Destroy(gameObject);
        }
    }
}