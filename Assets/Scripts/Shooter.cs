using UnityEngine;

public class Shooter : Widget {

    public float ShootStrength = 1f;
    public float MaxRotationDegrees = 80f;

    public Material ActiveTriangle, InactiveTriangle;
    public Detector Detector;
    public Renderer LeftArrow, RightArrow, UpArrow;
    public AudioSource Warbler, Buzzer, Failer;

    private float ActionedTime = 0, lastTurnAmount = 0;
    private bool actioned = false;

    public override void Turn(float turnAmount) {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.z += turnAmount;
        if (euler.z > 180) euler.z -= 360;
        if (euler.z < -MaxRotationDegrees) euler.z = -MaxRotationDegrees;
        if (euler.z > MaxRotationDegrees) euler.z = MaxRotationDegrees;
        transform.rotation = Quaternion.Euler(euler);

        LeftArrow.material = turnAmount > 0 ? ActiveTriangle : InactiveTriangle;
        RightArrow.material = turnAmount < 0 ? ActiveTriangle : InactiveTriangle;

        if (turnAmount != 0 && lastTurnAmount == 0) Buzzer.Play();
        else if (turnAmount == 0 && lastTurnAmount != 0) Buzzer.Stop();

        lastTurnAmount = turnAmount;
    }

    public override void Action(bool actioned) {
        bool wasActioned = this.actioned;
        this.actioned = actioned;
        if (actioned){
            if (!wasActioned) {
                ActionedTime = Time.time;
                Warbler.Play();
            }
        }
        else {
            if (Detector.ball != null && wasActioned) {
                float heldTime = Mathf.Clamp(Time.time - ActionedTime, 0.001f, 2f);
                Detector.ball.GetComponent<Rigidbody>().AddForce(transform.up * ShootStrength * heldTime, ForceMode.Impulse);
                GetComponent<AudioSource>().Play();
                GameController.Instance.ShotsTaken++;
            }

            Warbler.Stop();
            ActionedTime = 0;
        }
    }

    public override bool HasBall() { 
        return Detector.ball != null; 
    }

    protected void Update() {
        if (actioned) {
            UpArrow.material = ActiveTriangle;
            Vector3 scale = UpArrow.transform.localScale;
            float heldTime = Mathf.Clamp(Time.time - ActionedTime, 0.001f, 2f);
            scale.z = 1+heldTime;
            UpArrow.transform.localScale = scale;

            Warbler.pitch = heldTime/2f;

            if (Time.time - ActionedTime >= 2f) {
                Failer.Play();                
                Instantiate(PrefabsManager.Instance.GatePoof, transform.position, Quaternion.identity);
                this.actioned = false;
                Warbler.Stop();
                ActionedTime = 0;
            }
        }
        else {
            UpArrow.material = InactiveTriangle;
            UpArrow.transform.localScale = Vector3.one;
        }
    }
}