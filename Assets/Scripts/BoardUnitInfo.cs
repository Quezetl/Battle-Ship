using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardUnitInfo : MonoBehaviour
{
    public TMP_Text tmpBoardLabel;
    public GameObject Cub;

    public int Row;
    public int Col;
    public bool occupied;

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
