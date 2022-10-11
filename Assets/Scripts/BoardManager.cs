using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public GameObject boardUnityPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
        int row = 1;
        int col = 1;

        for(int i = 0; i < 10; i++)
        {
            for(int j= 0; j< 10; j++)
            {
                GameObject tmp = GameObject.Instantiate(this.boardUnityPrefab, new Vector3(i, 0, j), this.boardUnityPrefab.transform.rotation) as GameObject;

                BoardUnitInfo tmpBoardUnit = tmp.GetComponent<BoardUnitInfo>();
                string name = $"{{{row},{col}}}";
                //tmpBoardUnit;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
