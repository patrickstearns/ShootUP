using UnityEngine;

public class Triggered : Widget {

    public override void Turn(float turnValue) { }
    public override void Action(bool actioned) { }

    public virtual void Trigger(GameObject triggering) { }
    public virtual void ResetTrigger(GameObject triggering) { }

}
