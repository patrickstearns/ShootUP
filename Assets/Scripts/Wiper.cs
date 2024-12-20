using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wiper : MonoBehaviour {

    public TextMeshProUGUI Level;

    private RectTransform rectTransform;
    private bool wipingIn = false, wipingOut = false, wipedIn = false, wipedOut = true;
    private float wipeTime;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(0f, -600f, 0f);
    }

    void Update() {
        float wipeDuration = 0.25f;
        float t = 0f;

        if (wipingIn) {
            float r = (Time.time - wipeTime)/wipeDuration;
            t = r/2;
            if (r >= 1){
                wipingIn = false;
                wipedIn = true;
            }
        }
        else if (wipedIn) {
            t = 0.5f;            
        }
        else if (wipingOut) {
            float r = (Time.time - wipeTime)/wipeDuration;
            t = 0.5f + r/2;
            if (r >= 1){
                wipingOut = false;
                wipedOut = true;
            }
        }
        else if (wipedOut) {
            t = 1f;
        }

        rectTransform.anchoredPosition = Vector3.Lerp(new Vector3(0f, -600f, 0f), new Vector3(0f, 600f, 0f), t);
        Level.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(new Vector3(0f, 600f, 0f), new Vector3(0f, -600f, 0f), t);

        Level.text = "Level "+GameController.Instance.CurrentLevel;
    }

    public void WipeIn() {
        wipedOut = false;
        wipingIn = true;
        wipeTime = Time.time;
    }

    public void WipeOut() {
        wipedIn = false;
        wipingOut = true;
        wipeTime = Time.time;
    }

}
