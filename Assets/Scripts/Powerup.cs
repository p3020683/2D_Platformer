using UnityEngine;

public class Powerup : MonoBehaviour {
    [SerializeField] PlayerController _playerController;
    [SerializeField] bool _incMaxJumps;
    [SerializeField] bool _decDashCd;
    [SerializeField] bool _unlockWallJump;

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (_incMaxJumps) { _playerController.IncrementMaxJumps(); }
            if (_decDashCd) { _playerController.DecrementDashCd(); }
            if (_unlockWallJump) { _playerController.UnlockCanWallJump(); }
            Destroy(gameObject);
        }
    }
}
