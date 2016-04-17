using UnityEngine;
using System.Collections;

public class BulletEffect : MonoBehaviour {
    private ParticleSystem ps;
	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(!ps.IsAlive());
        if (!ps.IsAlive())
        {
            Destroy(gameObject);
        }
	}
}
