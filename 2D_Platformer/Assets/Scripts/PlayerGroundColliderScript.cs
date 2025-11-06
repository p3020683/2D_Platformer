using UnityEngine;

public class PlayerGroundColliderScript : MonoBehaviour {
    [SerializeField] PlayerController _playerController;

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            _playerController.GroundColliderEnter();
        }
    }
    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            _playerController.GroundColliderExit();
        }
    }

}
