using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{

    GameObject preview;
    Material mat;
    public GameObject ShipPreview;
    public GameObject boardUnityPrefabW;
    public GameObject boardUnityPrefabG;
    // Start is called before the first frame update
    void Start()
    {
        preview = GameObject.Instantiate(this.ShipPreview);
        preview.GetComponent<Collider>().enabled = false;
        BoardInit(0, 0);
        BoardInit(1, 11);
    }

    // Update is called once per frame
    void Update()
    {

        preview.SetActive(false);
        /*mat = Resources.Load("Materials/Dark Grey", typeof(Material)) as Material;
        preview.GetComponent<Renderer>().material = mat;*/
        bool onScreen = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit mousePos);
        if (onScreen)
        {
            preview.SetActive(true);
            if (mousePos.collider.tag.Equals("Base") == true)
            {
                mat = Resources.Load("Materials/Dark Grey", typeof(Material)) as Material;
                preview.GetComponent<Renderer>().material = mat;

                preview.transform.position = mousePos.transform.position + mousePos.normal - new Vector3(0, 0.7f, 0);
            }
            //else
                //preview.transform.position = new Vector3(mousePos.point.x, mousePos.point.y + (.1f), mousePos.point.z);


            /*if (Input.GetMouseButtonUp(0))
            {
                GameObject myCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo);
                myCube.GetComponent<Collider>().enabled = true;
                if (hit)
                {
                    Debug.Log($"You hit: {hitInfo.transform.name}");



                    Material buildingmat = Resources.Load("Materials/Dark Grey", typeof(Material)) as Material;

                    myCube.GetComponent<Renderer>().material = buildingmat;


                    if (hitInfo.transform.tag.Equals("Base"))
                    {
                        myCube.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + (0.5f), hitInfo.point.z);
                    }
                    else
                    {
                        myCube.transform.position = hitInfo.transform.position + hitInfo.normal;
                    }
                }
            }*/
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
                if (i % 2 == 0)
                {
                    if (j % 2 == 0)
                        tmp = GameObject.Instantiate(this.boardUnityPrefabG, new Vector3(opp + i, 0, -j), this.boardUnityPrefabG.transform.rotation) as GameObject;
                    else
                        tmp = GameObject.Instantiate(this.boardUnityPrefabW, new Vector3(opp + i, 0, -j), this.boardUnityPrefabW.transform.rotation) as GameObject;
                }
                else
                {
                    if (j % 2 == 0)
                        tmp = GameObject.Instantiate(this.boardUnityPrefabW, new Vector3(opp + i, 0, -j), this.boardUnityPrefabW.transform.rotation) as GameObject;
                    else
                        tmp = GameObject.Instantiate(this.boardUnityPrefabG, new Vector3(opp + i, 0, -j), this.boardUnityPrefabG.transform.rotation) as GameObject;
                }
                tmp.GetComponentInChildren<TMPro.TextMeshProUGUI>(tmp).text = name;
                row++;
            }
            col++;
            row = 1;
        }
    }
}
