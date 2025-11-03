using UnityEngine;

public class PlayerGroundColliderScript : MonoBehaviour {
    [SerializeField] PlayerController _playerController;

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            _playerController.GroundColliderTriggerEnter();
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            _playerController.GroundColliderTriggerExit();
        }
    }
}
