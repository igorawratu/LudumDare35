using UnityEngine;
using System.Collections;

public class QuadUp : MonoBehaviour {
    public GameObject sound;

    private int coord;
    private GenerateThings generator;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject player_rect = collider.gameObject;
        Debug.Assert(player_rect.layer == LayerMask.NameToLayer("Player"));
        GameObject player = player_rect.transform.parent.gameObject;
        player.GetComponent<Player>().grow();
        generator.freeSpace(coord);
        Instantiate(sound);
        Destroy(gameObject);
    }

    public void setCoord(int c, GenerateThings g)
    {
        coord = c;
        generator = g;
    }
}
