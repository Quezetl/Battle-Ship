using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoardUnitInfo : MonoBehaviour
{
    public TMP_Text tmpBoardLabel;
    public GameObject Cub;

    public bool[] shipPlaceable = new bool[5] { true, true, true, true, true };
    public int Row;
    public int Col;
    public bool occupied;

    public int shipLength(int value)
    {
        switch(value)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 3;
            case 3:
                return 4;
            case 4:
                return 5;
            default:
                return 0;
        }
    }
    private void Awake()
    {
        tmpBoardLabel.text = "{10,10}";
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(occupied)
        {

        }
    }
}
