using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    public TouchController touch_Controller;
    public GameCore game_Core;
    public GameObject troop_Model;
    public GameObject tile_AllowedModel;
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

    [ Range( 0.0f, 100.0f ) ]
    public float troops_RouteDifficulty = 25.0f;
    
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

    public void TestShowMovementAllowed() {
        Dictionary< Vector2Int, int > mappaTest = new Dictionary< Vector2Int, int >();
        
        mappaTest = ShowMovementAllowed( new Vector2Int( 50, 50 ), mappaTest, 1500 );
        //mappaTest = ShowMovementAllowed2( new Vector2Int( 50, 50 ), mappaTest, 200, Enum.Cardinal.NorthEast, true );
        //mappaTest = ShowMovementAllowed3( new Vector2Int( 50, 50 ), mappaTest, 100, Enum.Cardinal.NorthWest );

        GameObject tile_Start = Instantiate(tile_AllowedModel, new Vector3( 50 , 50, -0.5f), Quaternion.identity);
        tile_Start.GetComponent<Renderer>().material.color = Color.yellow;

        DestroyImmediate( GameObject.Find( "Test" ) );
        GameObject test = new GameObject( "Test" );

        foreach( Vector2Int tupla in mappaTest.Keys )
        {
            GameObject tile_Allowed = Instantiate( tile_AllowedModel, new Vector3( tupla.x , tupla.y, -0.5f ), Quaternion.identity, test.transform );

            float t = Vector3.Distance( tile_Allowed.transform.position, tile_Start.transform.position ) / 15;
            //float t = mappaTest[ tupla ] / 750;
            Debug.Log( ">>> t: " + t);
            Color tile_color = Vector4.Lerp( Color.yellow, Color.red, t);
            tile_color.a = 0.5f;
            tile_Allowed.GetComponent<Renderer>().material.color = tile_color;

        }
    }

    private Dictionary< Vector2Int, int > ShowMovementAllowed3( Vector2Int startingCoordinates, Dictionary< Vector2Int, int > allowedTilesMap, int remainedPA, Enum.Cardinal cardinalDirection )
    {
        int x = startingCoordinates.x, y = startingCoordinates.y; 

        int currentPA = 0;

        switch ( cardinalDirection ) {
            // Blocco Nord-Ovest
            case Enum.Cardinal.NorthWest:   Vector2Int pointNorthWest = new Vector2Int( x - 1, y + 1 );

                                            if( pointNorthWest.x >= 0 && pointNorthWest.y < game_Core.WorldYSize() ) {
                                                currentPA = remainedPA - game_Core.world_Matrix[ pointNorthWest.x, pointNorthWest.y ].tile_MovementRequired;

                                                if( currentPA >= 0 ) 
                                                {
                                                    if( allowedTilesMap.ContainsKey( pointNorthWest ) )
                                                    {
                                                        if( currentPA > allowedTilesMap[ pointNorthWest ] ) {
                                                            allowedTilesMap[ pointNorthWest ] = currentPA;
                                                        }
                                                    }
                                                    else 
                                                    {
                                                        allowedTilesMap.Add( pointNorthWest, currentPA );
                                                    }
                                                    

                                                    if( currentPA > 0 )
                                                    {
                                                        allowedTilesMap = ShowMovementAllowed3 ( pointNorthWest, allowedTilesMap, currentPA, Enum.Cardinal.NorthWest );
                                                    }
                                                }
                                            }

                                            //----//

                                            Vector2Int pointWest = new Vector2Int( x - 1, y );

                                            if( pointWest.x >= 0 ) {
                                                currentPA = remainedPA - game_Core.world_Matrix[ pointWest.x, pointWest.y ].tile_MovementRequired;

                                                if( currentPA >= 0 ) 
                                                {
                                                    if( allowedTilesMap.ContainsKey( pointWest ) )
                                                    {
                                                        if( currentPA > allowedTilesMap[ pointWest ] ) {
                                                            allowedTilesMap[ pointWest ] = currentPA;
                                                        }
                                                    }
                                                    else 
                                                    {
                                                        allowedTilesMap.Add( pointWest, currentPA );
                                                    }

                                                    if( currentPA > 0 )
                                                    {
                                                        allowedTilesMap = ShowMovementAllowed3 ( pointWest, allowedTilesMap, currentPA, Enum.Cardinal.NorthWest );
                                                    }
                                                }
                                            }

                                            //----//

                                            Vector2Int pointNorth = new Vector2Int( x, y + 1 );

                                            if( pointNorth.y < game_Core.WorldYSize() ) {
                                                currentPA = remainedPA - game_Core.world_Matrix[ pointNorth.x, pointNorth.y ].tile_MovementRequired;

                                                if( currentPA >= 0 ) 
                                                {
                                                    if( allowedTilesMap.ContainsKey( pointNorth ) )
                                                    {
                                                        if( currentPA > allowedTilesMap[ pointNorth ] ) {
                                                            allowedTilesMap[ pointNorth ] = currentPA;
                                                        }
                                                    }
                                                    else 
                                                    {
                                                        allowedTilesMap.Add( pointNorth, currentPA );
                                                    }

                                                    if( currentPA > 0 )
                                                    {
                                                        allowedTilesMap = ShowMovementAllowed3 ( pointNorth, allowedTilesMap, currentPA, Enum.Cardinal.NorthWest );
                                                    }
                                                }
                                            }

                                            break;
        }

        return allowedTilesMap;
    }

    private Dictionary< Vector2Int, int > ShowMovementAllowed2( Vector2Int startingCoordinates, Dictionary< Vector2Int, int > allowedTilesMap, int remainedPA, Enum.Cardinal cardinalDirection, bool isStarting )
    {
        int x = ( int )startingCoordinates.x, y = ( int )startingCoordinates.y, i = -1, j = -1; 

        Vector2Int currentCoordinates;
        int currentPA = 0;

        switch ( cardinalDirection ) {
            // Blocco Nord-Ovest
            case Enum.Cardinal.NorthWest:   if( isStarting ) 
                                            {
                                                i = x;
                                                isStarting = false;
                                            }
                                            else 
                                            {
                                                i = x - 1;
                                            }
                                            j = y + 1;

                                            if( i >= 0 && j < game_Core.WorldYSize() ) 
                                            {
                                                currentCoordinates = new Vector2Int( i, j );

                                                currentPA = remainedPA - game_Core.world_Matrix[ i, j ].tile_MovementRequired;

                                                if( currentPA >= 0) 
                                                {
                                                    allowedTilesMap.Add( currentCoordinates, currentPA );

                                                    if( currentPA > 0 )
                                                    {
                                                        allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.NorthWest, isStarting );
                                                        //allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.North, false );
                                                        //allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.West, false );
                                                    }
                                                }
                                            }

                                            break;

            // Blocco Ausiliario Nord
            case Enum.Cardinal.North:       i = x;
                                            j = y + 1;

                                            if( j < game_Core.WorldYSize() ) 
                                            {
                                                currentCoordinates = new Vector2Int( i, j );

                                                currentPA = remainedPA - game_Core.world_Matrix[ i, j ].tile_MovementRequired;

                                                if( currentPA >= 0) 
                                                {
                                                    allowedTilesMap.Add( currentCoordinates, currentPA );

                                                    if( currentPA > 0 )
                                                    {
                                                        allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.North, false );
                                                    }
                                                }
                                            }

                                            break;
            // Blocco Ausiliario Ovest
            case Enum.Cardinal.West:        i = x - 1;
                                            j = y;

                                            if( i >= 0 ) 
                                            {
                                                currentCoordinates = new Vector2Int( i, j );

                                                currentPA = remainedPA - game_Core.world_Matrix[ i, j ].tile_MovementRequired;

                                                if( currentPA >= 0) 
                                                {
                                                    allowedTilesMap.Add( currentCoordinates, currentPA );

                                                    if( currentPA > 0 )
                                                    {
                                                        allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.West, false );
                                                    }
                                                }
                                            }

                                            break;
            // Blocco Nord-Est
            case Enum.Cardinal.NorthEast:   if( isStarting ) 
                                            {
                                                j = y;
                                                isStarting = false;
                                            }
                                            else 
                                            {
                                                j = y + 1;
                                            }
                                            i = x + 1;

                                            if( i < game_Core.WorldXSize() && j < game_Core.WorldYSize() ) 
                                            {
                                                currentCoordinates = new Vector2Int( i, j );

                                                currentPA = remainedPA - game_Core.world_Matrix[ i, j ].tile_MovementRequired;

                                                if( currentPA >= 0) 
                                                {
                                                    allowedTilesMap.Add( currentCoordinates, currentPA );

                                                    if( currentPA > 0 )
                                                    {
                                                        allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.North, false );
                                                        allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.East, false );
                                                        allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.NorthEast, false );
                                                    }
                                                }
                                            }

                                            break;

            // Blocco Ausiliario East
            case Enum.Cardinal.East:        i = x + 1;
                                            j = y;

                                            if( i < game_Core.WorldXSize() ) 
                                            {
                                                currentCoordinates = new Vector2Int( i, j );

                                                currentPA = remainedPA - game_Core.world_Matrix[ i, j ].tile_MovementRequired;

                                                if( currentPA >= 0) 
                                                {
                                                    allowedTilesMap.Add( currentCoordinates, currentPA );

                                                    if( currentPA > 0 )
                                                    {
                                                        allowedTilesMap = ShowMovementAllowed2 ( currentCoordinates, allowedTilesMap, currentPA, Enum.Cardinal.East, false );
                                                    }
                                                }
                                            }

                                            break;
                                            
        }

        return allowedTilesMap;


    }

    private Dictionary< Vector2Int, int > ShowMovementAllowed( Vector2Int startingCoordinates, Dictionary< Vector2Int, int > allowedTilesMap, int actualMovementRemained )
    {
        int x = ( int )startingCoordinates.x, y = ( int )startingCoordinates.y, j = -1, i = -1;

        //*-*-*-*-*

        j = y + 1;
        if( j < game_Core.WorldYSize() )
        { 
        
            for( i = x; i <= x + 1; i++ )
            {
                if( i < game_Core.WorldXSize() )
                {
                    int currentTileMovementRemained = actualMovementRemained - game_Core.world_Matrix[ i, j ].tile_MovementRequired;
                    Enum.TileType currentTileType = game_Core.world_Matrix[ i, j ].tile_Type;

                    if( currentTileMovementRemained >= game_Core.tile_RoadMovementRequired )
                    {
                        Vector2Int coordinates = new Vector2Int( i, j );

                        if( allowedTilesMap.ContainsKey( coordinates )  ) 
                        {
                            int savedTileMovementRemained = allowedTilesMap[ coordinates ];

                            if( savedTileMovementRemained < currentTileMovementRemained ) 
                            {
                                allowedTilesMap[ coordinates ] = currentTileMovementRemained;

                                if( currentTileMovementRemained > game_Core.tile_RoadMovementRequired ){
                                    allowedTilesMap = ShowMovementAllowed( new Vector2Int( i, j ), allowedTilesMap, currentTileMovementRemained );
                                }
                            }
                            else 
                            {
                                currentTileMovementRemained = savedTileMovementRemained;
                            }
                        }
                        else 
                        {
                            if( currentTileType != Enum.TileType.River && currentTileType != Enum.TileType.City && currentTileType != Enum.TileType.Lake )
                            {
                                allowedTilesMap.Add( coordinates, currentTileMovementRemained );
                            }

                            if( currentTileMovementRemained > game_Core.tile_RoadMovementRequired )
                            { 
                                allowedTilesMap = ShowMovementAllowed( new Vector2Int( i, j ), allowedTilesMap, currentTileMovementRemained );
                            }
                        }
                    }
                }
            }
        }

        //*-*-*-*-*
        j = y - 1;
        if( j >= 0 )
        {
            for( i = x; i >= x - 1; i-- )
            {
                if( i >= 0 )
                {
                    int currentTileMovementRemained = actualMovementRemained - game_Core.world_Matrix[ i, j ].tile_MovementRequired;
                    Enum.TileType currentTileType = game_Core.world_Matrix[ i, j ].tile_Type;

                    if( currentTileMovementRemained >= game_Core.tile_RoadMovementRequired )
                    {
                        Vector2Int coordinates = new Vector2Int( i, j );

                        if( allowedTilesMap.ContainsKey( coordinates )  ) 
                        {
                            int savedTileMovementRemained = allowedTilesMap[ coordinates ];

                            if( savedTileMovementRemained < currentTileMovementRemained ) 
                            {
                                allowedTilesMap[ coordinates ] = currentTileMovementRemained;

                                if( currentTileMovementRemained > game_Core.tile_RoadMovementRequired ){
                                    allowedTilesMap = ShowMovementAllowed( new Vector2Int( i, j ), allowedTilesMap, currentTileMovementRemained );
                                }
                            }
                            else 
                            {
                                currentTileMovementRemained = savedTileMovementRemained;
                            }
                        }
                        else 
                        {
                            if( currentTileType != Enum.TileType.River && currentTileType != Enum.TileType.City && currentTileType != Enum.TileType.Lake )
                            {
                                allowedTilesMap.Add( coordinates, currentTileMovementRemained );
                            }

                            if( currentTileMovementRemained > game_Core.tile_RoadMovementRequired )
                            { 
                                allowedTilesMap = ShowMovementAllowed( new Vector2Int( i, j ), allowedTilesMap, currentTileMovementRemained );
                            }
                        }
                    }
                }
            }
        }

        //*-*-*-*-* 

        i = x + 1;
        if( i < game_Core.WorldXSize() )
        {

            for( j = y; j <= y + 1; j++ )
            {
                if( j < game_Core.WorldYSize() )
                {
                    int currentTileMovementRemained = actualMovementRemained - game_Core.world_Matrix[ i, j ].tile_MovementRequired;
                    Enum.TileType currentTileType = game_Core.world_Matrix[ i, j ].tile_Type;

                    if( currentTileMovementRemained >= game_Core.tile_RoadMovementRequired )
                    {
                        Vector2Int coordinates = new Vector2Int( i, j );

                        if( allowedTilesMap.ContainsKey( coordinates )  ) 
                        {
                            int savedTileMovementRemained = allowedTilesMap[ coordinates ];

                            if( savedTileMovementRemained < currentTileMovementRemained ) 
                            {
                                allowedTilesMap[ coordinates ] = currentTileMovementRemained;

                                if( currentTileMovementRemained > game_Core.tile_RoadMovementRequired ){
                                    allowedTilesMap = ShowMovementAllowed( new Vector2Int( i, j ), allowedTilesMap, currentTileMovementRemained );
                                }
                            }
                            else 
                            {
                                currentTileMovementRemained = savedTileMovementRemained;
                            }
                        }
                        else 
                        {
                            if( currentTileType != Enum.TileType.River && currentTileType != Enum.TileType.City && currentTileType != Enum.TileType.Lake )
                            {
                                allowedTilesMap.Add( coordinates, currentTileMovementRemained );
                            }

                            if( currentTileMovementRemained > game_Core.tile_RoadMovementRequired )
                            { 
                                allowedTilesMap = ShowMovementAllowed( new Vector2Int( i, j ), allowedTilesMap, currentTileMovementRemained );
                            }
                        }
                    }
                }
            }
        }

        //*-*-*-*-* 

        i = x - 1;
        if( i >= 0 )
        {

            for( j = y; j >= y - 1; j-- )
            {
                if( j < game_Core.WorldYSize() )
                {
                    int currentTileMovementRemained = actualMovementRemained - game_Core.world_Matrix[ i, j ].tile_MovementRequired;
                    Enum.TileType currentTileType = game_Core.world_Matrix[ i, j ].tile_Type;

                    if( currentTileMovementRemained >= game_Core.tile_RoadMovementRequired )
                    {
                        Vector2Int coordinates = new Vector2Int( i, j );

                        if( allowedTilesMap.ContainsKey( coordinates )  ) 
                        {
                            int savedTileMovementRemained = allowedTilesMap[ coordinates ];

                            if( savedTileMovementRemained < currentTileMovementRemained ) 
                            {
                                allowedTilesMap[ coordinates ] = currentTileMovementRemained;

                                if( currentTileMovementRemained > game_Core.tile_RoadMovementRequired ){
                                    allowedTilesMap = ShowMovementAllowed( new Vector2Int( i, j ), allowedTilesMap, currentTileMovementRemained );
                                }
                            }
                            else 
                            {
                                currentTileMovementRemained = savedTileMovementRemained;
                            }
                        }
                        else 
                        {
                            if( currentTileType != Enum.TileType.River && currentTileType != Enum.TileType.City && currentTileType != Enum.TileType.Lake )
                            {
                                allowedTilesMap.Add( coordinates, currentTileMovementRemained );
                            }

                            if( currentTileMovementRemained > game_Core.tile_RoadMovementRequired )
                            { 
                                allowedTilesMap = ShowMovementAllowed( new Vector2Int( i, j ), allowedTilesMap, currentTileMovementRemained );
                            }
                        }
                    }
                }
            }
        }

        //*-*-*-*-* 

        Debug.Log( "allowedTilesMap size: " + allowedTilesMap.Count );

        return allowedTilesMap;
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
        public int tile_X, tile_Y;

        public int tile_MaxMovementRemained { get; set; }

        public AllowedTile(int coordinate_X, int coordinate_Y ) // costruttore
        {
            tile_X = coordinate_X;
            tile_Y = coordinate_Y;
        }
    }   
}
