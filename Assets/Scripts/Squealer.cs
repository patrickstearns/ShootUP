using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squealer : MonoBehaviour {

    public void OnCollisionEnter(Collision collision) {
        Debug.Log(name+" collided with "+collision.collider.name);
    }

}
