using System;
using System.Collections.Generic;
using UnityEngine;

public class LogicalTriggered : Triggered {

    public enum LogicalOperation { AND, OR, NOT }

    public LogicalOperation Operation;
    public List<GameObject> Inputs;
    public List<Triggered> Outputs;

    private Dictionary<GameObject, bool> inputs = new Dictionary<GameObject, bool>();
    private bool currentOutput = false;

    protected void Start() {
        foreach (GameObject gameObject in Inputs) 
            inputs.Add(gameObject, false);
    }

    public override void Turn(float turnValue) { }
    public override void Action(bool actioned) { }

    public override void Trigger(GameObject triggering) { 
        inputs[triggering] = true;
        eval();
    }

    public override void ResetTrigger(GameObject triggering) { 
        inputs[triggering] = false;
        eval();
    }

    private void eval() {
        bool newOutput = false;
        switch (Operation) {
            case LogicalOperation.AND: default:
                newOutput = true;
                foreach (bool value in inputs.Values)
                    if (!value) 
                        newOutput = false;
                break;
            case LogicalOperation.OR:
                newOutput = false;
                foreach (bool value in inputs.Values)
                    if (value) 
                        newOutput = true;
                break;
            case LogicalOperation.NOT:
                newOutput = false;
                if (inputs.Values.Count == 0) 
                    newOutput = true;
                else 
                    foreach (bool value in inputs.Values) //there should only be one
                        newOutput = !value;
                break;
        }

        if (newOutput != currentOutput) {
            if (newOutput) 
                foreach (Triggered triggered in Outputs)
                    triggered.Trigger(gameObject);
            else
                foreach (Triggered triggered in Outputs)
                    triggered.ResetTrigger(gameObject);
        }
    }

}
