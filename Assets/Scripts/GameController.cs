using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour, Controls.IGameplayActions, Controls.IMenuActions {

    public float TurnSpeed = 10f; //TODO: move to shooter
    public float SpecialTriggerTime = 1f;
    public GameObject BallPrefab; //TODO: move to prefabs manager

    public float HelpTime = 5f;
    public Material TitleSkybox;
    public CinemachineVirtualCameraBase BallCam, LookUpCam, HighBallCam;
    public AudioSource BGMAudioSource;
    public ReflectionProbe ReflectionProbe;

    public bool GameMode = false;
    public int CoinsHeld = 0, CurrentLevel = 0, ShotsTaken = 0;

    public GameObject[] LevelPrefabs;

    public Controls Controls;
    private GameObject currentBall;
    private float turnValue, specialTime = 0f;
    private bool actionHeld, specialHeld, specialTriggered = false, levelStarted = false;

    private Stage board;
    private Widget lastShooter;

    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }
    private void Awake() {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else{ 
            _instance = this; 
            DontDestroyOnLoad(this);
        }

        if (Controls == null) Controls = new Controls();        
    }

    void OnEnable() {
        Controls.Gameplay.SetCallbacks(this);
        Controls.Menu.SetCallbacks(this);
        Controls.Menu.Enable();
    }

    void Start() {
        RenderSettings.skybox = TitleSkybox;
        DynamicGI.UpdateEnvironment();
        ReflectionProbe.RenderProbe(null);

        GameMode = false;
        playBGM(PrefabsManager.Instance.TitleBGM);
        Controls.Menu.Enable();
    }

    void Update() {
        if (GameMode) {
            //are we in a shooter? and maybe turn
            bool ballInShooter = false;
            foreach (Widget controllable in FindObjectsOfType<Widget>()) {
                bool ghosted = false;
                foreach (Ghostable ghostable in controllable.GetComponentsInChildren<Ghostable>())
                    if (ghostable.Ghosted)  
                        ghosted = true;

                if (!ghosted) controllable.Turn(turnValue * Time.deltaTime * TurnSpeed);
                ballInShooter |= controllable.HasBall();
                if (controllable.HasBall()) lastShooter = controllable;
            }

            //if we're specialing...
            if (specialHeld && !ballInShooter) {
                if (Time.time-specialTime < SpecialTriggerTime) {
                    currentBall.GetComponent<Ball>().Warbler.volume = (Time.time - specialTime)/SpecialTriggerTime;
                }
                else if (!specialTriggered){
                    specialTriggered = true;
                    StartCoroutine(rewindToLastShooter(lastShooter));
                }
            }
            else {
                specialTriggered = false;
                if (currentBall != null) currentBall.GetComponent<Ball>().Warbler.volume = 0f;
            }

            //if the ball is in a shooter, use highball cam by default, otherwise ballcam
            HighBallCam.Priority = levelStarted && ballInShooter ? 15 : 0;
        }
    }

    //gameplay controls//
    public void OnTurn(InputAction.CallbackContext context) {
        if (context.performed) turnValue = context.ReadValue<float>();
        else if (context.canceled) turnValue = 0;
    }

    public void OnAction(InputAction.CallbackContext context) {
        if (context.performed) actionHeld = true;
        else if (context.canceled) actionHeld = false;

        if (context.performed || context.canceled)
            foreach (Widget controllable in FindObjectsOfType<Widget>()) {
                bool ghosted = false;
                foreach (Ghostable ghostable in controllable.GetComponentsInChildren<Ghostable>())
                    if (ghostable.Ghosted)  
                        ghosted = true;
                controllable.Action(!ghosted && actionHeld);
            }
    }

    public void OnSpecial(InputAction.CallbackContext context) {
        if (context.performed){
            specialHeld = true;
            specialTime = Time.time;

            currentBall.GetComponent<Ball>().Warbler.Play();
        }
        else if (context.canceled){
            specialHeld = false;

            currentBall.GetComponent<Ball>().Warbler.Stop();
        }
    }

    public void OnLook(InputAction.CallbackContext context) {
        if (context.performed && GameMode) LookUpCam.Priority = 20;
        else if (context.canceled) LookUpCam.Priority = 0;
    }

    //menu controls//
    public void OnMove(InputAction.CallbackContext context) {
        //we don't actually have anything to do here
    }

    public void OnConfirm(InputAction.CallbackContext context) { 
        if (context.performed) StartCoroutine(startGame());
    }

    public void OnQuit(InputAction.CallbackContext context) { 
        if (context.performed && GameMode) StartCoroutine(quit());
    }

    //audio//
    private void playBGM(AudioClip audioClip) {
        BGMAudioSource.clip = audioClip;
        BGMAudioSource.Play();
    }

    //game control//
    private IEnumerator toTitle() {
        GameUI.Instance.Wiper.WipeIn();
        GameUI.Instance.LevelNumber.enabled = false;
        yield return new WaitForSeconds(1f);

        RenderSettings.skybox = TitleSkybox;
        DynamicGI.UpdateEnvironment();
        ReflectionProbe.RenderProbe(null);
        GameMode = false;

        GameUI.Instance.Wiper.WipeOut();
        yield return new WaitForSeconds(1f);
        GameUI.Instance.LevelNumber.enabled = true;

        playBGM(PrefabsManager.Instance.TitleBGM);
        Controls.Menu.Enable();
    }

    private IEnumerator startGame() {
        Controls.Menu.Disable();
        GameUI.Instance.Boomer.Play();
        yield return startLevel(0);
    }

    private IEnumerator startLevel(int level) {
        levelStarted = false;

        CurrentLevel = level;

        GameUI.Instance.Wiper.WipeIn();
        yield return new WaitForSeconds(1f);

        board = Instantiate(LevelPrefabs[CurrentLevel]).GetComponent<Stage>();

        RenderSettings.skybox = board.Skybox;
        DynamicGI.UpdateEnvironment();
        ReflectionProbe.RenderProbe(null);
        GameUI.Instance.Wiper.WipeOut();

        GameMode = true;
        ShotsTaken = 0;

        //rotate shooter downward to receive ball
        float startingMaxRotationDegrees = board.StartingShooter.MaxRotationDegrees;
        board.StartingShooter.MaxRotationDegrees = 180f;
        board.StartingShooter.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

        //have cam start at initial shooter
        BallCam.LookAt = board.StartingShooter.transform;
        BallCam.Follow = board.StartingShooter.transform;

        //spawn ball beneath shooter, have it fire upward into the shooter and stay there
        currentBall = Instantiate(BallPrefab, board.StartingShooter.transform.position + Vector3.down * 30, Quaternion.identity);
        currentBall.transform.SetParent(board.transform);
        currentBall.GetComponent<Rigidbody>().useGravity = false;
        currentBall.GetComponent<Collider>().enabled = false;

        float t = Time.time;
        while (Time.time - t < 0.5f) {
            currentBall.transform.position = Vector3.Lerp(board.StartingShooter.transform.position + Vector3.down * 30, board.StartingShooter.transform.position, (Time.time-t)/0.5f);
            yield return new WaitForEndOfFrame();
        }
        currentBall.transform.position = board.StartingShooter.transform.position;
        yield return new WaitForEndOfFrame();
        currentBall.transform.position = board.StartingShooter.transform.position;
        yield return new WaitForEndOfFrame();
        currentBall.GetComponent<Collider>().enabled = true;

        //rotate the shooter back to the top with the ball in it
        t = Time.time;
        while (Time.time - t < 1f) {
            board.StartingShooter.transform.rotation = Quaternion.Euler(Vector3.Slerp(new Vector3(0f, 0f, 180f), Vector3.zero, (Time.time-t)/1f));
            yield return new WaitForEndOfFrame();
        }
        board.StartingShooter.transform.rotation = Quaternion.identity;
        board.StartingShooter.MaxRotationDegrees = startingMaxRotationDegrees;
        currentBall.GetComponent<Rigidbody>().useGravity = true;

        //set ball cam to look at ball, startup mousic and show the goal
        BallCam.LookAt = currentBall.transform;
        BallCam.Follow = currentBall.transform;
        LookUpCam.Follow = currentBall.transform;
        HighBallCam.Follow = currentBall.transform;

        playBGM(board.BGMClip);

        yield return new WaitForSeconds(1);

        FindObjectOfType<Goal>().GoalCam.Priority = 30;

        yield return new WaitForSeconds(2);

        FindObjectOfType<Goal>().GoalCam.Priority = 0;
        HighBallCam.Priority = 15;

        yield return new WaitForSeconds(1);

        Controls.Gameplay.Enable();
        levelStarted = true;
    }

    public void Goal() { StartCoroutine(goal()); }
    private IEnumerator goal() {
        Controls.Gameplay.Disable();

        GameUI.Instance.Goal.SetActive(true);

        BallCam.LookAt = board.Goal.transform;
        BallCam.Follow = board.Goal.transform;
        BallCam.Priority = 50;

        yield return new WaitForSeconds(1);

        BallCam.LookAt = null;
        BallCam.Follow = null;

        Instantiate(PrefabsManager.Instance.GoalBoom, currentBall.transform.position + new Vector3(0, 0, -1), Quaternion.identity);
        BGMAudioSource.Stop();
        board.Goal.BoomerAudioSource.Play();

        currentBall.GetComponent<Rigidbody>().useGravity = false;

        float t = Time.time;
        while (Time.time - t < 0.5f) {
            currentBall.transform.position = Vector3.Lerp(board.Goal.transform.position, board.Goal.transform.position + Vector3.up * 30f, (Time.time-t)/0.5f);
            yield return new WaitForEndOfFrame();
        }
        currentBall.transform.position = board.Goal.transform.position + Vector3.up * 30f;

        yield return new WaitForSeconds(1);

        GameUI.Instance.Goal.SetActive(false);

        CurrentLevel++;

        Destroy(board.gameObject);

        BallCam.Priority = 10;

        if (CurrentLevel >= LevelPrefabs.Length) yield return toTitle();
        else yield return startLevel(CurrentLevel);
    }

    private IEnumerator quit() {
        Controls.Gameplay.Disable();

        Destroy(board.gameObject);

        BallCam.Priority = 10;

        yield return toTitle();
    }

    private IEnumerator rewindToLastShooter(Widget lastShooter) {
        //disable controls, stop the ball in place and hold it there
        Controls.Gameplay.Disable();
        currentBall.GetComponent<Rigidbody>().useGravity = false;
        currentBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        currentBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        yield return new WaitForEndOfFrame();
        currentBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        currentBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        yield return new WaitForEndOfFrame();

        //ball pops out
        currentBall.GetComponent<MeshRenderer>().enabled = false;
        Instantiate(PrefabsManager.Instance.BallPoof, currentBall.transform.position, Quaternion.identity);
        currentBall.GetComponent<Ball>().Teleport.Play();
        yield return new WaitForSeconds(0.5f);

        //scroll to lastShooter
        currentBall.transform.position = lastShooter.transform.position;
        currentBall.GetComponent<Ball>().ResetTrail();
        yield return new WaitForSeconds(1f);

        //ball pops in
        Instantiate(PrefabsManager.Instance.BallPoof, currentBall.transform.position, Quaternion.identity);
        currentBall.GetComponent<Ball>().Teleport.Play();
        yield return new WaitForSeconds(0.05f);
        currentBall.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(0.95f);
    
        //return control
        currentBall.GetComponent<Rigidbody>().useGravity = true;
        Controls.Gameplay.Enable();
    }

}
