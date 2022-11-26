using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerBoardSetup : MonoBehaviour
{
    public bool[,] playerBoard = new bool[10, 10];
    public GameObject GameManager;
    public GameObject boardPrefab;
    public GameObject[] playerShips;
    public GameObject shipPreviewPrefab;
    public GameObject mouseHighlighter;
    public LayerMask layer;
    public GameObject[] buttons;
    public Material greenCarpet;

    int orientation = 0;
    Vector3[] orientVec = new Vector3[5];
    int curShip = 999;
    RaycastHit mousePos;
    GameObject[] cubePreview = new GameObject[5];
    ships[] plyrShip = new ships[5];



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
        shipSync();
        boardInit();
    }

    // Update is called once per frame
    void Update()
    {

        bool onScreen = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousePos);
        for (int i = 0; i < 5; i++)
        {
            cubePreview[i].SetActive(false);
        }
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

    }
    

    void shipSync()
    {
        int j = 2;
        for (int i = 0; i < 5; i++)
        {
            plyrShip[i].init(playerShips[i], j, playerShips[i].name);

            if (i != 1)
                j++;
        }

        for (int i = 0; i < 5; i++)
        {
            cubePreview[i] = GameObject.Instantiate(shipPreviewPrefab);
            cubePreview[i].SetActive(false);
            cubePreview[i].name = "ship preview";
        }

        mouseHighlighter = GameObject.Instantiate(mouseHighlighter);
        mouseHighlighter.SetActive(false);
        for(int i= 0; i < 5; i++)
        {
            orientVec[i] = new Vector3(0.0f, 0.0f, i);
        }
    }

    void boardInit()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                playerBoard[i, j] = true;
            }
        }

        GameObject tmp;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                string objname = $"({i},{j})";
                string textname = $"({i + 1},{j + 1})";
                tmp = GameObject.Instantiate(this.boardPrefab, new Vector3(i, 0, j), this.boardPrefab.transform.rotation) as GameObject;
                tmp.GetComponentInChildren<TMPro.TextMeshProUGUI>(tmp).text = textname;
                tmp.name = objname;
                tmp.tag = "Base";
            }
        }
    }

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
    void blueBoxPreview()
    {
        mouseHighlighter.SetActive(true);
        mouseHighlighter.transform.position = mousePos.transform.position;
    }

    bool shipPreview()
    {
        bool plc = true;
        for (int i = 0; i < plyrShip[curShip].getLength(); i++)
        {
            cubePreview[i].SetActive(true);
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
        plyrShip[curShip].updateValid(plc ? true : false);

        for (int i = 0; i < plyrShip[curShip].getLength(); i++)
        {
            cubePreview[i].transform.position = mousePos.transform.position + orientVec[i];
        }
        if (!checkBounds())
        {
            for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                cubePreview[i].SetActive(false);
            plyrShip[curShip].updateValid(false);
            return false;
        }

        return true;
    }

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

        return true;
    }

    void placeShip()
    {
        float acom = 0;
        float x = mousePos.transform.position.x;
        float z = mousePos.transform.position.z;
        GameObject shipPiece = GameObject.Instantiate(plyrShip[curShip].getShipObj());
        float zBounds = shipPiece.GetComponent<Collider>().bounds.size.z;
        float xBounds = shipPiece.GetComponent<Collider>().bounds.size.x;
        float yBounds = shipPiece.GetComponent<Collider>().bounds.size.y;
        Vector3 shipBounds = new Vector3(xBounds, yBounds, zBounds);

        if (curShip == 4)
            acom = 1;
        else if (curShip == 3)
            acom = .6f;
        else
            acom = 0;


        switch (orientation)
        {
            case 0:
                shipPiece.transform.position = mousePos.transform.position + new Vector3(0f, 0f, zBounds / (3 - acom));
                shipPiece.transform.Rotate(0, -180, 0);
                break;
            case 1:
                shipPiece.transform.position = mousePos.transform.position + new Vector3(zBounds / (3 - acom), 0f, 0f);
                shipPiece.transform.Rotate(0, -90, 0);
                break;
            case 2:
                shipPiece.transform.position = mousePos.transform.position - new Vector3(0f, 0f, zBounds / (3 - acom));
                shipPiece.transform.Rotate(0, 0, 0);
                break;
            case 3:
                shipPiece.transform.position = mousePos.transform.position - new Vector3(zBounds / (3 - acom), 0f, 0f);
                shipPiece.transform.Rotate(0, -270, 0);
                break;
        };

        for (int i = 0; i < plyrShip[curShip].getLength(); i++)
        {
            GameObject shipCarpet = GameObject.Instantiate(mouseHighlighter);
            shipCarpet.GetComponent<Renderer>().material = greenCarpet;
            shipCarpet.transform.position = mousePos.transform.position + orientVec[i];

        }

        plyrShip[curShip].updatePlaced(true);
        for (int i = 0; i < 5; i++)
        {
            cubePreview[i].transform.position = new Vector3(0, 0, 0);
        }
        plyrShip[curShip].setCoord(new Vector3(x, 0, z));
        updateBoard((int)x, (int)z, orientation);
        buttons[curShip].SetActive(false);
    }

    void updateBoard(int x, int y, int orient)
    {
        switch (orient)
        {
            case 0:
                for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                {
                    playerBoard[x, y] = false;
                    y++;
                }
                break;
            case 1:
                for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                {
                    playerBoard[x, y] = false;
                    x++;
                }
                break;
            case 2:
                for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                {
                    playerBoard[x, y] = false;
                    y--;
                }
                break;
            case 3:
                for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                {
                    playerBoard[x, y] = false;
                    x--;
                }
                break;
        }
    }

    bool mouseOnBoard(RaycastHit mousePos)
    {
        return mousePos.collider.CompareTag("Base");
    }


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
    public void currentShipChange(int val)
    {
        curShip = val;
    }
}
