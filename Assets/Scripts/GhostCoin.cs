using UnityEngine;

public class GhostCoin : MonoBehaviour {

    public Detector Detector;
    public Renderer[] Renderers;

    public bool paid = false;

    public void Reset() {
        foreach (Renderer renderer in Renderers) renderer.enabled = true;
        paid = false;
    }

    void Update() {
        if (Detector.ball != null && !paid && GameController.Instance.CoinsHeld > 0) {
            GameController.Instance.CoinsHeld--;
            foreach (Renderer renderer in Renderers) renderer.enabled = false;
            paid = true;
            Instantiate(PrefabsManager.Instance.CoinSparkle, transform.position, Quaternion.identity);

            GetComponent<AudioSource>().Play();
        }        
    }
}
