using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    public LayerMask layer;
    BoardUIManager boardUIMana= new BoardUIManager();
    BoardUnitInfo shipInfo = new BoardUnitInfo();
    GameObject[] preview = new GameObject[5];
    GameObject[] ship = new GameObject[5];
    public GameObject[] shipArray = new GameObject[5];
    GameObject tmp;
    GameObject temp;
    Material previewMat;
    Material boardMat;
    public GameObject ShipPreview;
    public GameObject boardUnityPrefab;
    public bool[] previewable = new bool[5];

    public GameObject[] ShipButtons = new GameObject[5];



    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            preview[i] = GameObject.Instantiate(this.ShipPreview);
        }
        BoardInit(0, 0);
        BoardInit(1, 11);
        tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tmp.GetComponent<Collider>().enabled = false;
        previewMat = Resources.Load("Materials/Grey", typeof(Material)) as Material;
        boardMat = Resources.Load("Materials/Blue", typeof(Material)) as Material;
        for (int i = 0; i < 5; i++)
        {
            previewable[i] = true;
            preview[i].GetComponent<Renderer>().material = previewMat;
        }
        for(int i = 0; i < 5; i++)
            aiShipPlacement(i);

    }

    void aiShipPlacement(int j)
    {
        int aiOrient = (int)Random.Range(0.0f, 4.0f);

        ship[j] = GameObject.Instantiate(this.shipArray[j]);
        float pos = ship[j].GetComponent<Collider>().bounds.size.z;
        GameObject cuber = ship[j];
        int x, z;

        Vector3 coord;

        switch (aiOrient)
        {
            case 0:
                while (true)
                {
                    x = (int)Random.Range(11.0f, 21.0f);
                    z = (int)Random.Range(0f, 10.0f);
                    if ((j == 0 && x < 8) ||
                        (j == 1 && x < 7) ||
                        (j == 2 && x < 7) ||
                        (j == 3 && x < 6) ||
                        (j == 4 && x < 5))
                        break;
                }
                coord = new Vector3(x, 0, z);
                ship[j].transform.Rotate(0, 180.0f, 0);
                cuber.name = $"Cuber #{j}, for orient {aiOrient}";
                break;
            case 1:
                while (true)
                {
                    x = (int)Random.Range(11.0f, 21.0f);
                    z = (int)Random.Range(0f, 10.0f);
                    if ((j == 0 && z < 8) ||
                        (j == 1 && z < 7) ||
                        (j == 2 && z < 7) ||
                        (j == 3 && z < 6) ||
                        (j == 4 && z < 5))
                        break;
                }
                coord = new Vector3(x, 0, z);
                ship[j].transform.position = new Vector3(x, 0, z);
                ship[j].transform.Rotate(0, 270.0f, 0);
                cuber.name = $"Cuber #{j}, for orient {aiOrient}";
                break;
            case 2:
                while (true)
                {
                    x = (int)Random.Range(11.0f, 21.0f);
                    z = (int)Random.Range(0f, 10.0f);
                    if ((j == 0 && x > 2) ||
                        (j == 1 && x > 3) ||
                        (j == 2 && x > 3) ||
                        (j == 3 && x > 4) ||
                        (j == 4 && x > 5))
                        break;
                }
                coord = new Vector3(x, 0, z);
                ship[j].transform.position = new Vector3(x, 0, z);
                ship[j].transform.Rotate(0, 0.0f, 0);
                cuber.name = $"Cuber #{j}, for orient {aiOrient}";
                break;
            case 3:
                while (true)
                {
                    x = (int)Random.Range(11.0f, 21.0f);
                    z = (int)Random.Range(0f, 10.0f);
                    if ((j == 0 && z > 2) ||
                        (j == 1 && z > 3) ||
                        (j == 2 && z > 3) ||
                        (j == 3 && z > 4) ||
                        (j == 4 && z > 5))
                        break;
                }
                coord = new Vector3(x, 0, z);
                ship[j].transform.position = new Vector3(x, 0, z);
                ship[j].transform.Rotate(0, 90.0f, 0);
                cuber.name = $"Cuber #{j}, for orient {aiOrient}";
                break;
            default:
                break;
        }

    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 5; i++)
            preview[i].SetActive(false);
        tmp.SetActive(false);
        bool onScreen = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit mousePos);
        if (onScreen)
        {
            for (int i = 0; i < 5; i++)
                preview[i].SetActive(false);
            if (mousePos.collider.CompareTag("Base"))
            {
                temp = mousePos.collider.gameObject;
                tmp.transform.localScale = new Vector3(1.0f, .11f, 1.0f);
                tmp.GetComponent<Collider>().enabled = false;
                tmp.GetComponent<Renderer>().material = boardMat;


                tmp.SetActive(true);
                tmp.transform.position = temp.transform.position;

                

                for (int i = 0; i < boardUIMana.shipLength; i++)
                    preview[i].SetActive(true);
                
                if (!shipPlaceable(boardUIMana.shipChoice,boardUIMana.orientation))
                {
                    for (int i = 0; i < boardUIMana.shipLength; i++)
                        preview[i].SetActive(false);
                    return;
                }

                shipPreview(mousePos);


            }


            if (Input.GetMouseButtonUp(0))
            {
                if (!shipInfo.shipPlaceable[boardUIMana.shipChoice - 1])
                    return;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo);
                for (int i = 0; i < 5; i++)
                    if (!previewable[i])
                        return;
                if (hit)
                {
                    if (hitInfo.transform.tag.Equals("Base"))
                    {
                        shipPlace();
                    }
                }
                
            }
        }


    }

    void shipPlace()
    {
        GameObject[] shipCarpet = new GameObject[5];
        Material greenCarpet = Resources.Load("Materials/Green", typeof(Material)) as Material;
        for (int i = 0; i < 5; i++)
        {
            shipCarpet[i] = GameObject.Instantiate(tmp);
            shipCarpet[i].GetComponent<Renderer>().material = greenCarpet;
            shipCarpet[i].SetActive(false);
            shipCarpet[i].name = $"green carpet for ship:{boardUIMana.shipChoice}";
        }
            
        ship[boardUIMana.shipChoice - 1] = GameObject.Instantiate(this.shipArray[boardUIMana.shipChoice - 1]);
        float pos = ship[boardUIMana.shipChoice - 1].GetComponent<Collider>().bounds.size.z;
        switch (boardUIMana.orientation)
        {
            case 0:
                ship[boardUIMana.shipChoice - 1].transform.position = temp.transform.position + new Vector3(0f, 0f, pos / 2 - .2f);
                ship[boardUIMana.shipChoice - 1].transform.Rotate(0, 180.0f, 0);
                for (int i = 0; i < boardUIMana.shipLength; i++)
                {
                    shipCarpet[i].transform.position = tmp.transform.position + new Vector3(0f, 0f, (float)i);
                    shipCarpet[i].SetActive(true);
                }
                break;
            case 1:
                ship[boardUIMana.shipChoice - 1].transform.position = temp.transform.position + new Vector3(pos / 2 - .2f, 0f, 0f);
                ship[boardUIMana.shipChoice - 1].transform.Rotate(0, 270.0f, 0);
                for (int i = 0; i < boardUIMana.shipLength; i++)
                {
                    shipCarpet[i].transform.position = tmp.transform.position + new Vector3((float)i, 0f, 0);
                    shipCarpet[i].SetActive(true);
                }
                break;
            case 2:
                ship[boardUIMana.shipChoice - 1].transform.position = temp.transform.position + new Vector3(0f, 0f, -(pos / 2 - .2f));
                ship[boardUIMana.shipChoice - 1].transform.Rotate(0, 0.0f, 0);
                for (int i = 0; i < boardUIMana.shipLength; i++)
                {
                    shipCarpet[i].transform.position = tmp.transform.position + new Vector3(0f, 0f, -(float)i);
                    shipCarpet[i].SetActive(true);
                }
                break;
            case 3:
                ship[boardUIMana.shipChoice - 1].transform.position = temp.transform.position + new Vector3(-(pos / 2 - .2f), 0f, 0f);
                ship[boardUIMana.shipChoice - 1].transform.Rotate(0, 90.0f, 0);
                for (int i = 0; i < boardUIMana.shipLength; i++)
                {
                    shipCarpet[i].transform.position = tmp.transform.position + new Vector3(-(float)i, 0f, 0f);
                    shipCarpet[i].SetActive(true);
                }
                break;
        }
        shipInfo.shipPlaceable[boardUIMana.shipChoice - 1] = false;
        ShipButtons[boardUIMana.shipChoice - 1].SetActive(false);
        Debug.Log($"Ship: {ship[boardUIMana.shipChoice - 1].name} has been placed successfully");
    }

    void BoardInit(int plyr, int opp)
    {
        GameObject tmp;
        int row = 1;
        int col = 1;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                string objname = $"B{plyr}: ({col - 1},{row - 1})";
                string textname = $"B{plyr}: ({row},{col})";
                tmp = GameObject.Instantiate(this.boardUnityPrefab, new Vector3(opp + i, 0, j), this.boardUnityPrefab.transform.rotation) as GameObject;
                tmp.GetComponentInChildren<TMPro.TextMeshProUGUI>(tmp).text = textname;
                tmp.name = objname;
                row++;
            }
            col++;
            row = 1;
        }
    }

    void shipPreview(RaycastHit mousePos)
    {
        for (int i = 0; i < 5; i++)
        {
            if (Physics.CheckBox(preview[i].transform.position, preview[i].transform.localScale / 2, Quaternion.identity, layer))
            {
                preview[i].GetComponent<Renderer>().material = Resources.Load("Materials/Yellow", typeof(Material)) as Material;
                previewable[i] = false;
            }
            else
            {
                preview[i].GetComponent<Renderer>().material = Resources.Load("Materials/Grey", typeof(Material)) as Material;
                previewable[i] = true;
            }
            if (preview[i].active == false)
            {
                previewable[i] = true;
            }
        }

        /*int i = 0;
        Collider[] hitColliders = Physics.OverlapBox(preview[1].transform.position, transform.localScale / 2, Quaternion.identity, 6);
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            Debug.Log("Hit : " + hitColliders[i].name + i);
            //Increase the number of Colliders in the array
            i++;
        }

        while(hitColliders.Length > 0)
        {
            Debug.Log($"Hit: {hitColliders[0].name}");
        }*/

        switch (boardUIMana.shipChoice)
        {
            
            case 1:
                switch (boardUIMana.orientation)
                {
                    case 0:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -1);
                        break;

                    case 1:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-1, 0.7f, 0);
                        break;

                    case 2:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 1);
                        break;

                    case 3:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(1, 0.7f, 0);
                        break;
                }
                break;

            case 2:
                switch (boardUIMana.orientation)
                {
                    case 0:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -1);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -2);
                        break;

                    case 1:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-1, 0.7f, 0);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-2, 0.7f, 0);
                        break;

                    case 2:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 1);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 2);
                        break;

                    case 3:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(1, 0.7f, 0);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(2, 0.7f, 0);
                        break;
                }
                break;

            case 3:
                switch (boardUIMana.orientation)
                {
                    case 0:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -1);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -2);
                        break;

                    case 1:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-1, 0.7f, 0);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-2, 0.7f, 0);
                        break;

                    case 2:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 1);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 2);
                        break;

                    case 3:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(1, 0.7f, 0);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(2, 0.7f, 0);
                        break;
                }
                break;

            case 4:
                switch (boardUIMana.orientation)
                {
                    case 0:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -1);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -2);
                        preview[3].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -3);
                        break;

                    case 1:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-1, 0.7f, 0);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-2, 0.7f, 0);
                        preview[3].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-3, 0.7f, 0);
                        break;

                    case 2:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 1);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 2);
                        preview[3].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 3);
                        break;

                    case 3:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(1, 0.7f, 0);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(2, 0.7f, 0);
                        preview[3].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(3, 0.7f, 0);
                        break;
                }
                break;

            case 5:
                switch (boardUIMana.orientation)
                {
                    case 0:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -1);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -2);
                        preview[3].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -3);
                        preview[4].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, -4);
                        break;

                    case 1:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-1, 0.7f, 0);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-2, 0.7f, 0);
                        preview[3].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-3, 0.7f, 0);
                        preview[4].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(-4, 0.7f, 0);
                        break;

                    case 2:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 1);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 2);
                        preview[3].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 3);
                        preview[4].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 4);
                        break;

                    case 3:
                        preview[0].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
                        preview[1].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(1, 0.7f, 0);
                        preview[2].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(2, 0.7f, 0);
                        preview[3].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(3, 0.7f, 0);
                        preview[4].transform.position = mousePos.transform.position + mousePos.normal - new Vector3(4, 0.7f, 0);
                        break;
                }
                break;
        }
    }

    public bool shipPlaceable(int unitChoice, int Orient)
    {
        if (boardUIMana.shipChoice>0)
            if (!shipInfo.shipPlaceable[boardUIMana.shipChoice - 1])
                return false;

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit mousePos);

        switch (Orient)
        {
            case 0:
                if (shipInfo.shipLength(boardUIMana.shipChoice - 1) + mousePos.transform.position.z > 10)
                    return false;
                break;
            case 1:
                if (shipInfo.shipLength(boardUIMana.shipChoice - 1) + mousePos.transform.position.x > 10)
                    return false;
                break;
            case 2:
                if (mousePos.transform.position.z - shipInfo.shipLength(boardUIMana.shipChoice - 1) < -1)
                    return false;
                break;
            case 3:
                if (mousePos.transform.position.x - shipInfo.shipLength(boardUIMana.shipChoice - 1) < -1)
                    return false;
                break;
            default:
                break;
        }



        return true;
    }
    public void ShipButtoncalled(int value) { boardUIMana.SelectedBoardPiece(value); }
    public void OrientaionChange() { boardUIMana.ChangeOrientation();}
}
