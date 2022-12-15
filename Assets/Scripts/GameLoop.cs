using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public enum GameState { START, TURNDECISION, PLAYERTURN, AITURN, WON, LOST }


public class GameLoop : MonoBehaviour
{
    public GameObject Notification;
    public RaycastHit mousePos;
    public GameObject highlighter;
    public GameState state;
    public GameObject firstTurnPrompt;
    public bool[,] Board = new bool[20, 11];
    public struct ships
    {
        GameObject shipObj;
        int shipLength;
        string shipName;
        Vector3 coord;
        bool placed;
        int orientation;
        bool validPlacement;
        public bool isValid() { return validPlacement; }
        public void updateValid(bool validVal) { validPlacement = validVal; }
        public bool isPlaced() { return placed; }
        public int getLength() { return shipLength; }
        public string getName() { return shipName; }
        public Vector3 getCoord() { return coord; }
        public int getOrientation() { return orientation; }
        public void updatePlaced(bool placed) { this.placed = placed; }
        public void setCoord(Vector3 coord) { this.coord = coord; }
        public GameObject getShipObj() { return shipObj; }
        public void init(GameObject obj, int length, string name) { shipObj = obj; shipLength = length; shipName = name; placed = false; orientation = 0; validPlacement = true; }
    }


    // Start is called before the first frame update
    void Start()
    {
        highlighter = GameObject.Instantiate(highlighter);
        highlighter.SetActive(false);
        state = GameState.START;
        Invoke("WhoGoesFirst", 2);

    }

    // Update is called once per frame
    void Update()
    {
        bool onScreen = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousePos);

        if (!onScreen)
        {
            highlighter.SetActive(false);
            return;
        }
        if (!mousePos.collider.CompareTag("aiBase"))
        {
            highlighter.SetActive(false);
        }
        

        if (mousePos.collider.CompareTag("aiBase")) 
        {
            highlighter.SetActive(true);
            highlighter.transform.position = mousePos.transform.position; 
        }
    }



    void WhoGoesFirst()
    {
        firstTurnPrompt.SetActive(true);
    }

    public void onPlayerButton()
    {
        firstTurnPrompt.SetActive(false);
        state = GameState.PLAYERTURN;

        StartCoroutine(DisplayNotification("You Shall go first"));
    }

    public void onAIButton()
    {
        firstTurnPrompt.SetActive(false);
        state = GameState.AITURN;

        StartCoroutine(DisplayNotification("AI Shall go first"));

    }

    IEnumerator DisplayNotification(string message)
    {
        Notification.SetActive(true);
        Notification.GetComponentInChildren<TMPro.TextMeshProUGUI>(Notification).text = message;
        yield return new WaitForSeconds(3);
        Notification.SetActive(false);
        if (state == GameState.PLAYERTURN)
        {
            StartCoroutine(PlayerTurn());
        }
        else if (state == GameState.AITURN)
        {
            StartCoroutine(AITurn());
        }
    }


    IEnumerator AITurn()
    {
        int x = (int)Random.Range(1.0f, 10.0f);
        int y = (int)Random.Range(1.0f, 10.0f);
        // Animation for hit or miss goes here
        Debug.Log("Do animation for hit or miss");

        yield return new WaitForSeconds(3);

        if (HitCheck(x, y))
        {
            StartCoroutine(DisplayNotification($"Ai has hit!"));
        }
        else
        {
            state = GameState.PLAYERTURN;
            StartCoroutine(DisplayNotification($"Ai has missed!\nIt is now Your turn!"));            
            // Do player turn
        }

    }
    IEnumerator PlayerTurn()
    {
        yield return StartCoroutine(WaitForMouseDown());
        int x = (int)highlighter.transform.position.x;
        int y = (int)highlighter.transform.position.z;

        //Do Animation for hit or miss here
        Debug.Log("do hit or miss animation");
        yield return new WaitForSeconds(3);

        if (HitCheck(x, y))
        {
            StartCoroutine(DisplayNotification($"You hit!"));
        }
        else
        {
            state = GameState.AITURN;
            StartCoroutine(DisplayNotification($"You have missed!\nIt is now the Ai's turn!"));

            // Do player turn
        }
    }



    bool HitCheck(int x, int y) { return !Board[x, y] ? true : false; }
    public void setBoard(bool[,] board) { Board = board; }
    IEnumerator WaitForMouseDown() { while (!Input.GetMouseButtonUp(0)) { yield return null; } }

}
