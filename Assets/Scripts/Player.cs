using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public GameObject line_prefab;
    public GameObject bullet_prefab;
    public GameObject death_effect_prefab;

    public Color color;
    public int max_sides;
    public float move_speed;
    public float rot_speed;
    public float shrink_rate;
    public float shot_speed;
    public float slow_rate;
    public float rot_slow_rate;

    public float powerup_speed_inc;
    public float powerup_rotspeed_inc;

    public bool lock_input = false;

    private int num_sides;
    private List<GameObject> side_sprites;

    private const float side_length = 1f;

    public int player_number;
    private Controls player_controls;
    private GenerateThings manager;
    private int damage = 1;
    private GameObject player_center;

	// Use this for initialization
	void Start () {
        side_sprites = new List<GameObject>();
        num_sides = 3;
        generateGeometry();
        player_controls = PlayerControls.getPlayerControls()[player_number];
        player_center = gameObject.transform.FindChild("player_center").gameObject;
        player_center.GetComponent<SpriteRenderer>().color = color;
    }
	
	// Update is called once per frame
	void Update () {
        if (!lock_input)
        {
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

            //for testing purposes only----
            if (Input.GetKeyDown(KeyCode.G))
            {
                grow();
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                shrink();
            }
            //until here

            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            Vector2 vel = new Vector2((float)hor_factor * move_speed, (float)vert_factor * move_speed);
            rb.angularVelocity = 0f;

            if (vel != Vector2.zero)
            {
                rotatePlayer(vel);
            }


            rb.velocity = vel;

            processShootingLogic();
        }
        else
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * 360f / 2f);
        }
	}

    private void generateGeometry()
    {
        Debug.Assert(num_sides > 2);
        int curr_geometry = side_sprites.Count;
        int diff = num_sides - curr_geometry;

        if(diff > 0)
        {
            Vector3 scale = side_sprites.Count > 0 ? side_sprites[0].transform.localScale : line_prefab.transform.localScale;
            for(int i = 0; i < diff; ++i)
            {
                GameObject new_side = Instantiate(line_prefab);
                new_side.transform.parent = gameObject.transform;
                new_side.transform.localScale = scale;
                new_side.GetComponent<SpriteRenderer>().color = color;
                side_sprites.Add(new_side);
            }
        }
        else if(diff < 0)
        {
            int to_del = Mathf.Abs(diff);
            for(int i = 0; i < to_del; ++i)
            {
                Destroy(side_sprites[side_sprites.Count - 1]);
                side_sprites.RemoveAt(side_sprites.Count - 1);
            }
        }

        float apex_angle = 2 * Mathf.PI / (float)num_sides;
        float opposite = side_length / 2;
        float radius = opposite / Mathf.Tan(apex_angle / 2);

        int idx = 0;
        foreach(GameObject side in side_sprites)
        {
            side.transform.localPosition = new Vector3(Mathf.Sin(apex_angle * idx) * radius, Mathf.Cos(apex_angle * idx) * radius, 0);
            float rot_angle = apex_angle / Mathf.PI * 180f * idx;
            side.transform.rotation = gameObject.transform.rotation;
            side.transform.Rotate(new Vector3(0, 0, 1), -rot_angle);

            idx++;
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

    public void grow()
    {
        if(num_sides < max_sides)
        {
            num_sides++;
            player_center.transform.parent = null;
            gameObject.transform.localScale *= 1 / shrink_rate;
            player_center.transform.parent = gameObject.transform;
            move_speed *= 1 / slow_rate;
            rot_speed *= 1 / rot_slow_rate;
            generateGeometry();
        }       
    }

    public void shrink()
    {
        num_sides--;

        if (num_sides < 3)
        {
            manager.playerDied(player_number);
            GameObject effect = Instantiate(death_effect_prefab);
            effect.transform.position = gameObject.transform.position;
            effect.GetComponent<ParticleSystem>().startColor = color;
            Destroy(gameObject);
        }
        else
        {
            player_center.transform.parent = null;
            gameObject.transform.localScale *= shrink_rate;
            player_center.transform.parent = gameObject.transform;
            move_speed *= slow_rate;
            rot_speed *= rot_slow_rate;
            generateGeometry();
        }
    }

    private void processShootingLogic()
    {
        if (!Input.GetKeyDown(player_controls.shoot) || num_sides < 4)
        {
            return;
        }

        GameObject bullet = Instantiate(bullet_prefab);
        Vector3 dir = gameObject.transform.up;
        dir.Normalize();

        float rotation = Vector3.Angle(dir, bullet.transform.right);

        Vector3 cross = Vector3.Cross(dir, bullet.transform.right);
        if (cross.z > 0)
        {
            rotation *= -1;
        }

        bullet.transform.Rotate(new Vector3(0, 0, 1), rotation);        

        dir *= shot_speed;
        Bullet bullet_script = bullet.GetComponent<Bullet>();
        bullet_script.setVelocity(dir);
        bullet.transform.position = gameObject.transform.position;
        bullet_script.setDamage(damage);
        Collider2D bullet_collider = bullet.GetComponent<Collider2D>();
        bullet.GetComponent<SpriteRenderer>().color = color;

        foreach(GameObject side in side_sprites)
        {
            Collider2D side_collider = side.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(side_collider, bullet_collider);
        }

        shrink();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject collided_object = collider.gameObject;
        if(collided_object.layer == LayerMask.NameToLayer("Bullet"))
        {
            int damage = collided_object.GetComponent<Bullet>().getDamage();

            for (int i = 0; i < damage; ++i)
            {
                shrink();
            }
        }
    }

    public void setManager(GenerateThings m)
    {
        manager = m;
    }

    public void increaseSpeed()
    {
        move_speed += powerup_speed_inc;
    }

    public void increaseRotSpeed()
    {
        rot_speed += powerup_rotspeed_inc;
    }

    public void increaseDamage()
    {
        damage++;
    }
}
