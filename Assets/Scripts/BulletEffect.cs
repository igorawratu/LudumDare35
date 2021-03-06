﻿using UnityEngine;
using System.Collections;

public class BulletEffect : MonoBehaviour {
    private ParticleSystem ps;
	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
        gameObject.GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void Update () {
        if (!ps.IsAlive() && !gameObject.GetComponent<AudioSource>().isPlaying)
        {
            Destroy(gameObject);
        }
	}
}
