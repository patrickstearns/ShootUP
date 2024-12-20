using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public bool InitiallyPressed = false;
    public bool StaysPressed = true;
    public float UnpressesAfterSeconds = 0f;
    public float SpringSpeed = 0.03f;
    public List<Triggered> Triggered;

    public Transform ButtonContainer;
    public MeshRenderer ButtonMeshRenderer;
    public Light Light;

    public bool pressed;
    private Vector3 pressedPosition = new Vector3(0f, -0.4f, 0f);
    private float pressedAtTime = 0f;

    void Start() {
        if (InitiallyPressed) {
            pressed = true;
            ButtonContainer.localPosition = pressedPosition;
            pressedAtTime = Time.time;
        }
    }

    void Update() {
        //constrain to only move on relative y axis
        ButtonContainer.localPosition = new Vector3(0f, ButtonContainer.localPosition.y, 0f);

        //constrain y & z movement
        if (ButtonContainer.localPosition.y < pressedPosition.y){
            ButtonContainer.localPosition = pressedPosition;
            ButtonContainer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else if (ButtonContainer.localPosition.y > 0){
            ButtonContainer.localPosition = Vector3.zero;
            ButtonContainer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        //spring back out if not or hold in place if so
        if (!pressed) ButtonContainer.GetComponent<Rigidbody>().AddForce(ButtonContainer.up * SpringSpeed, ForceMode.Force);
        else ButtonContainer.localPosition = pressedPosition;

        //are we in enough to be pressed
        if (ButtonContainer.localPosition.y <= pressedPosition.y && !pressed) {
            pressed = true;
            pressedAtTime = Time.time;
            foreach (Triggered triggered in Triggered)
                triggered.Trigger(gameObject);
            ButtonMeshRenderer.material = PrefabsManager.Instance.ButtonOffMaterial;
            GetComponent<AudioSource>().Play();
        }

        //if we don't stay pressed, don't stay pressed
        else if (!StaysPressed && pressed) Unpress();

        //if we unpress after some time and it's been some time, unpress
        else if (UnpressesAfterSeconds > 0 && Time.time - pressedAtTime >= UnpressesAfterSeconds && pressed) Unpress();

        Light.enabled = !pressed;
    }

    public void Unpress() {
        pressed = false;
        foreach (Triggered triggered in Triggered)
            triggered.ResetTrigger(gameObject);
        ButtonMeshRenderer.material = PrefabsManager.Instance.ButtonOnMaterial;
        ButtonContainer.localPosition = pressedPosition + new Vector3(0f, 0.1f, 0f);
        GetComponent<AudioSource>().Play();
    }

}
