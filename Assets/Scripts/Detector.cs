using UnityEngine;

public class Detector : MonoBehaviour {

    public GameObject ball;

    public void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Ball>() != null)
            ball = other.gameObject;        
    }

    public void OnTriggerExit(Collider other) {
        if (other.GetComponent<Ball>() != null)
             ball = null;
    }

}
