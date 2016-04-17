using UnityEngine;
using System.Collections;

public class SoundObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<AudioSource>().Play();
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            Destroy(gameObject);
        }
	}
}
