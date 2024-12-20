using System.Collections.Generic;
using UnityEngine;

public class Gate : Triggered {

    public GameObject Bar;
    public Detector Detector;
    public Transform AnEnd, OtherEnd;

    public int Cost;
    public bool Open = false;

    private List<GhostCoin> GhostCoins = new List<GhostCoin>();

    public override void Turn(float turnValue) {}
    public override void Action(bool actioned) {}

    public override void Trigger(GameObject triggering) { open(); }
    public override void ResetTrigger(GameObject triggering) { close(); }

    protected void Start() { close(); }

    protected void Update() {
        if (!Open) {
            bool allPaid = true;
            foreach (GhostCoin coin in GhostCoins)
                if (!coin.paid)
                    allPaid = false;
        
            if (allPaid && Cost > 0) open();
        }
    }

    private void open() {
        //just in case
        foreach (GhostCoin coin in GhostCoins)
            Destroy(coin.gameObject);
        GhostCoins.Clear();

        Open = true;
        Bar.SetActive(!Open);

        float distance = Vector3.Distance(AnEnd.position, OtherEnd.position);
        for (int i = 0; i < distance; i++) {
            Vector3 pos = Vector3.Lerp(AnEnd.position, OtherEnd.position, i/distance);
            Instantiate(PrefabsManager.Instance.GatePoof, pos, Quaternion.identity);
        }

        GetComponent<AudioSource>().Play();
    }

    private void close() {
        //create ghost coins
        float distance = Vector3.Distance(AnEnd.position, OtherEnd.position);
        Vector3 increment = (OtherEnd.transform.position - AnEnd.transform.position) / (Cost+1);
        Vector3 pos = AnEnd.transform.position + increment;
        pos += -transform.up * 1;
        for (int i = 0; i < Cost; i++) {
            GhostCoin coin = Instantiate(PrefabsManager.Instance.GhostCoin, pos + new Vector3(0f, 0f, -0.5f), Quaternion.Euler(90f, 0f, 0f)).GetComponent<GhostCoin>();
            coin.transform.SetParent(transform);
            GhostCoins.Add(coin);
            pos += increment;
        }

        Open = false;
        Bar.SetActive(!Open);

        for (int i = 0; i < distance; i++) {
            pos = Vector3.Lerp(AnEnd.position, OtherEnd.position, i/distance);
            Instantiate(PrefabsManager.Instance.GatePoof, pos, Quaternion.identity);
        }

        GetComponent<AudioSource>().Play();
    }
}
