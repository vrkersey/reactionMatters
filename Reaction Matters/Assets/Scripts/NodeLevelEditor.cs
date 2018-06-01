using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NodeLevelEditor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
public struct Hallway
{
    public Node node1;
    public Node node2;
    public int Type;
    public int WidthType;//0 is 2m narrow and duct
}
public struct Room
{
    public LinkedList<RoomSection> RoomSections;
}
public struct RoomSection
{
    public LinkedList<Wall> Walls;
    public LinkedList<Node> Corners;
    
   
}
public struct Wall
{
    public Node node1;
    public Node node2;
    public int Type;//-1 is just floor

}
public struct Node
{
    public Transform transform;
    public LinkedList<Node> ConnetedNodes;
    public Node(Transform _transform)
    {
        ConnetedNodes = new LinkedList<Node>();
        transform = _transform;
    }
    public bool AddNode(Node node)
    {

        if (ConnetedNodes.Count >= 4)
        {
            ConnetedNodes.AddLast(node);
            return true;
        }

        return false;
    }
}
