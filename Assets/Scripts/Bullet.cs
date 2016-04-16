using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    // Use this for initialization

    private Vector2 velocity = Vector2.zero;
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 diff = velocity * Time.deltaTime;
        gameObject.transform.position += new Vector3(diff.x, diff.y, 0);
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        //play some animation here
        Destroy(gameObject);
    }

    public void setVelocity(Vector2 v)
    {
        velocity = v;
    }
}
