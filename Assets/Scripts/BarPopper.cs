using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarPopper : MonoBehaviour {
    public GameObject PopoutContainer;
    public Detector Detector;

    private GameObject ballToPop;

    public float PopForce = 40f;

    private bool popping = false, poppedBall = false;
    private float popTime = 0;
    private Vector3 initialPopoutPos;

    protected void Start() {
        popping = false;
        initialPopoutPos = PopoutContainer.transform.position;
    }

    protected void Update() {
        if (!popping && Detector.ball != null) {
            popping = true;
            popTime = Time.time;
            ballToPop = Detector.ball;
        }

        if (popping) {
            float t = Time.time - popTime;

            if (t < 0.05f) {
                PopoutContainer.transform.position = Vector3.Lerp(initialPopoutPos, initialPopoutPos + transform.up * 0.5f, t/0.05f);

                if (!poppedBall) {
                    poppedBall = true;
                    Vector3 outward = transform.up * PopForce;
                    ballToPop.GetComponent<Rigidbody>().AddForce(outward, ForceMode.Impulse);
                    ballToPop = null;
                }

                GetComponent<AudioSource>().clip = PrefabsManager.Instance.Pops[Random.Range(0, PrefabsManager.Instance.Pops.Length)];
                GetComponent<AudioSource>().Play();
            }
            else if (t < 0.1f) {
                PopoutContainer.transform.position = initialPopoutPos + transform.up * 0.5f;
            }
            else if (t < 0.2f) {
                PopoutContainer.transform.position = Vector3.Lerp(initialPopoutPos + transform.up * 0.5f, initialPopoutPos, (t-0.1f)*10f);
            }
            else {
                PopoutContainer.transform.position = initialPopoutPos;
                popping = false;
                poppedBall = false;
            }
        }
    }
}
