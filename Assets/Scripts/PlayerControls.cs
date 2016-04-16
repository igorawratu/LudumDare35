using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controls
{
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode shoot;
}

public class PlayerControls : Singleton<PlayerControls>
{
    private static Dictionary<int, Controls> player_controls = new Dictionary<int, Controls>();

    private static bool initialized = false;
	
    protected PlayerControls()
    {
    }

    public static Dictionary<int, Controls> getPlayerControls()
    {
        if (!initialized)
        {
            Controls p1 = new Controls();
            p1.up = KeyCode.W;
            p1.down = KeyCode.S;
            p1.left = KeyCode.A;
            p1.right = KeyCode.D;
            p1.shoot = KeyCode.Space;

            Controls p2 = new Controls();
            p2.up = KeyCode.UpArrow;
            p2.down = KeyCode.DownArrow;
            p2.left = KeyCode.LeftArrow;
            p2.right = KeyCode.RightArrow;
            p2.shoot = KeyCode.Return;

            player_controls[1] = p1;
            player_controls[2] = p2;

            initialized = true;
        }

        return player_controls;
    }
    

}
