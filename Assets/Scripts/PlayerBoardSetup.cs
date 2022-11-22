using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoardSetup : MonoBehaviour
{
    public bool[,] playerBoard = new bool[10, 10];
    public GameObject GameManager;
    public GameObject boardPrefab;
    public GameObject[] playerShips;
    public GameObject shipPreviewPrefab;
    public GameObject mouseHighlighter;

    int orientation = 0;
    int curShip = 999;
    RaycastHit mousePos;
    GameObject mouseObj;
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
        bool isPlaced() { return placed; }
        public int getLength() { return shipLength; }
        public string getName() { return shipName; }
        public Vector3 getCoord() { return coord; }
        public int getOrientation() { return orientation; }
        void updatePlaced(bool placed) { this.placed = placed; }
        public void setCoord(Vector3 coord) { this.coord = coord; }
        public GameObject getShipObj() { return shipObj; }
        public void init(GameObject obj, int length, string name) { shipObj = obj; shipLength = length; shipName = name; placed = false; orientation = 0; }
    }
    // Start is called before the first frame update
    void Start()
    {
        shipSync();
        boardInit();
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

    // Update is called once per frame
    void Update()
    {
        bool onScreen = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit mousePos);
        if (onScreen)
        {
            if (mouseOnBoard(mousePos))
            {
                mouseObj = mousePos.collider.gameObject;
                blueBoxPreview();
                if (curShip >= 0 && curShip <= 4)
                {
                    shipPreview();
                }
            }

            Debug.Log("mouse on board");
        }


    }

    void blueBoxPreview()
    {
        mouseHighlighter.SetActive(true);
        mouseHighlighter.transform.position = mouseObj.transform.position;
    }

    void shipPreview()
    {
        for (int i = 0; i < plyrShip[i].getLength(); i++)
            cubePreview[i].SetActive(true);

        if (!shipPlaceable())
        {
            for (int i = 0; i < plyrShip[curShip].getLength(); i++)
                cubePreview[i].SetActive(false);
            return;
        }
    }

    bool shipPlaceable()
    {
        switch (orientation)
        {
            case 0:
                if(plyrShip[curShip].getLength() > 11)
                    return false;
                break;
            case 1:
                if(plyrShip[curShip].getLength() < 10)
                    return false;
                break;
            case 2:
                if (plyrShip[curShip].getLength() < 1)
                    return false;
                break;
            case 3:
                if(plyrShip[curShip].getLength() < 0)
                    return false;
                break;
        }

        if (!checkShipCollision())
            return false;
        return true;
    }

    bool checkShipCollision()
    {
        //Add code to check for collisions
        return true;
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
    }
    public void currentShipChange(int val)
    {
        curShip = val;
    }
}
