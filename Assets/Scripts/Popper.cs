using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popper : MonoBehaviour {

    public GameObject PopoutContainer;
    public Detector Detector;

    private GameObject ballToPop;

    public float PopForce = 40f;

    private bool popping = false, poppedBall = false;
    private float popTime = 0;

    protected void Start() {
        popping = false;
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
                float s = 1 + t * 20;
                PopoutContainer.transform.localScale = new Vector3(s, s, 1);

                if (!poppedBall) {
                    poppedBall = true;
                    Vector3 outward = (ballToPop.transform.position - transform.position).normalized * PopForce;
                    ballToPop.GetComponent<Rigidbody>().AddForce(outward, ForceMode.Impulse);
                    ballToPop = null;
                }

                GetComponent<AudioSource>().clip = PrefabsManager.Instance.Pops[Random.Range(0, PrefabsManager.Instance.Pops.Length)];
                GetComponent<AudioSource>().Play();
            }
            else if (t < 0.1f) {
                PopoutContainer.transform.localScale = new Vector3(2, 2, 1);
            }
            else if (t < 0.2f) {
                float s = 2 - (t - 0.1f) * 10f;
                PopoutContainer.transform.localScale = new Vector3(s, s, 1);
            }
            else {
                PopoutContainer.transform.localScale = new Vector3(1, 1, 1);
                popping = false;
                poppedBall = false;
            }
        }
    }
}
