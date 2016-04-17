using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GenerateThings : MonoBehaviour {
    public GameObject obstacle_prefab;
    public GameObject quadup_prefab;
    public GameObject powerup_prefab;
    public GameObject top_prefab;
    public GameObject bottom_prefab;
    public GameObject left_prefab;
    public GameObject right_prefab;
    public float quadup_time_threshold;
    public float powerup_time_threshold;
    public float jitter_ratio;

    public GameObject player_1_prefab;
    public GameObject player_2_prefab;

    public float obstacle_gen_prob;
    public int max_obstacle_length;

    public GameObject p1wins;
    public GameObject p2wins;

    private const int width = 16;
    private const int height = 10;
    private bool[] environment_grid;
    private List<int> free_spaces;

    private float hor_extent;
    private float vert_extent;

    private float quadup_timer;
    private bool freeze = false;

    private float powerup_timer;

	// Use this for initialization
	void Start () {
        free_spaces = new List<int>();

        environment_grid = new bool[width * height];
        for(int i = 0; i < width * height; ++i)
        {
            environment_grid[i] = false;
        }

        hor_extent = Camera.main.orthographicSize * Screen.width / Screen.height;
        vert_extent = Camera.main.orthographicSize;

        generateEnvironment();
        findFreeSpaces();

        generatePlayers();

        quadup_timer = 0f;
        powerup_timer = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (!freeze)
        {
            generateQuadUps();
            generatePowerups();
            drawDebugGridlines();
        }
    }

    private void generateEnvironment()
    {
        GameObject top = Instantiate(top_prefab);
        GameObject bottom = Instantiate(bottom_prefab);
        GameObject left = Instantiate(left_prefab);
        GameObject right = Instantiate(right_prefab);

        top.transform.position = new Vector3(0, vert_extent);
        bottom.transform.position = new Vector3(0, -vert_extent);
        left.transform.position = new Vector3(-hor_extent, 0);
        right.transform.position = new Vector3(hor_extent, 0);

        for (int y = 1; y < height - 1; ++y)
        {
            for(int x = 1; x < width - 1; ++x)
            {
                int coord = y * width + x;
                if (emptySurroundings(coord) && Random.value < obstacle_gen_prob)
                {
                    bool horizontal = Random.value < 0.5f;

                    int max_length = findMaxLength(horizontal, coord);
                    int chosen_length = Mathf.Min(max_obstacle_length, (int)(Random.value * (float)max_length + 1f));
                    setEnvironmentOccupied(x, y, chosen_length, horizontal);

                    int end_x = horizontal ? x + chosen_length - 1 : x;
                    int end_y = !horizontal ? y + chosen_length - 1: y;

                    Vector2 midpoint = findMidpoint(x, y, end_x, end_y);

                    GameObject new_obstacle = Instantiate(obstacle_prefab);
                    new_obstacle.transform.localScale = new Vector3(new_obstacle.transform.localScale.x * chosen_length, new_obstacle.transform.localScale.y, new_obstacle.transform.localScale.z);
                    new_obstacle.transform.position = new Vector3(midpoint.x, midpoint.y);

                    if (!horizontal)
                    {
                        new_obstacle.transform.Rotate(new Vector3(0, 0, 1), 90f);
                    }
                }
            }
        }
    }

    private int findMaxLength(bool horizontal, int coord)
    {
        int x = coord % width;
        int y = coord / width;

        int length = 1;
        if (horizontal)
        {
            while(++x < width - 1)
            {
                int curr_coord = y * width + x;
                if (!emptySurroundings(curr_coord))
                {
                    break;
                }
                length++;
            }
        }
        else
        {
            while(++y < height - 1)
            {
                int curr_coord = y * width + x;
                if (!emptySurroundings(curr_coord))
                {
                    break;
                }
                length++;
            }
        }

        return length;
    }

    private void setEnvironmentOccupied(int x, int y, int length, bool horizontal)
    {
        for(int i = 0; i < length; ++i)
        {
            int actual_x = horizontal ? x + i : x;
            int actual_y = !horizontal ? y + i : y;
            int coord = actual_x + actual_y * width;
            environment_grid[coord] = true;
        }
    }

    private Vector2 findMidpoint(int x, int y, int endx, int endy)
    {
        float cell_dim_x = 1f / (float)width * hor_extent * 2;
        float cell_dim_y = 1f / (float)height * vert_extent * 2;

        Vector2 start = new Vector2((float)x * cell_dim_x - hor_extent, (float)y * -cell_dim_y + vert_extent);
        Vector2 end = new Vector2((float)(endx + 1) * cell_dim_x - hor_extent, (float)(endy + 1) * -cell_dim_y + vert_extent);

        Vector2 midpoint = (start + end) / 2;

        return midpoint;
    }

    private void drawDebugGridlines()
    {
        float cell_dim_half = 1f / (float)width * hor_extent;
        for (int y = 0; y < height; ++y)
        {
            Vector2 p1 = findMidpoint(0, y, 0, y);
            Vector2 p2 = findMidpoint(width - 1, y, width - 1, y);
            p2.x += cell_dim_half * 2;

            Debug.DrawLine(new Vector3(p1.x - cell_dim_half, p1.y + cell_dim_half), new Vector3(p2.x - cell_dim_half, p2.y + cell_dim_half));
        }

        for (int x = 0; x < width; ++x)
        {
            Vector2 p1 = findMidpoint(x, 0, x, 0);
            Vector2 p2 = findMidpoint(x, height - 1, x, height - 1);
            p2.y -= cell_dim_half * 2;

            Debug.DrawLine(new Vector3(p1.x - cell_dim_half, p1.y + cell_dim_half), new Vector3(p2.x - cell_dim_half, p2.y + cell_dim_half));
        }
    }

    private bool emptySurroundings(int coord)
    {
        int x = coord % width;
        int y = coord / width;

        bool empty = true;

        int start_x = Mathf.Max(0, x - 1);
        int end_x = Mathf.Min(width - 1, x + 1);
        int start_y = Mathf.Max(0, y - 1);
        int end_y = Mathf.Min(height - 1, y + 1);
        
        for(int cy = start_y; cy <= end_y; ++cy)
        {
            for(int cx = start_x; cx <= end_x; ++cx)
            {
                int curr_coord = cy * width + cx;
                empty = empty && !environment_grid[curr_coord];
            }
        }

        return empty;
    }

    private void findFreeSpaces()
    {
        for (int i = 0; i < environment_grid.Length; ++i)
        {
            if (!environment_grid[i])
            {
                free_spaces.Add(i);
            }
        }
    }

    public void freeSpace(int coord)
    {
        free_spaces.Add(coord);
        environment_grid[coord] = false;
    }

    private void generateQuadUps()
    {
        quadup_timer += Time.deltaTime;

        if(quadup_timer >= quadup_time_threshold)
        {
            quadup_timer = 0f;
            if(free_spaces.Count > 0)
            {
                int idx = Mathf.Min((int)(Random.value * free_spaces.Count), free_spaces.Count - 1);
                int coord = free_spaces[idx];
                free_spaces.RemoveAt(idx);
                GameObject quadupp = Instantiate(quadup_prefab);
                GameObject quadup = quadupp.transform.FindChild("QuadUp").gameObject;
                QuadUp qu = quadup.GetComponent<QuadUp>();
                qu.setCoord(coord, this);
                int x = coord % width;
                int y = coord / width;
                Vector2 pos = findMidpoint(x, y, x, y);

                float cell_dim_x = 1f / (float)width * hor_extent * 2;
                float cell_dim_y = 1f / (float)height * vert_extent * 2;

                float x_jitter = (Random.value - 0.5f) * jitter_ratio * cell_dim_x;
                float y_jitter = (Random.value - 0.5f) * jitter_ratio * cell_dim_y;

                pos += new Vector2(x_jitter, y_jitter);

                quadupp.transform.position = new Vector3(pos.x, pos.y);
            }
        }

    }

    private void generatePowerups()
    {
        powerup_timer += Time.deltaTime;

        if (powerup_timer >= powerup_time_threshold)
        {
            powerup_timer = 0f;
            if (free_spaces.Count > 0)
            {
                int idx = Mathf.Min((int)(Random.value * free_spaces.Count), free_spaces.Count - 1);
                int coord = free_spaces[idx];
                free_spaces.RemoveAt(idx);
                GameObject powerupp = Instantiate(powerup_prefab);
                GameObject powerup = powerupp.transform.FindChild("Powerup").gameObject;
                Powerup pu = powerup.GetComponent<Powerup>();
                pu.setCoord(coord, this);
                int x = coord % width;
                int y = coord / width;
                Vector2 pos = findMidpoint(x, y, x, y);

                float cell_dim_x = 1f / (float)width * hor_extent * 2;
                float cell_dim_y = 1f / (float)height * vert_extent * 2;

                float x_jitter = (Random.value - 0.5f) * jitter_ratio * cell_dim_x;
                float y_jitter = (Random.value - 0.5f) * jitter_ratio * cell_dim_y;

                pos += new Vector2(x_jitter, y_jitter);

                powerupp.transform.position = new Vector3(pos.x, pos.y);
            }
        }

    }

    private void generatePlayers()
    {
        Vector2 p1_pos = findMidpoint(0, 0, 0, 0);
        Vector2 p2_pos = findMidpoint(width - 1, height - 1, width - 1, height - 1);
        
        GameObject p1 = Instantiate(player_1_prefab);
        p1.transform.position = new Vector3(p1_pos.x, p1_pos.y);
        float p1_rot = Vector2.Angle(-p1_pos, p1.transform.up);
        p1.transform.Rotate(new Vector3(0, 0, 1), p1_rot);

        GameObject p2 = Instantiate(player_2_prefab);
        p2.transform.position = new Vector3(p2_pos.x, p2_pos.y);
        float p2_rot = Vector2.Angle(-p2_pos, p2.transform.up);
        p1.transform.Rotate(new Vector3(0, 0, 1), -p2_rot);

        p1.GetComponent<Player>().setManager(this);
        p2.GetComponent<Player>().setManager(this);
    }

    public void playerDied(int player)
    {
        int winner = player == 1 ? 2 : 1;
        GameObject winner_obj = winner == 1 ? player_1_prefab : player_2_prefab;

        GameObject winner_text = winner == 1 ? Instantiate(p1wins) : Instantiate(p2wins);
        winner_text.GetComponent<SpriteRenderer>().color = winner_obj.GetComponent<Player>().color;

        freeze = true;

        StartCoroutine(end());
    }

    IEnumerator end()
    {
        yield return new WaitForSeconds(4f);
        Application.LoadLevel("start");
    }
}
