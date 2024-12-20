using UnityEngine;

public class DestroyWhenDone : MonoBehaviour {

    private AudioSource audioSource;
    private float startTime;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        startTime = Time.time;
    }

    void Update() {
        if (Time.time - startTime > audioSource.clip.length)
            Destroy(gameObject);
    }
}
