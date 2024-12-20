using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghostable : MonoBehaviour {

    private new Renderer renderer;
    private new Collider collider;
    private Material originalMaterial;

    public bool Ghosted = false;

    void Awake() {
        renderer = GetComponent<Renderer>();
        collider = GetComponent<Collider>();
        originalMaterial = renderer.sharedMaterial;        
    }

    public void Ghost(bool active) {
        Ghosted = !active;

        if (active) renderer.material = originalMaterial;
        else renderer.material = PrefabsManager.Instance.GhostMaterial;

        if (collider != null ) collider.enabled = active;
    }
}
