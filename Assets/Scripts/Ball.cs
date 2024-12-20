using UnityEngine;

public class Ball : MonoBehaviour {

    public LineRenderer Trail;
    public AudioSource Warbler, Teleport;

    private bool touching = false;
    private float trailDropTime = 0f;

    protected void OnCollisionEnter(Collision collision) {
        Instantiate(PrefabsManager.Instance.BallKnock, collision.contacts[0].point, Quaternion.identity);
        GetComponent<AudioSource>().Play();
        touching = true;
    }

    protected void OnCollisionExit(Collision collision) {
        GetComponent<AudioSource>().Stop();
        touching = false;
    }

    protected void Start() {
        ResetTrail();
    }

    protected void Update() {
        if (touching) {
            float m = GetComponent<Rigidbody>().velocity.magnitude;
            GetComponent<AudioSource>().volume = Mathf.Clamp(m, 0, 10)/5f;
        }        

        if (Time.time - trailDropTime > 0.0125f) {
            Vector3[] oldPositions = new Vector3[Trail.positionCount];
            Vector3[] positions = new Vector3[Trail.positionCount];
            Trail.GetPositions(oldPositions);
            positions[0] = transform.position;
            for (int i = 1; i < Trail.positionCount; i++) positions[i] = oldPositions[i-1];
            Trail.SetPositions(positions);
            trailDropTime = Time.time;
        }
    }

    public void ResetTrail() {
        Trail.positionCount = 40;
        Vector3[] positions = new Vector3[Trail.positionCount];
        for (int i = 0; i < Trail.positionCount; i++) positions[i] = transform.position;
        Trail.SetPositions(positions);
        trailDropTime = Time.time;
    }

}
