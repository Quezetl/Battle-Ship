using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

//Creation of Gamestates to Handle GamePlay
public enum GameState { START, TURNDECISION, PLAYERTURN, AITURN, GAMEOVER}


public class GameLoop : MonoBehaviour
{
    //Initialize Variables
    public GameObject Notification;
    public GameObject firstTurnPrompt;
    public GameObject Marker;
    public GameObject Smoke;
    public GameObject cmrObj;
    public GameObject PlayerScoreBoard;
    public GameObject AiScoreBoard;
    public GameObject Cursor;
    public RaycastHit mousePos;
    public GameState state;
    public Material Red;
    public Material Blue;
    public AudioClip bombSE;
    public AudioClip waterSE;
    int playerScore;
    int aiScore;
    int[,] playerBoard = new int[10,10];


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


    // Initialize Beggin settings
    void Start()
    {
        aiScore = 0;
        playerScore = 0;
        PlayerScoreBoard.SetActive(true);
        AiScoreBoard.SetActive(true);
        state = GameState.START;
        Invoke("WhoGoesFirst", 2);
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                playerBoard[i, j] = 0;
            }
        }

    }


    // Update only Called for Updating Cursor Position
    void Update()
    {
        bool onScreen = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousePos);

        if (!onScreen)
            return;

    }


    //Ui Prompt for Checking Who Goes First
    void WhoGoesFirst()
    {
        firstTurnPrompt.SetActive(true);
        state = GameState.TURNDECISION;
    }


    //UI Function for Player First Option
    public void onPlayerButton()
    {
        firstTurnPrompt.SetActive(false);
        state = GameState.PLAYERTURN;
        Cursor.SetActive(true);

        StartCoroutine(DisplayNotification("You Shall go first"));
        cmrObj.GetComponent<CameraMovement>().enabled = true;
    }


    //UI Function for AI First Option
    public void onAIButton()
    {
        firstTurnPrompt.SetActive(false);
        state = GameState.AITURN;
        Cursor.SetActive(true);

        StartCoroutine(DisplayNotification("AI Shall go first"));
        cmrObj.GetComponent<CameraMovement>().enabled = true;

    }


    //Coroutine for Displaying Update Messages
    IEnumerator DisplayNotification(string message)
    {
        Notification.SetActive(true);
        Notification.GetComponentInChildren<TMPro.TextMeshProUGUI>(Notification).text = message;
        yield return new WaitForSeconds(3);
        Notification.SetActive(false);

        //Changes Turn Depending on State
        if (state == GameState.PLAYERTURN)
        {
            StartCoroutine(PlayerTurn());
        }
        else if (state == GameState.AITURN)
        {
            StartCoroutine(AITurn());
        }
    }


    //AI Turn Handler
    IEnumerator AITurn()
    {
        //Variable Initialize
        int x;
        int y;

        //Randomize First Guess of AI
        do
        {
            x = (int)Random.Range(0, 9f);
            y = (int)Random.Range(0, 9f);
        } while (playerBoard[x,y] != 0);
        
        //Checks if Shot was a Hit
        bool hit = placeMarker(x, y + 1, HitCheck(x, y));

        //Updates Board if the Shot Hit
        updatePlayerBoard(x, y, hit);

        //Logic for Checking Neighboring Spaces Valid Next Shot
        while(hit)
        {
            //Update ScoreBoard
            aiScore++;
            updateScore(false);

            //Pause For Update
            yield return new WaitForSeconds(1);

            //Display Message With Coordinates
            Notification.SetActive(true);
            Notification.GetComponentInChildren<TMPro.TextMeshProUGUI>(Notification).text = $"AI has hit @: ({x+1}, {y+1})\nIt will go again";

            //Pause For Msg & Hides Msg
            yield return new WaitForSeconds(3);
            Notification.SetActive(false);

            //Logic for Next Shot
            switch (aiDirection(x, y))
            {
                case 0:
                    y++;
                    hit = placeMarker(x, y + 1, HitCheck(x, y));
                    break;
                case 1:
                    x++;
                    hit = placeMarker(x, y + 1, HitCheck(x, y));
                    break;
                case 2:
                    y--;
                    hit = placeMarker(x, y + 1, HitCheck(x, y));
                    break;
                case 3:
                    x--;
                    hit = placeMarker(x, y + 1, HitCheck(x, y));
                    break;
                case 4:
                    do
                    {
                        y = (int)Random.Range(1.0f, 10.0f);
                        x = (int)Random.Range(1.0f, 10.0f);
                    } while (playerBoard[x, y] == 0);
                    hit = placeMarker(x, y + 1, HitCheck(x, y));
                    break;
                default:
                    goto case 4;
            }
        }

        //Pause for animation
        yield return new WaitForSeconds(1);

        //Update state and Assigns a Msg for Display
        state = GameState.PLAYERTURN;
        string msg = $"Ai has shot ({x+1}, {y+1}) and Missed!\nIt is now Your turn!";

        //Checks if Game Over
        if (EndGame())
        {
            Notification.GetComponentInChildren<TMPro.TextMeshProUGUI>(Notification).text = EndGameMessage();
            cmrObj.GetComponent<CameraMovement>().enabled = false;
            this.gameObject.SetActive(false);
        }

        //Pause
        yield return new WaitForSeconds(1.5f);

        //Display Predetermined Msg 
        StartCoroutine(DisplayNotification(msg));
    }


    //Logic for Next Shot
    int aiDirection(int x, int y)
    {
        //Randomize Direction
        int dir = (int)Random.Range(0, 3f);

        //Logic to Check for Proper Neighboring Shot
        if (x < 9 && x > 0 && y < 9 && y > 0)
        {
            switch (dir)
            {
                case 0:
                    if (playerBoard[x, y + 1] == 0)
                        return 0;
                    goto case 1;
                case 1:
                    if (playerBoard[x + 1, y] == 0)
                        return 1;
                    goto case 2;
                case 2:
                    if (playerBoard[x, y - 1] == 0)
                        return 2;
                    goto case 3;
                case 3:
                    if (playerBoard[x - 1, y] == 0)
                        return 3;
                    break;
            }
        }
        dir = (int)Random.Range(0, 2f);
        if (y == 9 && x < 9 && x > 0)
        {
            switch (dir)
            {
                case 0:
                    if (playerBoard[x + 1, y] == 0)
                        return 1;
                    goto case 1;
                case 1:
                    if (playerBoard[x, y - 1] == 0)
                        return 2;
                    goto case 2;
                case 2:
                    if (playerBoard[x - 1, y] == 0)
                        return 3;
                    break;
            }
        }
        if (x == 9 && y < 9 && y > 0)
        {
            switch (dir)
            {
                case 0:
                    if (playerBoard[x, y + 1] == 0)
                        return 0;
                    goto case 1;
                case 1:
                    if (playerBoard[x, y - 1] == 0)
                        return 2;
                    goto case 2;
                case 2:
                    if (playerBoard[x - 1, y] == 0)
                        return 3;
                    break;
            }
        }
        if (y == 0 && x < 9 && x > 0)
        {
            switch (dir)
            {
                case 0:
                    if (playerBoard[x, y + 1] == 0)
                        return 0;
                    goto case 1;
                case 1:
                    if (playerBoard[x + 1, y] == 0)
                        return 1;
                    goto case 2;
                case 2:
                    if (playerBoard[x - 1, y] == 0)
                        return 3;
                    break;
            }
        }
        if (x == 0 && y < 9 && y > 0)
        {
            switch (dir)
            {
                case 0:
                    if (playerBoard[x, y + 1] == 0)
                        return 0;
                    goto case 1;
                case 1:
                    if (playerBoard[x + 1, y] == 0)
                        return 1;
                    goto case 2;
                case 2:
                    if (playerBoard[x, y - 1] == 0)
                        return 2;
                    break;
            }
        }
        return 4;
    }


    //Update 2D Board with either 1 for Hit or 2 For Miss
    void updatePlayerBoard(int x, int y, bool hit)
    {
        if (hit)
        {
            playerBoard[x, y] = 1;
        }
        if (!hit)
        {
            playerBoard[x, y] = 2;
        }
    }


    //Player Turn Handler
    IEnumerator PlayerTurn()
    {
        //Wait for Click to Start Routine
        yield return StartCoroutine(WaitForMouseDown());
        
        //Default Variables
        string msg = $"You hit!";
        int x = (int)mousePos.transform.position.x;
        int y = (int)mousePos.transform.position.z;

        //Check for Hit or Miss
        bool hit = placeMarker(x, y, HitCheck(x, y));

        //Pause for Animation
        yield return new WaitForSeconds(1f);

        //Updates ScoreBoard or State
        if (!hit)
        {
            state = GameState.AITURN;
            msg = $"You have missed!\nIt is now the Ai's turn!";
        }
        else
        {
            playerScore++;
            updateScore(true);
        }


        //Check For Game Over
        if (EndGame())
        {
            Notification.SetActive(true);
            Notification.GetComponentInChildren<TMPro.TextMeshProUGUI>(Notification).text = EndGameMessage();
            cmrObj.GetComponent<CameraMovement>().enabled = false;
            this.gameObject.SetActive(false);
        }


        //Pause
        yield return new WaitForSeconds(2f);

        //Display Message 
        StartCoroutine(DisplayNotification(msg));
    }


    //Function to Update ScoreBoard
    void updateScore(bool player)
    {
        if (player)
        {
            PlayerScoreBoard.GetComponentInChildren<TMPro.TextMeshProUGUI>(Notification).text = $"Player Score: {playerScore}";
        }
        else
        {
            AiScoreBoard.GetComponentInChildren<TMPro.TextMeshProUGUI>(Notification).text = $"AI Score: {aiScore}";
        }
    }


    //Check Who Won and Change State
    bool EndGame()
    {
        if (aiScore > 16)
        {
            state = GameState.GAMEOVER;
            return true;
        }
        if (playerScore > 16)
        {
            state = GameState.GAMEOVER;
            return true;
        }
        return false;
    }


    //Display Game over Msg
    string EndGameMessage()
    {
        string msg = "Game is over\n";
        if (aiScore > playerScore)
        {
            msg += "AI wins!\n";
        }
        else
        {
            msg += "You Win!\n";
        }
        return msg;
    }


    //Place Markers on Board For Hit or Miss
    bool placeMarker(int x, int y, bool hit)
    {
        //Checks if Shot Hits
        if (hit)
        {
            //Spawn Marker at Shot Location
            // Renames Marker
            GameObject hitOBJ = GameObject.Instantiate(Marker);
            hitOBJ.GetComponent<Renderer>().material = Red;
            hitOBJ.transform.position = new Vector3(x, 0, y);
            hitOBJ.name = $"Hit: ({x}, {y})";

            //Plays Sound Effect
            AudioSource.PlayClipAtPoint(bombSE, hitOBJ.transform.position);
            
            //Assign Visual Effect OBJ then Call Coroutine to
            // Activate Visual Effect At Shot location
            Transform VisualEffect = hitOBJ.transform.Find("Smoke Effect");
            StartCoroutine(splashEffect(VisualEffect));

            //Returns Hit Value
            return true;
        }
        else
        {
            //Spawn Marker at Shot Location
            // Renames Marker
            GameObject hitOBJ = GameObject.Instantiate(Marker);
            hitOBJ.GetComponent<Renderer>().material = Blue;
            hitOBJ.transform.position = new Vector3(x, 0, y);
            hitOBJ.name = $"Miss: ({x}, {y})";

            //Plays Sound Effect
            AudioSource.PlayClipAtPoint(waterSE, hitOBJ.transform.position);

            //Assign Visual Effect OBJ then Call Coroutine to
            // Activate Visual Effect At Shot location
            Transform VisualEffect = hitOBJ.transform.Find("Water Spout Effect");
            StartCoroutine(splashEffect(VisualEffect));

            //Returns Hit Value
            return false;
        }
    }


    //Activates Visual Effects
    IEnumerator splashEffect(Transform effect)
    {
        effect.GetComponentInChildren<VisualEffect>().enabled = true;
        if (effect.GetComponentInChildren<Light>())
            effect.GetComponentInChildren<Light>().enabled = true;
        
        //Pause for Visual Effect Duration
        yield return new WaitForSeconds(2f);
    }


    //Returns if Shot Hit Using 2D Board Array
    bool HitCheck(int x, int y) { return !Board[x, y] ? true : false; }


    //Initialize 2D Board Array
    public void setBoard(bool[,] board) { Board = board; }


    //Custom Pause for Mouse Click
    IEnumerator WaitForMouseDown() { while (!Input.GetMouseButtonUp(0) ) { yield return null; } }

}
