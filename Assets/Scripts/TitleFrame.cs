using UnityEngine;
using System.Collections;

public class TitleFrame : MonoBehaviour {

    public GameObject top_prefab;
    public GameObject bottom_prefab;
    public GameObject left_prefab;
    public GameObject right_prefab;

	// Use this for initialization
	void Start () {
        float hor_extent = Camera.main.orthographicSize * Screen.width / Screen.height;
        float vert_extent = Camera.main.orthographicSize;

        GameObject top = Instantiate(top_prefab);
        GameObject bottom = Instantiate(bottom_prefab);
        GameObject left = Instantiate(left_prefab);
        GameObject right = Instantiate(right_prefab);

        top.transform.position = new Vector3(0, vert_extent);
        bottom.transform.position = new Vector3(0, -vert_extent);
        left.transform.position = new Vector3(-hor_extent, 0);
        right.transform.position = new Vector3(hor_extent, 0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
