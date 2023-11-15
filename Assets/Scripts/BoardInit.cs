using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class BoardInit : MonoBehaviour
{
    //Initialize All Prefabs
    public GameObject Water;
    public GameObject boardPrefab;
    public GameObject[] Ships;
    public GameObject shipPreviewPrefab;
    public GameObject mouseHighlighter;
    public LayerMask layer;
    public GameObject[] buttons;
    public Material greenCarpet;

    //Initialize Internal Variables
    int orientation = 0;
    Vector3[] orientVec = new Vector3[5];
    int curShip = 999;
    RaycastHit mousePos;
    GameObject[] cubePreview = new GameObject[5];
    ships[] plyrShip = new ships[5];
    ships[] botShip = new ships[5];
    
    //Variable for Handling Whether the Board
    // has a Ship on it
    public bool[,] boardObj = new bool[20, 11];

    //Custom Structure to Handle Ship Information
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
    
    

    // Spawn Board, Assign Ships etc.
    void Start()
    {
        shipSync();
        boardInit();
        aiShipCheck();
    }


    // Main Loop Handles Ship Placement for Player
    void Update()
    {

        //Checks for if the Player has the mouse on the Board, and shows blue preivew if true
        // otherwise ends update preemtively to stop erros
        bool onScreen = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousePos);
        for (int i = 0; i < 5; i++)
        {
            cubePreview[i].SetActive(false);
        }
        if (!onScreen)
        {
            mouseHighlighter.SetActive(false);
            return;
        }
        if (!mouseOnBoard(mousePos))
        {
            mouseHighlighter.SetActive(false);
        }

        //Condition for Checking if all ships have been placed
        // if not then will check if place has valid placement for ship
        if (onScreen && !allPiecesPlaced())
        {
            if (!mouseOnBoard(mousePos))
                mouseHighlighter.SetActive(false);

            if (mouseOnBoard(mousePos))
            {
                blueBoxPreview();
                if (curShip >= 0 && curShip <= 4 && !plyrShip[curShip].isPlaced())
                {
                    if (shipPreview() && Input.GetMouseButtonUp(0) && plyrShip[curShip].isValid())
                    {
                        placeShip();
                    }
                }
            }

        }

        //Checks if This script has done everything it need to
        // after it will end itself
        if (allPiecesPlaced())
        {
            //Disables Highlighter in Cases Where Mouse is
            //Still on Board
            mouseHighlighter.SetActive(false);

            //Calls GameLoop script which handles actual game
            // and brings over 2d array of ship information
            this.GetComponent<GameLoop>().enabled = true;
            this.GetComponent<GameLoop>().setBoard(boardObj);
            this.GetComponent<BoardInit>().enabled = false;
        }

    }
    

    //Sinces up Ships with Manipulatable objects in Script
    void shipSync()
    {
        //Assigns information of player ships
        // to ship using custom structure
        int j = 2;
        for (int i = 0; i < 5; i++)
        {
            plyrShip[i].init(Ships[i], j, Ships[i].name);

            if (i != 1)
                j++;
        }

        //Instantiates Cube Preview
        for (int i = 0; i < 5; i++)
        {
            cubePreview[i] = GameObject.Instantiate(shipPreviewPrefab);
            cubePreview[i].SetActive(false);
            cubePreview[i].name = "ship preview";
        }

        //Instantiates Blue Highlighter Ship Preview
        mouseHighlighter = GameObject.Instantiate(mouseHighlighter);
        mouseHighlighter.SetActive(false);
        for(int i= 0; i < 5; i++)
        {
            orientVec[i] = new Vector3(0.0f, 0.0f, i);
        }

        //Assigns information of AI ships
        // to ship using custom structure
        j = 2;
        for (int i = 0; i < 5; i++)
        {
            botShip[i].init(GameObject.Instantiate(Ships[i]), j, Ships[i].name);

            if (i != 1)
                j++;
        }
    }


    //Creates Board & Water Shader
    void boardInit()
    {
        //Spawn Water
        GameObject.Instantiate(Water).transform.position = new Vector3(10, 0.100000001f, 5.5f);

        //GameObject to handle transforms of Board OBJs
        GameObject tmp;
        
        //Sets 2D Bool array to true for later use
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                boardObj[i, j] = true;
            }
        }

        //Creates Board
        for (int i = 0; i < 21; i++)
        {
            if (i < 10)
            {
                for (int j = 1; j < 11; j++)
                {
                    string objname = $"({i},{j})";
                    string textname = $"({i + 1},{j })";
                    tmp = GameObject.Instantiate(this.boardPrefab, new Vector3(i, 0, j), this.boardPrefab.transform.rotation) as GameObject;
                    tmp.GetComponentInChildren<TMPro.TextMeshProUGUI>(tmp).text = textname;
                    tmp.name = objname;
                    tmp.tag = "Base";
                }
            }
            else if( i > 10)
            {
                for (int j = 1; j < 11; j++)
                {
                    string objname = $"({i},{j})";
                    string textname = $"({i},{j})";
                    tmp = GameObject.Instantiate(this.boardPrefab, new Vector3(i, 0, j), this.boardPrefab.transform.rotation) as GameObject;
                    tmp.GetComponentInChildren<TMPro.TextMeshProUGUI>(tmp).text = textname;
                    tmp.name = objname;
                    tmp.tag = "aiBase";
                }
            }
        }
    }


    //Checks if All Player ships have been placed
    public bool allPiecesPlaced()
    {
        for (int i = 0; i < 5; i++)
        {
            if (!plyrShip[i].isPlaced())
                return false;
        }
        buttons[5].SetActive(false);
        return true;
    }


    //Moves Blue Highlighters to mousePos
    void blueBoxPreview()
    {
        mouseHighlighter.SetActive(true);
        mouseHighlighter.transform.position = mousePos.transform.position;
    }


    //Spawns Grey Cubes for Ship Orientation Preview
    bool shipPreview()
    {
        bool plc = true;

        //Loop Detecting Ship Collision
        for (int i = 0; i < plyrShip[curShip].getLength(); i++)
        {
            //Sets Proper Amount of Ship Preivews 
            // for Collision Detection
            cubePreview[i].SetActive(true);

            //Turns Cube Yellow on Collision & updates plc Bool
            // otherwise turns it back grey
            if (Physics.CheckBox(cubePreview[i].transform.position, cubePreview[i].transform.localScale / 2, Quaternion.identity, layer))
            {
                cubePreview[i].GetComponent<Renderer>().material = Resources.Load("Materials/Yellow", typeof(Material)) as Material;
                plc = false;
            }
            else
            {
                cubePreview[i].GetComponent<Renderer>().material = Resources.Load("Materials/Grey", typeof(Material)) as Material;
            }
        }

        //Updates Bool for if ship is Placeable
        plyrShip[curShip].updateValid(plc ? true : false);

        //Spawns Correct Amount of Ship Preview Cubes
        for (int i = 0; i < plyrShip[curShip].getLength(); i++)
        {
            cubePreview[i].transform.position = mousePos.transform.position + orientVec[i];
        }

        //Error Checking if Placement is on the Board
        if (!checkBounds())
        {
            for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                cubePreview[i].SetActive(false);
            plyrShip[curShip].updateValid(false);
            return false;
        }

        //Returns Value
        return true;
    }


    //Checks if Ship will Fit on Board
    bool checkBounds()
    {
        switch (orientation)
        {
            case 0:
                if( plyrShip[curShip].getLength() + mousePos.transform.position.z > 11)
                    return false;
                break;
            case 1:
                if(plyrShip[curShip].getLength() + mousePos.transform.position.x > 10)
                    return false;
                break;
            case 2:
                if ( mousePos.transform.position.z - plyrShip[curShip].getLength() < 0)
                    return false;
                break;
            case 3:
                if(mousePos.transform.position.x - plyrShip[curShip].getLength() < -1)
                    return false;
                break;
        }

        //Returns Value
        return true;
    }


    //Function for Placing Ship if it passes all Previous Checks
    void placeShip()
    {
        //Initialize Variables for Ship Placement
        GameObject shipPiece = GameObject.Instantiate(plyrShip[curShip].getShipObj());
        float acom;
        float x = mousePos.transform.position.x;
        float z = mousePos.transform.position.z;
        float zBounds = shipPiece.GetComponent<Collider>().bounds.size.z;

        //Set Acommadations for Certain Ships
        if (curShip == 4)
            acom = 1;
        else if (curShip == 3)
            acom = .6f;
        else
            acom = 0;

        //Place Ships According to Desired Orientation
        switch (orientation)
        {
            case 0:
                shipPiece.transform.position = mousePos.transform.position + new Vector3(0f, 0, zBounds / (3 - acom));
                shipPiece.transform.Rotate(0, -180, 0);
                break;
            case 1:
                shipPiece.transform.position = mousePos.transform.position + new Vector3(zBounds / (3 - acom), 0, 0f);
                shipPiece.transform.Rotate(0, -90, 0);
                break;
            case 2:
                shipPiece.transform.position = mousePos.transform.position - new Vector3(0f, 0, zBounds / (3 - acom));
                shipPiece.transform.Rotate(0, 0, 0);
                break;
            case 3:
                shipPiece.transform.position = mousePos.transform.position - new Vector3(zBounds / (3 - acom), 0, 0f);
                shipPiece.transform.Rotate(0, -270, 0);
                break;
        };

        //Update Value of Placed Bool
        plyrShip[curShip].updatePlaced(true);

        //Moves Preview Cubes so it doesn't Detect Improper Collision
        for (int i = 0; i < 5; i++)
            cubePreview[i].transform.position = new Vector3(0, 0, 0);

        //Assigns Coord in Ship Struct
        plyrShip[curShip].setCoord(new Vector3(x, 0, z));

        //Update 2D Board Array
        updateBoard((int)x, (int)z, orientation);

        //Get Rid of Button Assigned to Ship
        buttons[curShip].SetActive(false);
    }


    //Function to Update 2D Board array
    void updateBoard(int x, int y, int orient)
    {
        y--;
        switch (orient)
        {
            case 0:
                for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                {
                    boardObj[x, y] = false;
                    y++;
                }
                break;
            case 1:
                for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                {
                    boardObj[x, y] = false;
                    x++;
                }
                break;
            case 2:
                for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                {
                    boardObj[x, y] = false;
                    y--;
                }
                break;
            case 3:
                for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                {
                    boardObj[x, y] = false;
                    x--;
                }
                break;
        }
    }


    //Change Orientation Variable if Rotation
    // Button is Pressed
    public void orientationChange()
    {
        if (orientation > 2)
            orientation = 0;
        else
            orientation++;

        switch (orientation)
        {
            case 0:
                for (int i = 0; i < 5; i++)
                {
                    orientVec[i] = new Vector3(0.0f, 0.0f, i);
                }
                break;
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    orientVec[i] = new Vector3(i, 0.0f, 0.0f);
                }
                break;
            case 2:
                for (int i = 0; i < 5; i++)
                {
                    orientVec[i] = new Vector3(0.0f, 0.0f, -i);
                }
                break;
            case 3:
                for (int i = 0; i < 5; i++)
                {
                    orientVec[i] = new Vector3(-i, 0.0f, 0.0f);
                }
                break;
        }
    }


    //Initialize 2D Board Array
    void boardFill(int x, int y, int orientation, int length)
    {
        for (int i = 0; i < length; i++)
        {
            switch (orientation)
            {
                case 0:
                    boardObj[x, y] = false;
                    y++;
                    break;
                case 1:
                    boardObj[x, y] = false;
                    x++;
                    break;
                case 2:
                    boardObj[x, y] = false;
                    y--;
                    break;
                case 3:
                    boardObj[x, y] = false;
                    x--;
                    break;
            }
        }
    }

    
    //Function to see Where AI Can Place
    //Their Ships
    void aiShipCheck()
    {
        //Initial Variables
        int orientation;
        int x;
        int y;

        //Loop for All Ships
        for (int i = 0; i < 5; i++)
        {
            //Loop to Randomize Orientation and Coordinates
            do
            {
                orientation = (int)Random.Range(0.0f, 4.0f);
                x = (int)Random.Range(11.0f, 20.0f);
                y = (int)Random.Range(1.0f, 10.0f);
            } while (!validPosition(x, y, orientation, i));

            //Once Valid Position found
            //Update Ship Struct infro
            botShip[i].setCoord(new Vector3(x, 0, y));

            //Update 2D Board Array
            boardFill(x, y, orientation, botShip[i].getLength());

            //Place the Ship on the Board
            aiShipPlace(i, orientation);
        }
    }


    //Function for Placing Ships on AI Side
    void aiShipPlace(int curShip, int orientation)
    {
        //Initialize Variables for Ships
        GameObject shipPlacement = botShip[curShip].getShipObj();
        float acom;
        float x = botShip[curShip].getCoord().x;
        float z = botShip[curShip].getCoord().z;
        float zBounds = shipPlacement.GetComponent<Collider>().bounds.size.z;

        //Name Ship According to Relavent Infor
        shipPlacement.name = "(" + botShip[curShip].getCoord().x.ToString() + ", " + botShip[curShip].getCoord().z.ToString() + ")" + " Length:" + botShip[curShip].getLength() + "orientation: " + orientation;


        //Adjust Accomadations for Certain Ships
        if (curShip == 4)
            acom = 1;
        else if (curShip == 3)
            acom = .6f;
        else
            acom = 0;

        //Place Actual Ships
        switch (orientation)
        {
            case 0:
                shipPlacement.transform.position = new Vector3(x, 0, z) + new Vector3(0f, 0f, zBounds / (3 - acom));
                shipPlacement.transform.Rotate(0, -180, 0);
                break;
            case 1:
                shipPlacement.transform.position = new Vector3(x, 0, z) + new Vector3(zBounds / (3 - acom), 0f, 0f);
                shipPlacement.transform.Rotate(0, -90, 0);
                break;
            case 2:
                shipPlacement.transform.position = new Vector3(x, 0, z) - new Vector3(0f, 0f, zBounds / (3 - acom));
                shipPlacement.transform.Rotate(0, 0, 0);
                break;
            case 3:
                shipPlacement.transform.position = new Vector3(x, 0, z) - new Vector3(zBounds / (3 - acom), 0f, 0f);
                shipPlacement.transform.Rotate(0, -270, 0);
                break;
        };

        //Consoling out Ship Info for Error Testing
        Debug.Log(shipPlacement.name);

        //Destroy AI Ship to Free up Resources
        Destroy(shipPlacement);
    }


    //Checks if Passed Ship is in Valid Placement
    bool validPosition(int x, int y, int orient, int curBoat)
    {
        //Checks if Ship will Fit on Board
        switch (orient)
        {
            case 0:
                if (y + botShip[curBoat].getLength() > 10)
                    return false;
                break;
            case 1:
                if (x + botShip[curBoat].getLength() > 20)
                    return false;
                break;
            case 2:
                if (y - botShip[curBoat].getLength() < 0)
                    return false;
                break;
            case 3:
                if (x - botShip[curBoat].getLength() < 11)
                    return false;
                break;
        }
        
        //Checks if a Ship is Selected
        if (curBoat == 0)
            return true;

        //Checks if Ship is Valid 
        //According to 2D Board Array
        for (int i = 0; i < botShip[curBoat].getLength(); i++)
        {
            switch (orient)
            {
                case 0:
                    if (!boardObj[x, y])
                        return false;
                    y++;
                    break;
                case 1:
                    if (!boardObj[x, y])
                        return false;
                    x++;
                    break;
                case 2:
                    if (!boardObj[x, y])
                        return false;
                    y--;
                    break;
                case 3:
                    if (!boardObj[x, y])
                        return false;
                    x--;
                    break;
            }
        }

        //Returns Value
        return true;
    }


    //Helper Function Used for Changing Ship Selection According to UI
    public void currentShipChange(int val) { curShip = val; }


    //Helper Function for Checking if Mouse is on Player Board
    bool mouseOnBoard(RaycastHit mousePos) { return mousePos.collider.CompareTag("Base"); }



}
