using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : Widget {

    public float ActivateTime = 1f;

    public Detector detector;
    public CinemachineVirtualCamera GoalCam;
    public AudioSource BoomerAudioSource;

    private bool activated = false, triggered = false;
    private float activatedTime = 0f;

    public override void Turn(float turnValue) {}
    public override void Action(bool actioned) {}

    protected void Update() {
        if (detector.ball == null) {
            activated = false;
        }
        else if (!activated) {
            activated = true;
            activatedTime = Time.time;
        }
        else if (activated && !triggered && Time.time-activatedTime >= ActivateTime) {
            GetComponent<AudioSource>().Play();

            triggered = true;
            GameController.Instance.Goal();
        }
    }

}
