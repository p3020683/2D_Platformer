using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {
    public int m_Damage = 1;
    PlayerHealth m_PlrHp;

    void Start() {
        
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {

        }
    }
}
