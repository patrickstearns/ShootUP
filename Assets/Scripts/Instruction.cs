using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Instruction : MonoBehaviour {

    public float FadeTime = 0.25f;
    public float BallDistance = 4f;

    private Color initialColor, transparent = new Color(0f, 0f, 0f, 0f);
    
    private TextMeshPro textMeshPro;
    private bool visible = false;
    private float visibleTime = 0f;

    void Start() {
        textMeshPro = GetComponent<TextMeshPro>();
        initialColor = textMeshPro.color;
        visible = false;
        visibleTime = 0f;
    }

    void Update() {
        float a = 0;

        bool wasVisible = visible;
        visible = Vector3.Distance(GameObject.FindWithTag("Ball").transform.position, transform.position) < BallDistance &&
            GameController.Instance.GameMode && GameController.Instance.Controls.Gameplay.enabled;
        if (visible != wasVisible) visibleTime = Time.time;

        if (visible) {
            if (Time.time - visibleTime < FadeTime) 
                a = (Time.time - visibleTime) / FadeTime;
            else a = 1;
        }   
        else {
            if (Time.time - visibleTime < FadeTime) 
                a = 1 - ((Time.time - visibleTime) / FadeTime);
            else a = 0;
        }
        textMeshPro.color = new Color(initialColor.r, initialColor.g, initialColor.b, a);
    }

}
