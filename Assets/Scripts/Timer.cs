using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : Triggered {

    public GameObject Face, Backface;
    public AudioSource Ticker, Dinger;
    public float TimeSeconds;

    private bool triggered = false;
    private float triggeredTime = 0f, lastTickTime = 0f, tickTime = 0f;

    public override void Turn(float turnValue) { }
    public override void Action(bool actioned) { }
    public override void Trigger(GameObject triggering) { setTriggered(true); }
    public override void ResetTrigger(GameObject triggering) { } //setTriggered(false); }

    protected void Start() { setTriggered(false); }

    void Update() {
        float r = 1f;
        if (triggered) {
            if (Time.time - triggeredTime < TimeSeconds) {
                r = (Time.time - triggeredTime) / TimeSeconds;                

                //tick goes from 1/s to 8/s
                if (Time.time - lastTickTime >= tickTime) {
                    Ticker.Play();
                    lastTickTime = Time.time;
                    tickTime = 1 / (1 + r * 7);
                }
            }
            else {
                Dinger.Play();
                triggered = false;
            }
        }
        Face.transform.localScale = new Vector3(1-r, 1-r, 0.001f);

        Backface.GetComponent<Renderer>().material.color = (r == 1) ? Color.black : Color.white;
        if (r == 1) Backface.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        else Backface.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
    }

    private void setTriggered(bool triggered) {
        bool wasTriggered = this.triggered;
        this.triggered = triggered;

        if (!wasTriggered && triggered) {
            triggeredTime = Time.time;
            lastTickTime = Time.time;
            tickTime = 1f;
        }
    }
}
