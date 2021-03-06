﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    // Use this for initialization
    public GameObject effect;
    public GameObject owner;

    private Vector2 velocity = Vector2.zero;
    private int damage;
	void Start () {
        gameObject.GetComponent<AudioSource>().Play();
    }
	
	// Update is called once per frame
	void Update () {
        Vector2 diff = velocity * Time.deltaTime;
        gameObject.transform.position += new Vector3(diff.x, diff.y, 0);
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        //play some animation here
        GameObject e = Instantiate(effect);
        e.transform.position = gameObject.transform.position;
        e.GetComponent<ParticleSystem>().startColor = gameObject.GetComponent<SpriteRenderer>().color;
        Destroy(gameObject);
    }

    public void setVelocity(Vector2 v)
    {
        velocity = v;
    }

    public void setDamage(int dmg)
    {
        damage = dmg;
    }

    public int getDamage()
    {
        return damage;
    }

    public void setOwner(GameObject p)
    {
        owner = p;
    }

    public GameObject getOwner()
    {
        return owner;
    }
}
