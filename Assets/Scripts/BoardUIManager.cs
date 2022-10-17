using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUIManager : MonoBehaviour
{
    public int shipChoice;
    public int orientation;
    public int shipLength;
    // Start is called before the first frame update
    void Start()
    {
        orientation = 0;
        shipChoice = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SelectedBoardPiece(int value)
    {
        shipChoice = value;
        switch(value)
        {
            case 1:
                shipLength = 2;
                break;
            case 2:
            case 3:
                shipLength = 3;
                break;
            case 4:
                shipLength = 4;
                break;
            case 5:
                shipLength = 5;
                break;
        default:
                shipLength = 0;
                break;
        }    

        //some logic to distinguish between the different types
    }

    public void ChangeOrientation()
    {
        if (orientation >= 3)
            orientation = 0;
        else
            orientation++;
    }
}
