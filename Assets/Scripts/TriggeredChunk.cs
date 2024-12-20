using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredChunk : Triggered {

    public enum ChunkActiveBehavior { Always, WhenTriggered, WhenNotTriggered }

    public bool InitiallyTriggered = false;
    public ChunkActiveBehavior ChunkBehavior = ChunkActiveBehavior.Always;
    public float TurnSpeed = 0f;

    public float MoveSpeed = 0f;
    public List<Transform> path = new List<Transform>();

    private bool triggered = false;
    private new Rigidbody rigidbody;
    private int currentPathDestination = 0;

    public override void Turn(float turnValue) { }
    public override void Action(bool actioned) { }
    public override void Trigger(GameObject triggering) { 
        if (ChunkBehavior != ChunkActiveBehavior.Always && !triggered)
            foreach (Ghostable ghostable in gameObject.GetComponentsInChildren<Ghostable>())
                ghostable.Ghost(ChunkBehavior == ChunkActiveBehavior.WhenTriggered); 
        triggered = true; 
    }

    public override void ResetTrigger(GameObject triggering) { 
        if (ChunkBehavior != ChunkActiveBehavior.Always && triggered)
            foreach (Ghostable ghostable in gameObject.GetComponentsInChildren<Ghostable>())
                ghostable.Ghost(ChunkBehavior == ChunkActiveBehavior.WhenNotTriggered); 
        triggered = false; 
    }

    protected void Start() {
        rigidbody = GetComponent<Rigidbody>();
        triggered = InitiallyTriggered;

        if (ChunkBehavior != ChunkActiveBehavior.Always) {
            if (InitiallyTriggered) Trigger(null);
            else{
                triggered = true; //so next call will do something
                ResetTrigger(null);
            }
        }
    }

    protected void Update() {
        if ((ChunkBehavior == ChunkActiveBehavior.Always || 
                    (triggered && ChunkBehavior == ChunkActiveBehavior.WhenTriggered) || 
                    (!triggered && ChunkBehavior == ChunkActiveBehavior.WhenNotTriggered))) {
            //if we're turning, turn
            if (TurnSpeed > 0f) {
                float zTorque = 0f;
                if (TurnSpeed > 0) {
                    if (rigidbody.angularVelocity.z > TurnSpeed) zTorque = -TurnSpeed/2f;
                    else if (rigidbody.angularVelocity.z < TurnSpeed) zTorque = 5f;
                } 
                else if (TurnSpeed < 0) {
                    if (rigidbody.angularVelocity.z > TurnSpeed) zTorque = -5f;
                    else if (rigidbody.angularVelocity.z < TurnSpeed) zTorque = TurnSpeed/2f;
                }

                rigidbody.AddRelativeTorque(new Vector3(0f, 0f, zTorque));
            }
            else rigidbody.angularVelocity = Vector3.zero;

            //if we're moving, move
            if (triggered && path.Count > 0) {
                Vector3 towardDestination = path[currentPathDestination].position - transform.position;
                float angle = Vector3.Angle(rigidbody.velocity, towardDestination);
                if (towardDestination.magnitude > 1f) {
                    if (angle > 90f || rigidbody.velocity.magnitude > towardDestination.magnitude * 2)
                        rigidbody.AddForce(-rigidbody.velocity.normalized);
                    rigidbody.AddForce(towardDestination.normalized * MoveSpeed);
                    if (rigidbody.velocity.magnitude > MoveSpeed) rigidbody.velocity = rigidbody.velocity.normalized * MoveSpeed;
                }
                else {
                    currentPathDestination++;
                    if (currentPathDestination >= path.Count) currentPathDestination = 0;
                }
            }
            else rigidbody.velocity = Vector3.zero;
        }
    }
}
