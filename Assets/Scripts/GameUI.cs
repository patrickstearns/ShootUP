using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour {

    public GameObject Logo, PressSpace, Goal, CoinIcon, ShotIcon;
    public TextMeshProUGUI Coins, LevelNumber, Shots;
    public Wiper Wiper;
    public AudioSource Boomer;

    private static GameUI _instance;
    public static GameUI Instance { get { return _instance; } }
    private void Awake() {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }

    protected void Start() {
        Goal.SetActive(false);
    }

    protected void Update() {
        Logo.SetActive(!GameController.Instance.GameMode);
        PressSpace.SetActive(!GameController.Instance.GameMode);
        CoinIcon.SetActive(GameController.Instance.GameMode);
        ShotIcon.SetActive(GameController.Instance.GameMode);

        Coins.enabled = GameController.Instance.GameMode;
        Coins.text = ""+GameController.Instance.CoinsHeld;

        Shots.enabled = GameController.Instance.GameMode;
        Shots.text = ""+GameController.Instance.ShotsTaken;
    }
}
