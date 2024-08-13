using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.U2D;
using System.Net.NetworkInformation;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] provinces;
    public Canvas canvas;
    readonly Dictionary<string, GameObject> nations = new();

    //DATE
    public int day;
    public int month;
    public int timeInterval;

    Element[] elements;

    Unit selectedUnit;
    public readonly List<GameObject> units = new();

    private readonly string provinceHistDataFolder = "Assets/History/Provinces/";
    private readonly string provinceComDataFolder = "Assets/Common/Provinces/";
    private readonly string nationHistDataFolder = "Assets/History/Countries/";
    private readonly string nationComDataFolder = "Assets/Common/Countries/";

    public camera cameraFollow;
    private Vector3 cameraFollowPosition;

    Nation playerNation;
    PanelUI pu;

    void Start()
    {
        LoadNationData();
        timeInterval = 8;

        playerNation = nations["ISR"].GetComponent<Nation>();

        GameObject[] nationsArray = nations.Values.ToArray();
        Nation[] nationsScript = new Nation[nations.Count];
        Province[] provincesScript = new Province[provinces.Length];


        for (int i = 0; i < provinces.Length; i++)
        {
            provincesScript[i] = provinces[i].GetComponent<Province>();
        }

        for (int i = 0; i < nations.Count; i++)
        {
            nationsScript[i] = nationsArray[i].GetComponent<Nation>();
        }

        elements = new Element[nationsScript.Length + provincesScript.Length];
        nationsScript.CopyTo(elements, 0);
        provincesScript.CopyTo(elements, nationsScript.Length);

        cameraFollow.Setup(() => cameraFollowPosition);
        foreach (GameObject p in provinces)
        {
            Province a = p.GetComponent<Province>();
            LoadProvinceData(a, p.name);
        }

        foreach (GameObject p in provinces)
        {
            //COM ELSE
            string filePath = provinceComDataFolder + p.name + ".txt";
            string id = "";
            string weight = "";
            string terrain = "";

            if (File.Exists(filePath))
            {
                string data = File.ReadAllText(filePath);
                string[] subs = data.Split('=', '\n');

                for (int i = 0; i < subs.Length; i++)
                {
                    if (subs[i].Trim().Equals("id"))
                    {
                        id = subs[i + 1];
                    } else if (subs[i].Trim().Equals("weight"))
                    {
                        weight = subs[i + 1];
                    } else if (subs[i].Trim().Equals("terrain")) {
                        terrain = subs[i + 1];
                    }
                }
            } else
            {
                Debug.Log("file not found + " + p.name);
            }

            p.GetComponent<Province>().InitCom(id, weight, terrain);   
        }

        foreach (GameObject p in provinces)
        {
            //NEIGHBOURS
            string filePath = provinceComDataFolder + p.name + ".txt";

            if (File.Exists(filePath))
            {
                string data = File.ReadAllText(filePath);
                string[] subs = data.Split('=', '\n');

                List<GameObject> neighbours = new();

                for (int i = 0; i < subs.Length; i++)
                {
                    if (subs[i].Trim().Equals("neighbours"))
                    {
                        string[] nbs = subs[i + 1].Split(",");


                        for (int j = 0; j < nbs.Length; j++)
                        {
                            foreach (GameObject province in provinces)
                            {
                                if (!nbs[j].Trim().Equals("null"))
                                {
                                    if (province.GetComponent<Province>().provinceId == Int32.Parse(nbs[j].Trim()))
                                    {
                                        neighbours.Add(province);
                                    }
                                }
                            }
                        }
                        
                    }
                }

                p.GetComponent<Province>().InitNeighbours(neighbours);
            }
        }

        foreach (GameObject p in nations.Values)
        {
            p.GetComponent<Nation>().InitArmy(p.GetComponent<Nation>().capital);
            units.AddRange(p.GetComponent<Nation>().unitList);
        }

        StartCoroutine(Days());
        StartCoroutine(Tick());

        var gett = canvas.transform.Find("TopPanelOutline");

        gett.GetComponent<UI>().Display(playerNation.manpower + "k", 12, 13, Resources.Load<Sprite>("Flags/" + playerNation.nationTag), playerNation.prefix + " " + playerNation.nationName);


    }

    public float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    #region Tick, day and month
    IEnumerator Tick() //MONTH
    {

        for (; ; )
        {

            var gett = canvas.transform.Find("TopPanelOutline");

            gett.GetComponent<UI>().Display(playerNation.manpower + "k", 12, 13, Resources.Load<Sprite>("Flags/" + playerNation.nationTag), playerNation.prefix + " " + playerNation.nationName);

            foreach (GameObject n in nations.Values)
            {
                var n2 = n.GetComponent<Nation>().unitList;
                if (n2 != null || n2.Count > 0)
                {
                    foreach (GameObject u in n2)
                    {
                        if (u.GetComponent<Unit>().battleObj != null)
                        {
                            var ul = u.GetComponent<Unit>().battleObj;
                            if (ul != null && ul.GetComponent<Battle>().instantiated)
                            {
                                ul.GetComponent<Battle>().Tick();
                            }
                        }
                        
                    }
                }
           
            }


            foreach (Element elm in elements)
            {
                if (elm != null)
                {
                    elm.Tick();
                }

            }

            yield return new WaitForSeconds(timeInterval);
        }

    }

    IEnumerator Days()
    {
        for (; ; )
        {
            if (day == 30)
            {
                day = 1;
                if (month != 12)
                {
                    month++;
                }
                else
                {
                    month = 1;
                }

            }
            day++;

            foreach (GameObject u in units)
            {
                if (u != null)
                {
                    var uComp = u.GetComponent<Unit>();

                    if (uComp.isMoving)
                    {
                        uComp.daysToGoal--;
                        uComp.daysToNextNode--;
                    }
                }
                
            }


            yield return new WaitForSeconds(0.26f);
        }


    }
    #endregion

    #region Province and Nation Data Loading
    public void LoadProvinceData(Province p, string provinceName)
    {
        //TAG + POP + NAME (FN)
        string filePath = provinceHistDataFolder + provinceName + ".txt";
        string name = provinceName.Split('-')[1];

        if (File.Exists(filePath))
        {
            string tag = "";
            string pop = "";

            string data = File.ReadAllText(filePath);
            string[] subs = data.Split('=', '\n');

            for (int i = 0; i < subs.Length; i++)
            {
                if (subs[i].Trim().Equals("tag")) {
                    tag = subs[i + 1].Trim();
                }

                if (subs[i].Trim().Equals("pop"))
                {
                    pop = subs[i + 1].Trim();
                }
            }

            Debug.Log(tag);
            var nat = nations[tag];

            p.Init(name, pop, tag, nat.GetComponent<Nation>());
            nat.GetComponent<Nation>().AddProvince(provinceName.Split('-')[0].Trim(), p);

        }
        else
        {
            Debug.Log("file not found " + provinceName);
        }
    }

    public void LoadNationData()
    {
        string[] files = Directory.GetFiles(nationComDataFolder);
        string[] histFiles = Directory.GetFiles(nationHistDataFolder);

        for (int i = 0; i < files.Length; i++)
        {
            if (File.Exists(files[i]) && files[i].Substring(files[i].Length - 3, 3).Equals("txt"))
            {
                string[] nSubs = files[i].Split('/', '.');
                string tag = "";
                string prefix = "";
                string name = "";
                GameObject nat = new(nSubs[3]);

                string data = File.ReadAllText(files[i]);
                string[] subs = data.Split('=',',', '\n');

                Color32 color = new();
                Color32 color2 = new();

                for (int p = 0; p < subs.Length; p++)
                {
                    if (subs[p].Trim().Equals("color"))
                    {
                        color = new(Byte.Parse(subs[p + 1]), Byte.Parse(subs[p + 2]), Byte.Parse(subs[p + 3]), Byte.Parse("135"));
                    }
                    else if (subs[p].Trim().Equals("tag"))
                    {
                        tag = subs[p + 1].Trim();
                    }
                    else if (subs[p].Trim().Equals("color2"))
                    {
                        color2 = new(Byte.Parse(subs[p + 1]), Byte.Parse(subs[p + 2]), Byte.Parse(subs[p + 3]), Byte.Parse("135"));
                    }
                    else if (subs[p].Trim().Equals("prefix"))
                    {
                        prefix = subs[p + 1].Trim();
                    }
                    else if (subs[p].Trim().Equals("name"))
                    {
                        name = subs[p + 1].Trim();
                    }
                }
                
                
                nat.AddComponent<Nation>();


                nat.GetComponent<Nation>().InitCom(name, tag, color, color2, prefix);

                string key = tag;

                nations.Add(key, nat);
            }
        }

        foreach (GameObject nat in nations.Values)
        {
            Nation natFile = nat.GetComponent<Nation>();
            for (int i = 0; i < histFiles.Length; i++)
            {
                string[] nSubs = histFiles[i].Split('/', '.');
                string capital = "";

                if (File.Exists(histFiles[i]) && histFiles[i].Substring(histFiles[i].Length - 3, 3).Equals("txt") && nSubs[3][..3].Equals(natFile.nationTag.Trim()))
                {
                    string data = File.ReadAllText(histFiles[i]);
                    string[] subs = data.Split('=', '\n');

                    for (int j = 0; j < subs.Length; j++)
                    {
                        if (subs[j].Trim().Equals("capital"))
                        {
                            capital = subs[j+1].Trim();
                        }
                    }
                    natFile.InitHist(capital);
                }
            }
        }
    }

    #endregion

    void Update()
    {
        #region RayCasts
        //LEFT CLICK
        /*Left Click checks if the hit collider is null, if not, it will check to see if it contains either strings designated in the condtionals
             * If it contains '-', it will report it as a province, due to it being the static factor in all province names. 
             * If it contains'Unit' it will report it as a unit, due to it being the static factor in all unit names.
             * When it is determined that a province is selected, it will toggle false all highlights of other provinces, and then re-highlight the desired one
             * When it is determed that a unit is selected, it will toggle that unit as selected
             */
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                pu?.Hide();

                if (hit.collider.name.Contains("-"))
                {
                    foreach (GameObject p in provinces)
                    {
                        p.GetComponent<Province>().isHighlighted = false;
                    }
                    var province = hit.collider.gameObject;
                    Province pv = province.GetComponent<Province>();
                    pv.isHighlighted = true;
                    pu = canvas.transform.GetChild(1).GetComponent<ProvinceUI>();
                    ProvinceUI ps = (ProvinceUI)pu;
                    ps.Display(pv.provinceName, pv.terrain, pv.pop, Resources.Load<Sprite>("Flags/" + pv.ROOT_nation.nationTag), pv.ROOT_nation.nationName);
                }
                else if (hit.collider.name.Contains("Unit"))
                {
                    foreach (GameObject u in units)
                    {
                        if (u != null)
                        {
                            u.GetComponent<Unit>().isSelected = false;
                        }
                    }

                    selectedUnit = hit.collider.GetComponent<Unit>();
                    selectedUnit.isSelected = true;
                    pu = canvas.transform.GetChild(3).GetComponent<ArmyUI>();
                    ArmyUI au = (ArmyUI)pu;
                    au.Display("placeholder", Resources.Load<Sprite>("Sprites/Icons/Infantry"));
                }
            }
        }

        //RIGHT CLICK
        /* Right Click checks if the hit collider is null, if not, it will check to see if it contains either strings desingated in the conditionals
         * It will first check if it is a province, rather than a unit. Then it will check if the selectedUnit has a value, if it does, it will
         * nullify the nation command, and get a pathfind route for the unit to take. If it is valid, the unit is then designated to move.
         * */

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.name.Contains("-"))
                {
                    GameObject province = hit.collider.gameObject;
                    var provROOT = province.GetComponent<Province>();

                    if (selectedUnit != null && !selectedUnit.inCombat) 
                    {
                       selectedUnit.FindPath(provROOT);
                      
                    } else
                    {
                        //Bring up nation ledger
                    }
                }
            }
        }

        //How fast the camera moves
        #endregion
        #region Camera Input Movement 
        float moveAmount = Map(cameraFollow.GetComponent<camera>().zoom, cameraFollow.GetComponent<camera>().minZoom, cameraFollow.GetComponent<camera>().maxZoom, 200f, 350f);
        if (Input.GetKey(KeyCode.W))
        {
            cameraFollowPosition.y += moveAmount * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            cameraFollowPosition.y -= moveAmount * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            cameraFollowPosition.x -= moveAmount * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            cameraFollowPosition.x += moveAmount * Time.deltaTime;
        }

        float edgeSize = 5f;

        if (Input.mousePosition.x > Screen.width - edgeSize)
        {
            cameraFollowPosition.x += moveAmount * Time.deltaTime;
        }
        if (Input.mousePosition.x < edgeSize)
        {
            cameraFollowPosition.x -= moveAmount * Time.deltaTime;
        }
        if (Input.mousePosition.y > Screen.height - edgeSize)
        {
            cameraFollowPosition.y += moveAmount * Time.deltaTime;
        }
        if (Input.mousePosition.y < edgeSize)
        {
            cameraFollowPosition.y -= moveAmount * Time.deltaTime;
        }
        #endregion
    }
}

