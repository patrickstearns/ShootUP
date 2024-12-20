using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredWire : Triggered {

    public Color TriggeredColor, UntriggeredColor;

    private LineRenderer lineRenderer;

    protected void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = UntriggeredColor;
        lineRenderer.endColor = UntriggeredColor;
    }

    public override void Turn(float turnValue) { }
    public override void Action(bool actioned) { }

    //TODO: can we make an energy pulsing material?
    public override void Trigger(GameObject triggering) { 
        lineRenderer.startColor = TriggeredColor;
        lineRenderer.endColor = TriggeredColor;    
    }

    public override void ResetTrigger(GameObject triggering) { 
        lineRenderer.startColor = UntriggeredColor;
        lineRenderer.endColor = UntriggeredColor;    
    }

}
