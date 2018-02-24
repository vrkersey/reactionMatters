using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour {

    public struct Cell
    {
        public GameObject WallXPlus;
        public GameObject WallXMinus;//no rotation
        public GameObject WallZPlus;
        public GameObject WallZMinus;
    }
    public GameObject WallPrefab;
    public GameObject BoxPrefab;
    public Material NotSelected;
    public Material Selected;
    public int SizeX = 5, SizeZ = 5;
   

    public bool MakeWall=false;
    public bool EditLevel = true;
   
    public bool ShowInputs;


    private GameObject[,] EditorGrid;
    private Cell[,] CellGrid;
    private GameObject BoxHolder;
    private GameObject Level;

    private int LastX, LastZ, CurrentX, CurrentZ;
    // Use this for initialization
    void Start () {
      

    }
	
	// Update is called once per frame
	void Update () {
        try
        {

           GameObject selectedObj= Selection.activeGameObject.transform.parent.gameObject;
            Debug.Log(selectedObj.name);
            if(selectedObj.name.Contains("Wall"))
            {
                selectedObj.SetActive(MakeWall);
            }
        }
        catch
        {

        }
       
    }
    public void BuildGrid()
    {
        CurrentZ = -1;
        CurrentX = -1;
        LastX = -1;
        LastZ = -1;

        BoxHolder = transform.GetChild(0).gameObject;
        Level = new GameObject();
        Level.name = "Level";
        EditorGrid = new GameObject[SizeX, SizeZ];
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

            }
        }
    }
    public void ClearSelectedCells()
    {
        EditorGrid[CurrentX, CurrentZ].GetComponent<Renderer>().material = NotSelected;
        CurrentZ = -1;
        CurrentX = -1;
        LastX = -1;
        LastZ = -1;
    }
    public void MoveToCell(int x,int z)
    {
        if(x==CurrentX&&CurrentZ==z)
        {
            return;
        }
        Debug.Log("select"+x+" "+CurrentX+ " "+LastX);
       // EditorGrid[x, z].GetComponent<Renderer>().material = Selected;
        LastX = CurrentX;
        LastZ = CurrentZ;
        CurrentX = x;
        CurrentZ = z;
        if(LastX!=-1&&CurrentX!=-1)
        {
            //   EditorGrid[LastX, LastZ].GetComponent<Renderer>().material = NotSelected;
            Debug.Log("add");
            AddRemoveWall();
        }
    }
    private void AddRemoveWall()
    {
        Debug.Log(MakeWall);
        if(CurrentX+1==LastX)
        {
            Debug.Log("x+");
            CellGrid[CurrentX, CurrentZ].WallXMinus.SetActive(MakeWall);
            CellGrid[LastX, LastZ].WallXPlus.SetActive(MakeWall);
        }
        else if(CurrentX - 1 == LastX)
        {
            Debug.Log("x-");
            CellGrid[CurrentX, CurrentZ].WallXPlus.SetActive(MakeWall);
            CellGrid[LastX, LastZ].WallXMinus.SetActive(MakeWall);
        }
        else if (CurrentZ + 1 == LastZ )
        {
            Debug.Log("z+");
            CellGrid[CurrentX, CurrentZ].WallZMinus.SetActive(MakeWall);
            CellGrid[LastX, LastZ].WallZPlus.SetActive(MakeWall);
        }
        else if (CurrentZ - 1 == LastZ)
        {
            Debug.Log("z-");
            CellGrid[CurrentX, CurrentZ].WallZMinus.SetActive(MakeWall);
            CellGrid[LastX, LastZ].WallZPlus.SetActive(MakeWall);
        }
    }
}
