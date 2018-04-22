using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour {

    public struct Cell
    {
        public int x, z;
        public GameObject WallXPlus;
        public GameObject WallXMinus;//no rotation
        public GameObject WallZPlus;
        public GameObject WallZMinus;
    }
    public GameObject WallPrefab;
    public GameObject CornerOutPrefab;
    public GameObject CornerInPrefab;
    public GameObject CornerPrefab;
    public int SizeX = 5, SizeZ = 5;
   

    
   public bool MakeWall=false;
    public bool ShowInputs;
    
    private Cell[,] CellGrid;
    public GameObject Level;
    private GameObject selectedObj;
    private GameObject Corners;

    // Use this for initialization
    void Start () {
      

    }
	
	// Update is called once per frame
	void Update () {
        try
        {
            if(selectedObj != Selection.activeGameObject)
            {
              
                selectedObj = Selection.activeGameObject;
                Debug.Log(selectedObj.name);
                if (selectedObj.name.Contains("Wall"))
                {
                    selectedObj.SetActive(MakeWall);
                }
               
            }
          
        }
        catch
        {

        }
       
    }
    public void ClearCorners()
    {
        foreach (Transform c in Corners.transform)
        {
            if(c.GetChild(0).gameObject.activeSelf)
            GameObject.DestroyImmediate(c.gameObject);
        }
    }
    public void ActiveCorners()
    {
        foreach (Transform c in Corners.transform)
        {
            c.GetChild(0).gameObject.SetActive(true);
               
        }
    }
    public void Finish()
    {
        GameObject level = GameObject.Find("Level");
        foreach(Transform c in level.transform)
        {
            if (!c.gameObject.activeSelf)
                GameObject.DestroyImmediate(c.gameObject);
        }
        MakeWall = true;
        
        // DestroyImmediate(gameObject);
    }
    public void Undo()
    {
        selectedObj.SetActive(!MakeWall);
    }
    public void Rebuild()
    {
        Debug.Log(Level.transform.childCount);
        string end = Level.transform.GetChild(Level.transform.childCount-1).name;
       
        int.TryParse(end.Substring(5, end.IndexOf(",") - end.IndexOf(":") - 1), out SizeX);
        int.TryParse(end.Substring(end.IndexOf(",") + 1, end.Length - 3 - end.IndexOf(",")),out SizeZ);
        SizeX++;
            SizeZ++;
         CellGrid = new Cell[SizeX, SizeZ];
        for (int x = 0; x < SizeX; x++)
        {
            for (int z = 0; z < SizeZ; z++)
            {
                
                CellGrid[x,z] = new Cell();
                CellGrid[x, z].x = x;
                CellGrid[x, z].z = z;
            }
        }
        for(int i=0;i<Level.transform.childCount;i++)
        {
            GameObject temp =Level.transform.GetChild(i).gameObject;
            int x, z;
            end = temp.name;
            int.TryParse(end.Substring(5, end.IndexOf(",") - end.IndexOf(":") - 1), out x);
            int.TryParse(end.Substring(end.IndexOf(",") + 1, end.Length - 3 - end.IndexOf(",")), out z);
            Cell cell;
            
            try
            {
                cell = CellGrid[x, z];
            }
            catch
            {
                Debug.Log(x+","+ z);
            }
          
            if (temp.name.Contains("X-"))
            {
                cell.WallXMinus = temp;
            }
            else if (temp.name.Contains("X+"))
            {
                cell.WallXPlus = temp;
            }
            else if (temp.name.Contains("Z-"))
            {
                cell.WallZMinus = temp;
            }
            else if (temp.name.Contains("Z+"))
            {
                cell.WallZPlus = temp;
            }

        }


               
        


    }
    //x- z- defualt
    public void Cornerfy()
    {
        GameObject stay = new GameObject();
        stay.name = "CornerStay";
        Corners = new GameObject();
        Corners.name = "Corners";
        for (int x = 0; x < SizeX; x++)
        {
            for (int z = 0; z < SizeZ; z++)
            {

                GameObject temp = PrefabUtility.InstantiatePrefab(CornerPrefab) as GameObject;
                temp.name = "CornerPre:" + x + "," + z + " X-";
                temp.transform.position = new Vector3(x * 2, 0, z * 2);
                temp.transform.parent = Corners.transform;
                temp.GetComponent<CornerButton>().Stay = stay;



                temp = PrefabUtility.InstantiatePrefab(CornerPrefab) as GameObject;
                temp.name = "CornerPre:" + x + "," + z + " Z+";
                temp.transform.position = new Vector3(x * 2, 0, z * 2);
                temp.transform.Rotate(new Vector3(0, 90, 0));
                temp.transform.parent = Corners.transform;
                temp.GetComponent<CornerButton>().Stay = stay;

                temp = PrefabUtility.InstantiatePrefab(CornerPrefab) as GameObject;
                temp.name = "CornerPre:" + x + "," + z + " X+";
                temp.transform.position = new Vector3(x * 2, 0, z * 2);
                temp.transform.Rotate(new Vector3(0, 180, 0));
                temp.transform.parent = Corners.transform;
                temp.GetComponent<CornerButton>().Stay = stay;

                temp = PrefabUtility.InstantiatePrefab(CornerPrefab) as GameObject;
                temp.name = "CornerPre:" + x + "," + z + " Z-";
                temp.transform.position = new Vector3(x * 2, 0, z * 2);
                temp.transform.Rotate(new Vector3(0, -90, 0));
                temp.transform.parent = Corners.transform;
                temp.GetComponent<CornerButton>().Stay = stay;
            }
               

        }
    }
    
    public void BuildGrid()
    {
        

      
        Level = new GameObject();
        Level.name = "Level";
        CellGrid = new Cell[SizeX, SizeZ];
        for (int x=0;x<SizeX;x++)
        {
            for(int z=0;z<SizeZ;z++)
            {
                //GameObject box = Instantiate(BoxPrefab);
                //box.name = "Box:" + x + "," + z;
                //box.transform.position = new Vector3(x * 2,3, z * 2);
                //box.AddComponent<LevelEditorBox>();
                //box.GetComponent<LevelEditorBox>().X=x;
                //box.GetComponent<LevelEditorBox>().Z = z;
                //box.transform.parent = BoxHolder.transform;
                //EditorGrid[x, z] = box;

                Cell cell = new Cell();
                GameObject temp = PrefabUtility.InstantiatePrefab(WallPrefab) as GameObject;
                temp.name = "Wall:" + x + "," + z + " X-";
                temp.transform.position = new Vector3(x * 2, 0, z * 2);
                temp.transform.parent = Level.transform;
                cell.WallXMinus = temp;

                temp = PrefabUtility.InstantiatePrefab(WallPrefab) as GameObject;
                temp.name = "Wall:" + x + "," + z + " Z+";
                temp.transform.position = new Vector3(x * 2, 0, z * 2);
                temp.transform.Rotate(new Vector3(0,90,  0));
                temp.transform.parent = Level.transform;
                cell.WallZPlus = temp;

                temp = PrefabUtility.InstantiatePrefab(WallPrefab) as GameObject;
                temp.name = "Wall:" + x + "," + z + " X+";
                temp.transform.position = new Vector3(x * 2, 0, z * 2);
                temp.transform.Rotate(new Vector3(0,180, 0));
                temp.transform.parent = Level.transform;
                cell.WallXPlus = temp;

                temp = PrefabUtility.InstantiatePrefab(WallPrefab) as GameObject;
                temp.name = "Wall:" + x + "," + z + " Z-";
                temp.transform.position = new Vector3(x * 2, 0, z * 2);
                temp.transform.Rotate(new Vector3(0,-90, 0));
                temp.transform.parent = Level.transform;
                cell.WallZMinus = temp;

                CellGrid[x, z] = cell;
                CellGrid[x, z].x = x;
                CellGrid[x, z].z = z;

            }
        }
    }
   
   
   
}
#endif