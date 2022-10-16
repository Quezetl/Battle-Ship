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

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            preview[i] = GameObject.Instantiate(this.ShipPreview);
            preview[i].GetComponent<Collider>().enabled = false;
        }
        BoardInit(0, 0);
        BoardInit(1, 11);
        tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tmp.GetComponent<Collider>().enabled = false;
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
            /*tmpBlock = GameObject.Instantiate(this.ShipPreview)*/
            for (int i = 0; i < 5; i++)
                preview[i].SetActive(false);
            if (mousePos.collider.CompareTag("Base"))
            {
                temp = mousePos.collider.gameObject;
                tmp.transform.localScale = new Vector3(1.0f, .11f, 1.0f);
                tmp.GetComponent<Collider>().enabled = false;
                Debug.Log("you have moused over " + temp);
                boardMat = Resources.Load("Materials/Blue", typeof(Material)) as Material;
                tmp.GetComponent<Renderer>().material = boardMat;
                previewMat = Resources.Load("Materials/Grey", typeof(Material)) as Material;


                tmp.SetActive(true);
                tmp.transform.position = temp.transform.position;


                for (int i = 0; i < 5; i++)
                    preview[i].GetComponent<Renderer>().material = previewMat;
                //boardMat.GetComponent<Renderer>().material =

                for (int i = 0; i < boardUIMana.shipLength; i++)
                    preview[i].SetActive(true);
                if (!shipInfo.shipPlaceable[boardUIMana.shipChoice - 1])
                    return;

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
                //Destroy(tmp);

            }

            //else
                //preview.transform.position = new Vector3(mousePos.point.x, mousePos.point.y + (.1f), mousePos.point.z);


            if (Input.GetMouseButtonUp(0))
            {
                if (!shipInfo.shipPlaceable[boardUIMana.shipChoice - 1])
                    return;
                ship[boardUIMana.shipChoice-1] = GameObject.Instantiate(this.shipArray[boardUIMana.shipChoice-1]);
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo);

                if (hit)
                {
                    Debug.Log($"You hit: {hitInfo.transform.name}");

                    if (hitInfo.transform.tag.Equals("Base"))
                    {
                        switch (boardUIMana.orientation)
                        {
                            case 0:
                                ship[boardUIMana.shipChoice - 1].transform.position = temp.transform.position;
                                ship[boardUIMana.shipChoice - 1].transform.Rotate(0, 180.0f, 0);
                                break;
                            case 1:
                                ship[boardUIMana.shipChoice - 1].transform.position = temp.transform.position;
                                ship[boardUIMana.shipChoice - 1].transform.Rotate(0, 270.0f, 0);
                                break;
                            case 2:
                                ship[boardUIMana.shipChoice - 1].transform.position = temp.transform.position;
                                ship[boardUIMana.shipChoice - 1].transform.Rotate(0, 0.0f, 0);
                                break;
                            case 3:
                                ship[boardUIMana.shipChoice - 1].transform.position = temp.transform.position;
                                ship[boardUIMana.shipChoice - 1].transform.Rotate(0, 90.0f, 0);
                                break;
                        }

                    }
                }
                shipInfo.shipPlaceable[boardUIMana.shipChoice - 1] = false;
                
            }
        }


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
                string name = $"B{plyr}: ({col},{row})";
                tmp = GameObject.Instantiate(this.boardUnityPrefab, new Vector3(opp + i, 0, -j), this.boardUnityPrefab.transform.rotation) as GameObject;
                tmp.GetComponentInChildren<TMPro.TextMeshProUGUI>(tmp).text = name;
                tmp.name = name;
                row++;
            }
            col++;
            row = 1;
        }
    }

    public void ShipButtoncalled(int value) { boardUIMana.SelectedBoardPiece(value); }
    public void OrientaionChange() { boardUIMana.ChangeOrientation(); }
}
