using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AIShipPlace : MonoBehaviour
{
    bool[,] boardObj = new bool[20, 10];
    public GameObject[] aiShips;
    public GameObject boardPrefab;
    struct ships
    {
        GameObject shipObj;
        int shipLength;
        string shipName;
        Vector3 coord;
        public int getLength() { return shipLength; }
        public string getName() { return shipName; }
        public Vector3 getCoord() { return coord; }
        public void setCoord(Vector3 coord) { this.coord = coord; }
        public GameObject getShipObj() { return shipObj; }
        public void init(GameObject obj, int length, string name) { shipObj = obj; shipLength = length; shipName = name; }
    }
    ships[] botShip = new ships[5];

    // Start is called before the first frame update
    void Start()
    {
        aiShipSync();
        boardInit();
        aiShipCheck();
    }

    void aiShipSync()
    {
        int j = 2;
        for (int i = 0 ; i < 5; i++)
        {
            botShip[i].init(GameObject.Instantiate(aiShips[i]), j, aiShips[i].name);

            if (i != 1)
                j++;

        }
    }
    
    void boardInit()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                boardObj[i, j] = true;
            }
        }
        GameObject tmp;
        for (int i = 11; i < 21; i++)
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
    void aiShipCheck()
    {
        int orientation;
        int x;
        int y;

        for (int i = 0; i < 5; i++)
        {
            do
            {
                orientation = (int)Random.Range(0.0f, 4.0f);
                x = (int)Random.Range(11.0f, 20.0f);
                y = (int)Random.Range(1.0f, 10.0f);
            } while (!validPosition(x, y, orientation, i));
            botShip[i].setCoord(new Vector3(x,0,y));
            boardFill(x, y, orientation, botShip[i].getLength());
            aiShipPlace(i, orientation);
            
        }
        

    }

    void aiShipPlace(int curShip, int orientation)
    {
        int acom = 0;
        float x = botShip[curShip].getCoord().x;
        float z = botShip[curShip].getCoord().z;
        GameObject shipPlacement = botShip[curShip].getShipObj();
        float zBounds = shipPlacement.GetComponent<Collider>().bounds.size.z;
        float xBounds = shipPlacement.GetComponent<Collider>().bounds.size.x;
        float yBounds = shipPlacement.GetComponent<Collider>().bounds.size.y;
        Vector3 shipBounds = new Vector3(xBounds, yBounds, zBounds);

        shipPlacement.name = "("+botShip[curShip].getCoord().x.ToString() +", "+ botShip[curShip].getCoord().z.ToString()+")"+" Length:" + botShip[curShip].getLength() +"orientation: " + orientation;

        if (curShip == 4)
            acom = 1;
        else
            acom = 0;
        switch(orientation)
        {
            case 0:
                shipPlacement.transform.position = new Vector3(x, 0, z)+ new Vector3(0f, 0f, zBounds / (3-acom));
                shipPlacement.transform.Rotate(0, -180, 0);
                break;
            case 1:
                shipPlacement.transform.position = new Vector3(x, 0, z) + new Vector3(zBounds / (3-acom) , 0f, 0f);
                shipPlacement.transform.Rotate(0, -90, 0);
                break;
            case 2:
                shipPlacement.transform.position = new Vector3(x, 0, z) - new Vector3(0f, 0f, zBounds / (3-acom));
                shipPlacement.transform.Rotate(0, 0, 0);
                break;
            case 3:
                shipPlacement.transform.position = new Vector3(x, 0, z) - new Vector3(zBounds / (3-acom), 0f, 0f);
                shipPlacement.transform.Rotate(0, -270, 0);
                break;
        };

    }

    bool validPosition(int x, int y, int orient, int curBoat)
    {
        float bounds = botShip[curBoat].getLength();
        switch (orient)
        {
            case 0:
                if (y + botShip[curBoat].getLength() > 10)
                    return false;
                break;
            case 1:
                if (x+ botShip[curBoat].getLength() > 20 )
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

        if (curBoat == 0)
            return true;

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
        return true;
    }



}
