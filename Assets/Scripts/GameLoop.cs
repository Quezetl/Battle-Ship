using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public GameObject shipObjcts;


    public bool[,] plyrBoard = new bool[10, 10];
    public bool[,] aiBoard = new bool[20, 10];

    bool gameStart = false;
    bool playerShipsSet = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkGameReady())
        {
            return;
        }
        if (!playerShipsSet)
        {
            setShipLayout();
            playerShipsSet = true;
        }

        Debug.Log("Game Start");
    }


    void setShipLayout()
    {
        plyrBoard = shipObjcts.GetComponent<PlayerBoardSetup>().playerBoard;
        aiBoard = shipObjcts.GetComponent<AIShipPlace>().boardObj;
    }
    bool checkGameReady()
    {
        return shipObjcts.GetComponent<PlayerBoardSetup>().allPiecesPlaced();
    }
}
