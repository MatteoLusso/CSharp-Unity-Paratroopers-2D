using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    public TouchController touch_Controller;
    public GameCore game_Core;
    public GameObject troop_Model;
    public int troops_StartingNumber;

    public int troops_PlayersNumber;

    public float touch_Radius;

    public float UI_Distance;
    public float smooth_UIMovement;
    public float smooth_UIRotation;
    public float smooth_UIScaling;

    public float UI_MinScale;
    public float UI_MaxScale;

    public float UI_IconCorrectionX;
    public float UI_IconCorrectionY;
    
    private List<Troop> troops_List = new List<Troop>();

    void LateUpdate()
    {
        UpdateTroopInfo();
    }

    public void ChangeColor(Touch touch_Info)
    {
        Ray touch_Ray = Camera.main.ScreenPointToRay(touch_Info.position);

        RaycastHit[] touch_Hitted = Physics.SphereCastAll(touch_Ray, touch_Radius, Mathf.Infinity);

        foreach(RaycastHit hit_Object in touch_Hitted)
        {
            foreach(Troop troop_Info in troops_List)
            {
                if(troop_Info.gameObjectInstance == hit_Object.collider.gameObject)
                {
                    touch_Controller.AutoReachPosition(new Vector3(hit_Object.collider.gameObject.transform.position.x, hit_Object.collider.gameObject.transform.position.y, Camera.main.transform.position.z));

                    GameObject troop_UI = troop_Info.gameObjectInstance.transform.GetChild(0).GetChild(0).gameObject;

                    if(troop_Info.highlighted)
                    {
                        hit_Object.collider.gameObject.GetComponent<Renderer>().material.color = troop_Info.startingColor;

                        troop_UI.SetActive(false);

                        troop_Info.highlighted = false;
                    }
                    else
                    {
                        hit_Object.collider.gameObject.GetComponent<Renderer>().material.color = new Color (0.5f, 0.0f , 0.0f, 1.0f);

                        troop_UI.SetActive(true);
                        troop_UI.GetComponentInChildren<Text>().text = "Attacco: " + troop_Info.attack + "\nDifesa: " + troop_Info.defense + "\nVelocità: " + troop_Info.speed;

                        troop_Info.highlighted = true;
                    }
                }
            }
        }
    }

    private void ShowMovementAllowed(){
        
    }

    private void UpdateTroopInfo()
    {
        float UI_MinX = UI_IconCorrectionX;
        float UI_MaxX = Screen.width - UI_IconCorrectionX;
        float UI_MinY = UI_IconCorrectionY;
        float UI_MaxY = Screen.height - UI_IconCorrectionY;

        float angle_Alpha = (Mathf.Atan2(Screen.height, Screen.width) * Mathf.Rad2Deg);
        float angle_Gamma = Vector3.SignedAngle(Vector3.right, Camera.main.transform.right, Vector3.forward);

        //Debug.Log("Alpha: " + angle_Alpha);
        //Debug.Log("Gamma: " + angle_Gamma);

        foreach(Troop troop_Info in troops_List)
        {

            GameObject troop_Stats = troop_Info.gameObjectInstance.transform.GetChild(0).GetChild(0).gameObject;
            GameObject troop_Icon = troop_Info.gameObjectInstance.transform.GetChild(1).GetChild(0).gameObject;
            GameObject troop_Object = troop_Info.gameObjectInstance;

            Debug.DrawLine(new Vector3(troop_Object.transform.position.x, troop_Object.transform.position.y, 0.0f), new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f), Color.red);

            if(troop_Stats.activeSelf)
            {
                Vector3 UI_Position = troop_Info.gameObjectInstance.transform.position;

                Vector3 UI_Direction = (Camera.main.transform.position - UI_Position).normalized * UI_Distance;
                UI_Direction = new Vector3(UI_Direction.x, UI_Direction.y, 0.0f);

                UI_Position += UI_Direction;

                troop_Stats.transform.position = Vector3.Lerp(troop_Stats.transform.position, UI_Position, smooth_UIMovement * Time.deltaTime);
                troop_Stats.transform.rotation = Quaternion.Lerp(troop_Stats.transform.rotation, Camera.main.transform.rotation, smooth_UIRotation * Time.deltaTime);

                float UI_Scale = UI_MinScale + ((UI_MaxScale - UI_MinScale) / (touch_Controller.zoom_OutExtendedMax - touch_Controller.zoom_OutExtendedMin) * (Camera.main.orthographicSize - touch_Controller.zoom_OutExtendedMin));
                troop_Stats.transform.localScale = Vector3.Lerp(troop_Stats.transform.localScale, new Vector3(UI_Scale, UI_Scale, troop_Stats.transform.localScale.z), smooth_UIScaling * Time.deltaTime);
            }
        }
    }

    public void AddTroops()
    {
        if(troops_List != null)
        {
            troops_List = new List<Troop>();
        }

        for(int player = 1; player <= troops_PlayersNumber; player++)
        {
            for(int i = 0; i < troops_StartingNumber; i++)
            {

                int new_X = Random.Range(0, game_Core.WorldXSize());
                int new_Y = Random.Range(0, game_Core.WorldYSize());

                if(game_Core.TileType(new_X, new_Y) != Enum.TileType.Lake)
                {
                    if(troops_List != null)
                    {
                        foreach(Troop troop_Deployed in troops_List)
                        {
                            if(troop_Deployed.coordX == new_X && troop_Deployed.coordY == new_Y)
                            {
                                Debug.Log("Troop not added");
                                goto TroopNotGenerated;
                            }
                        }
                    }

                    troops_List.Add(new Troop(new_X, new_Y, (Enum.Player)player));

                    TroopNotGenerated:

                    ;

                }
            }
        }
    }

    public void DrawTroops()
    {
        for(int player = 1; player <= troops_PlayersNumber; player++)
        {
            GameObject level_Troops = new GameObject("TroopsP" + player);
            level_Troops.transform.parent = GameObject.Find("Level").transform;

            int troop_Number = 0;

            foreach(Troop troop_Tile in troops_List)
            {
                troop_Number++;
                if(troop_Tile.player == (Enum.Player)player)
                {
                    troop_Tile.gameObjectInstance = Instantiate(troop_Model, new Vector3(troop_Tile.coordX , troop_Tile.coordY, -0.5f), Quaternion.identity, level_Troops.transform);
                    troop_Tile.gameObjectInstance.name = "P" + player + " | " +  troop_Number;
                    troop_Tile.startingColor = troop_Tile.gameObjectInstance.gameObject.GetComponent<Renderer>().material.color;
                }
            }
        }
    }

    public class Troop
    {
        public GameObject gameObjectInstance = null;

        public int coordX = -1;
        public int coordY = -1;

        public bool highlighted = false;

        public Color startingColor = Vector4.zero;

        public Enum.Player player = Enum.Player.Empty;

        public int defense = 10;
        public int attack = 5;
        public int speed = 15;
        public int maxMovement = 20;

        public List<AllowedTile> allowedTiles = new List<AllowedTile>();

        public Troop(int inputX, int inputY, Enum.Player inputPlayer)
        {
            coordX = inputX;
            coordY = inputY;

            player = inputPlayer;
        }

    }

    public class AllowedTile
    {
        int tile_X, tile_Y;
        public AllowedTile(int coordinate_X, int coordinate_Y) // costruttore
        {
            tile_X = coordinate_X;
            tile_Y = coordinate_Y;
        }
    }   
}
