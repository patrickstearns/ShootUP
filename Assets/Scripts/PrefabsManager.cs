using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsManager : MonoBehaviour {

    public GameObject CoinSparkle, GoalBoom, GatePoof, GhostCoin, BallKnock, BallPoof;

    public AudioClip[] Pops;
    public AudioClip BallKnockSFX;
    public AudioClip TitleBGM, PlayBGM;

    public Material FeltMaterial, ButtonOnMaterial, ButtonOffMaterial, GhostMaterial;

    public PhysicMaterial FeltPhysicMaterial;

    private static PrefabsManager _instance;
    public static PrefabsManager Instance { get { return _instance; } }
    private void Awake() {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else{ 
            _instance = this; 
            DontDestroyOnLoad(this);
        }
    }

}
