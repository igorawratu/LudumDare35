using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public GameObject line_prefab;
    public Color color;
    public int max_sides;
    public float move_speed;
    public float rot_speed;

    private int num_sides;
    private List<GameObject> side_sprites;

    private const float side_length = 1f;

    public int player_number;
    private Controls player_controls;

	// Use this for initialization
	void Start () {
        side_sprites = new List<GameObject>();
        num_sides = 3;
        generateGeometry();
        //player_number = 1;
        player_controls = PlayerControls.getPlayerControls()[player_number];
    }
	
	// Update is called once per frame
	void Update () {
        int vert_factor = 0;
        int hor_factor = 0;

        if (Input.GetKey(player_controls.up))
        {
            vert_factor++;
        }

        if (Input.GetKey(player_controls.down))
        {
            vert_factor--;
        }

        if (Input.GetKey(player_controls.left))
        {
            hor_factor--;
        }

        if (Input.GetKey(player_controls.right))
        {
            hor_factor++;
        }

        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        Vector2 vel = new Vector2((float)hor_factor * move_speed, (float)vert_factor * move_speed);

        if(vel != Vector2.zero)
        {
            rotatePlayer(vel);
        }
        

        rb.velocity = vel;
	}

    private void generateGeometry()
    {
        float apex_angle = 2 * Mathf.PI / (float)num_sides;
        float opposite = side_length / 2;
        float radius = opposite / Mathf.Tan(apex_angle / 2);

        for(int i = 0; i < num_sides; ++i)
        {
            GameObject new_side = Instantiate(line_prefab);
            new_side.transform.parent = gameObject.transform;
            new_side.transform.localPosition = new Vector3(Mathf.Sin(apex_angle * i) * radius, Mathf.Cos(apex_angle * i) * radius, 0);
            float rot_angle = apex_angle / Mathf.PI * 180f * i;
            new_side.transform.Rotate(new Vector3(0, 0, 1), -rot_angle);

            side_sprites.Add(new_side);
        }
    }
    
    public void setPlayerNumber(int pnum)
    {
        player_number = pnum;
        player_controls = PlayerControls.getPlayerControls()[player_number];
    }

    private void rotatePlayer(Vector2 velocity)
    {
        Vector3 vel_3d = new Vector3(velocity.x, velocity.y, 0);
        float angle = Vector3.Angle(vel_3d, gameObject.transform.up);
        Vector3 cross = Vector3.Cross(vel_3d, gameObject.transform.up);
        
        float rot_amount = Mathf.Min(angle, rot_speed * Time.deltaTime);
        
        if(cross.z > 0)
        {
            rot_amount *= -1;
        }

        gameObject.transform.Rotate(new Vector3(0, 0, 1), rot_amount);
    }
}
