using UnityEngine;

public class Coin : Widget {

    public float SpinSpeed = 1f;

    public int Value = 1;

    public MeshRenderer[] Renderers;

    private bool collected = false;

    public override void Turn(float turnValue) {}
    public override void Action(bool actioned) {}

    protected void OnTriggerEnter(Collider other) {
        if (!collected) {
            collected = true;
            Instantiate(PrefabsManager.Instance.CoinSparkle, transform.position, Quaternion.identity);
            GameController.Instance.CoinsHeld += Value;
            foreach (MeshRenderer renderer in Renderers) 
                renderer.enabled = false;

            GetComponent<AudioSource>().Play();
        }
    }

    protected void Update() {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y += SpinSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(euler);
    }
}
