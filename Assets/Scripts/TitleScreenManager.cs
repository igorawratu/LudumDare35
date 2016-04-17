using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleScreenManager : MonoBehaviour {
    private GameObject player1;
    private GameObject player2;
    private GameObject p1title;
    private GameObject p2title;
    private GameObject p1controls;
    private GameObject p2controls;
    private GameObject ready_up;
    private GameObject three;
    private GameObject two;
    private GameObject one;

    private bool starting = false;
    private bool p1_ready = false;
    private bool p2_ready = false;

    Controls player1_controls;
    Controls player2_controls;
    // Use this for initialization
    void Start () {
        player1 = gameObject.transform.FindChild("Player1").gameObject;
        player2 = gameObject.transform.FindChild("Player2").gameObject;
        p1title = gameObject.transform.FindChild("Player1Title").gameObject;
        p2title = gameObject.transform.FindChild("Player2Title").gameObject;
        p1controls = gameObject.transform.FindChild("p1controls").gameObject;
        p2controls = gameObject.transform.FindChild("p2controls").gameObject;
        ready_up = gameObject.transform.FindChild("readyup").gameObject;
        three = gameObject.transform.FindChild("3").gameObject;
        two = gameObject.transform.FindChild("2").gameObject;
        one = gameObject.transform.FindChild("1").gameObject;

        three.SetActive(false);
        two.SetActive(false);
        one.SetActive(false);

        Color p1color = player1.GetComponent<Player>().color;
        Color p2color = player2.GetComponent<Player>().color;

        p1title.GetComponent<SpriteRenderer>().color = p1color;
        p1controls.GetComponent<SpriteRenderer>().color = p1color;
        p2title.GetComponent<SpriteRenderer>().color = p2color;
        p2controls.GetComponent<SpriteRenderer>().color = p2color;

        player1.SetActive(false);
        player2.SetActive(false);

        player1_controls = PlayerControls.getPlayerControls()[1];
        player2_controls = PlayerControls.getPlayerControls()[2];

        StartCoroutine("waitcr");
    }
	
	// Update is called once per frame
	void Update () {
        checkReadyStatus();
    }

    void checkReadyStatus()
    {
        if (Input.GetKeyDown(player1_controls.shoot))
        {
            p1_ready = !p1_ready;
            player1.SetActive(p1_ready);
        }

        if (Input.GetKeyDown(player2_controls.shoot))
        {
            p2_ready = !p2_ready;
            player2.SetActive(p2_ready);
        }

        if (starting != (p1_ready && p2_ready))
        {
            toggleStart();
        }
    }

    private void toggleStart()
    {
        if (starting)
        {
            starting = !starting;
            ready_up.SetActive(true);
            three.SetActive(false);
            two.SetActive(false);
            one.SetActive(false);
            StopCoroutine("startcr");
            StartCoroutine("waitcr");
        }
        else
        {
            starting = !starting;
            ready_up.SetActive(false);
            StopCoroutine("waitcr");
            StartCoroutine("startcr");
        }
    }

    IEnumerator waitcr()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            ready_up.SetActive(!ready_up.activeInHierarchy);
        }
    }

    IEnumerator startcr()
    {
        three.SetActive(true);
        yield return new WaitForSeconds(1f);

        three.SetActive(false);
        two.SetActive(true);
        yield return new WaitForSeconds(1f);

        two.SetActive(false);
        one.SetActive(true);
        yield return new WaitForSeconds(1f);

        Application.LoadLevel("main");
    }
}
