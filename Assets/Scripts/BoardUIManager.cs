using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectedBoardPiece(int value)
    {
        Debug.Log($"User selected piecee # {value}");

        //some logic to distinguish between the different types
    }

    public void ChangeOrientation(bool orientation)
    {
        orientation = !orientation;
        Debug.Log($"you have selected orientation: {orientation}");
    }
}
