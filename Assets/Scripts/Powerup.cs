using UnityEngine;
using System.Collections;

public enum PowerupType {SPEED, ROTATION, DAMAGE}

public class Powerup : MonoBehaviour {
    public Sprite speedup_sprite;
    public Sprite rotup_sprite;
    public Sprite dmgup_sprite;

    public Color speedup_color;
    public Color rotup_color;
    public Color dmgup_color;

    public bool lock_type;
    public PowerupType preset_type;

    public GameObject sound;

    private GenerateThings generator;
    private int coord;
    private PowerupType type;

	// Use this for initialization
	void Start () {
        if (lock_type)
        {
            type = preset_type;
        }
        else
        {
            int rand = Mathf.Min(3, (int)(Random.value * 3f));
            switch (rand)
            {
                case 0:
                    type = PowerupType.SPEED;
                    break;
                case 1:
                    type = PowerupType.ROTATION;
                    break;
                case 2:
                    type = PowerupType.DAMAGE;
                    break;
                default:
                    type = PowerupType.SPEED;
                    break;
            }
        }

        switch (type)
        {
            case PowerupType.DAMAGE:
                gameObject.GetComponent<SpriteRenderer>().sprite = dmgup_sprite;
                gameObject.GetComponent<SpriteRenderer>().color = dmgup_color;
                break;
            case PowerupType.SPEED:
                gameObject.GetComponent<SpriteRenderer>().sprite = speedup_sprite;
                gameObject.GetComponent<SpriteRenderer>().color = speedup_color;
                break;
            case PowerupType.ROTATION:
                gameObject.GetComponent<SpriteRenderer>().sprite = rotup_sprite;
                gameObject.GetComponent<SpriteRenderer>().color = rotup_color;
                break;
            default:
                break;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject player_rect = collider.gameObject;
        Debug.Assert(player_rect.layer == LayerMask.NameToLayer("Player"));
        GameObject player = player_rect.transform.parent.gameObject;
        Player player_script = player.GetComponent<Player>();

        switch (type)
        {
            case PowerupType.DAMAGE: player_script.increaseDamage();
                break;
            case PowerupType.SPEED: player_script.increaseSpeed();
                break;
            case PowerupType.ROTATION: player_script.increaseRotSpeed();
                break;
            default:
                break;
        }

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
