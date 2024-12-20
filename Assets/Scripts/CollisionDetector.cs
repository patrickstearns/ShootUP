using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour {

    public Ball Ball;
    public Collision Collision;

    public void OnCollisionEnter(Collision collision) { colliding(collision); }
    public void OnCollisionStay(Collision collision) { colliding(collision); }
    private void colliding(Collision collision) {
Debug.Log("Colliding!");
        if (collision.collider.GetComponent<Ball>() != null) {
            Ball = collision.collider.GetComponent<Ball>();
            Collision = collision;
        }
    }
}
