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
    public int SizeX = 5, SizeZ = 5;
   

    public bool MakeWall=false;
   
    public bool ShowInputs;
    
    private Cell[,] CellGrid;
    private GameObject Level;
    private GameObject selectedObj;
    // Use this for initialization
    void Start () {
      

    }
	
	// Update is called once per frame
	void Update () {
        try
        {
            if(selectedObj != Selection.activeGameObject.transform.parent.gameObject)
            {
                selectedObj = Selection.activeGameObject.transform.parent.gameObject;
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
    public void Finish()
    {
        GameObject level = GameObject.Find("Level");
        foreach(Transform c in level.transform)
        {
            if (!c.gameObject.activeSelf)
                GameObject.DestroyImmediate(c.gameObject);
        }
        DestroyImmediate(gameObject);
    }
    public void Undo()
    {
        selectedObj.SetActive(!MakeWall);
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

            }
        }
    }
   
   
   
}
