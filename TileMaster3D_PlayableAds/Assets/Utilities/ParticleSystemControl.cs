using System.Collections;
using UnityEngine;

public class ParticleSystemControl : MonoBehaviour {
    public bool bDestroySelf = false;
    public float fMaxLifetime;
    private void Start() {
        if (this.bDestroySelf) StartCoroutine(this.DestroySelf());
    }

    public void Play() {
        var particle = this.GetComponent<ParticleSystem>();
        if (particle != null) {
            particle.Play();
            return;
        }
        var systems = GetComponentsInChildren<ParticleSystem>();
        foreach (var system in systems) {
            system.Play();
        }
    }


    private IEnumerator DestroySelf() {
        MDebug.Log($"=====fMaxLifetime:{fMaxLifetime}");
        yield return new WaitForSeconds(fMaxLifetime);
        GameObject.Destroy(this.gameObject);
    }
}