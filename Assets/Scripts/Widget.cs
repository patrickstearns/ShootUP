using UnityEngine;

public abstract class Widget : MonoBehaviour {
    public abstract void Turn(float turnValue);
    public abstract void Action(bool actioned);
    public virtual bool HasBall() { return false; }
}
