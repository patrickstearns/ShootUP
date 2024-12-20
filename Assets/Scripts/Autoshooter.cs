using System.Collections;
using UnityEngine;

public class Autoshooter : Widget {

    public Shooter TargetShooter;

    public Detector Detector;
    public AudioSource Boomer, Warbler, Buzzer;

    private bool activated = false;

    public override void Turn(float turnAmount) { }
    public override void Action(bool actioned) { }

    public override bool HasBall() { 
        return false; //we don't want this to let the cam change 
    }

    protected void Update() {
        if (Detector.ball == null) {
            activated = false;
        }
        else if (!activated) {
            activated = true;
            StartCoroutine(autoshoot());
        }
    }

    private IEnumerator autoshoot() {
        Ball currentBall = Detector.ball.GetComponent<Ball>();
      
        //disable controls, turn off ball gravity and collider
        GameController.Instance.Controls.Gameplay.Disable();
        currentBall.GetComponent<Rigidbody>().useGravity = false;
        currentBall.GetComponent<Collider>().enabled = false;

        //pull ball to our center
        float pullTime = 0.5f;
        float startTime = Time.time;
        Vector3 ballStartPos = currentBall.transform.position;
        while (Time.time - startTime < pullTime) {
            currentBall.transform.position = Vector3.Lerp(ballStartPos, transform.position, (Time.time - startTime)/pullTime);
            currentBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            yield return new WaitForEndOfFrame();
        }
        currentBall.transform.position = transform.position;
        yield return new WaitForEndOfFrame();
        currentBall.transform.position = transform.position;
        yield return new WaitForEndOfFrame();

        //unlock rotation on us and on target
        float targetMaxRotationDegrees = TargetShooter.MaxRotationDegrees;
        TargetShooter.MaxRotationDegrees = 180f;

        //rotate to point at our target shooter & rotate target to point at us
        float angleTowardTarget = Mathf.Rad2Deg * Mathf.Atan2(TargetShooter.transform.position.y - transform.position.y, TargetShooter.transform.position.x - transform.position.x);

        Vector3 startRotation = transform.rotation.eulerAngles;
        Vector3 endRotation = transform.rotation.eulerAngles;
        endRotation.z = angleTowardTarget - 90f;

        Vector3 targetStartRotation = TargetShooter.transform.rotation.eulerAngles;
        Vector3 targetEndRotation = TargetShooter.transform.rotation.eulerAngles;
        targetEndRotation.z = angleTowardTarget + 180f - 90f;

        float turnTime = 1f;
        startTime = Time.time;
        Buzzer.Play();
        while (Time.time - startTime < turnTime) {
            transform.rotation = Quaternion.Euler(Vector3.Slerp(startRotation, endRotation, (Time.time - startTime)/turnTime));
            TargetShooter.transform.rotation = Quaternion.Euler(Vector3.Slerp(targetStartRotation, targetEndRotation, (Time.time - startTime)/turnTime));
            yield return new WaitForEndOfFrame();
        }
        
        transform.rotation = Quaternion.Euler(endRotation);
        TargetShooter.transform.rotation = Quaternion.Euler(targetEndRotation);
        Buzzer.Stop();

        //charge up
        float chargeTime = 2f;
        startTime = Time.time;
        Warbler.Play();
        while (Time.time - startTime < chargeTime) {
            Warbler.pitch = (Time.time - startTime)/chargeTime;
            yield return new WaitForEndOfFrame();
        }
        Warbler.Stop();

        //fire
        Instantiate(PrefabsManager.Instance.GoalBoom, currentBall.transform.position + new Vector3(0, 0, -1), Quaternion.identity);
        Boomer.Play();

        //ball flies to target shooter; is received by it
        float flyDistance = Vector3.Distance(transform.position, TargetShooter.transform.position);
        float flyTime = flyDistance/60f; //move at 60 units/sec
        startTime = Time.time;
        while (Time.time - startTime < flyTime) {
            currentBall.transform.position = Vector3.Lerp(transform.position, TargetShooter.transform.position, (Time.time - startTime)/flyTime);
            yield return new WaitForEndOfFrame();
        }
        currentBall.transform.position = TargetShooter.transform.position;
        yield return new WaitForEndOfFrame();
        currentBall.transform.position = TargetShooter.transform.position;
        yield return new WaitForEndOfFrame();

        //rotate both to upward
        startRotation = transform.rotation.eulerAngles;
        endRotation = Vector3.zero;

        targetStartRotation = TargetShooter.transform.rotation.eulerAngles;
        targetEndRotation = Vector3.zero;

        Buzzer.Play();
        startTime = Time.time;
        while (Time.time - startTime < turnTime) {
            transform.rotation = Quaternion.Euler(Vector3.Slerp(startRotation, endRotation, (Time.time - startTime)/turnTime));
            TargetShooter.transform.rotation = Quaternion.Euler(Vector3.Slerp(targetStartRotation, targetEndRotation, (Time.time - startTime)/turnTime));
            yield return new WaitForEndOfFrame();
        }
        
        transform.rotation = Quaternion.Euler(endRotation);
        TargetShooter.transform.rotation = Quaternion.Euler(targetEndRotation);
        Buzzer.Stop();

        //unlock target, reset ourselves, turn gravity back on, reenable controls
        TargetShooter.MaxRotationDegrees = targetMaxRotationDegrees;        
        currentBall.GetComponent<Rigidbody>().useGravity = true;
        currentBall.GetComponent<Collider>().enabled = true;
        Detector.ball = null; //since we'd turned the collider off
        activated = false;
        GameController.Instance.Controls.Gameplay.Enable();
    }
}
