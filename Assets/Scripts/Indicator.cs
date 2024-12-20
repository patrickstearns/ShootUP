using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : Triggered {

    public Material OnMaterial, OffMaterial;
    public MeshRenderer IndicatorMesh;
    public Light Light;

    public override void Turn(float turnValue) { }
    public override void Action(bool actioned) { }
    public override void Trigger(GameObject triggering) { setTriggered(true); }
    public override void ResetTrigger(GameObject triggering) { setTriggered(false); }

    protected void Start() { setTriggered(false); }

    private void setTriggered(bool triggered) {
        IndicatorMesh.material = triggered ? OnMaterial : OffMaterial;
        Light.enabled = triggered;
   }
}
