using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Node:MonoBehaviour
{
	[HideInInspector]
	public UnityEvent Changed = new UnityEvent();
	public float Mod = 1;
	public GameObject Direction;
    public Vector3 GetPosition()
	{
		return transform.position*Mod;
	}
	public Vector3 GetDirection()
	{
		return Direction.transform.position*Mod;
	}
	private void OnValidate()
	{
		if (Changed != null)
			Changed.Invoke();
	}
	public void CreateDir ()
	{
		
		Direction = new GameObject("Dir");
		Direction.transform.parent = transform;
	}
	private void Update()
	{
		if(transform.hasChanged||Direction.transform.hasChanged)
		{
			transform.hasChanged = false;
			Direction.transform.hasChanged = false;
			Debug.Log(gameObject.name + " has moved");
			if (Changed != null)
				Changed.Invoke();
		}
	}
	public void SetDirToParent()
	{
		Direction.transform.position = transform.parent.position;
	}
}
public struct CurvePoint
{
    public Vector3 Position, Normal;

    public CurvePoint(Vector3 position, Vector3 normal)
    {
        Position = position;
        Normal = normal;
    }
}


[Serializable]
[ExecuteInEditMode]
public class BezierCurve : MonoBehaviour {

    
   [SerializeField]
    private int STEP_COUNT = 1000;
    private float T_STEP;

    public float Length;
	[SerializeField]
	public Node StartNode, EndNode;
    public Vector3[] Points;
    private float[] PointLengths;

    public UnityEvent Changed = new UnityEvent();

    private void Update()
    {
        
    }
    public void CreateBezierCurve(Node startNode,Node endNode)
    {
      
        StartNode = startNode;
        EndNode = endNode;

    }
    private Node CreateNode(string name)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.parent = transform;
        sphere.transform.localPosition = Vector3.zero;
		Node n =sphere.AddComponent<Node>();
		n.CreateDir();
		n.Changed.AddListener(() => GenertatePoints());

		return n;
    }
    private void OnDrawGizmos()
    {
		try
		{
			Gizmos.color = Color.blue;
			foreach (Vector3 pos in Points)
			{
				Gizmos.DrawSphere(pos, .1f);
			}
		}
		catch
		{
			GenertatePoints();
		}
       
        
    }
	private void Start()
	{
		if(StartNode==null)		StartNode=CreateNode("Start");
		if(EndNode==null)		EndNode=CreateNode("End");
		GenertatePoints();
	}
	private void OnEnable()
	{
		
	}
	private void OnDisable()
	{
		
	}
	//public CurvePoint[][] GetDeformPoints()
	//{
	//    CurvePoint[][] CurvePoints = new CurvePoint[Walls][];
	//    float step = LengthPerWall / 10;
	//    int wall = 0;
	//    int i = 0;
	//    bool nextwall = true;
	//    for(int p =0;p<Points.Length;p++)
	//    {
	//        if(nextwall)
	//        {
	//            i = 0;

	//            nextwall = false;
	//            CurvePoints[wall] = new CurvePoint[11];
	//            CurvePoints[wall][i++] = new CurvePoint(Points[p], Tangents[p]);
	//        }
	//        else
	//        {
	//            if(PointLengths[p]>(i*step+LengthPerWall*wall))
	//            {
	//                if(Mathf.Abs(PointLengths[p]-step*i)> Mathf.Abs(PointLengths[p-1] - step * i))
	//                {
	//                    CurvePoints[wall][i++] = new CurvePoint(Points[p-1], Tangents[p-1]);
	//                }
	//                else
	//                {
	//                    CurvePoints[wall][i++] = new CurvePoint(Points[p], Tangents[p]);
	//                }
	//                if(i>=11)
	//                {
	//                    nextwall = true;
	//                    wall++;
	//                }
	//            }
	//            else if(p>=Points.Length-1)
	//            {
	//                CurvePoints[wall][i++] = new CurvePoint(Points[p], Tangents[p]);
	//            }
	//        }
	//    }
	//    return CurvePoints;


	//}
	public void GenertatePoints()
    {
		if (StartNode == null)
		{
			StartNode = transform.GetChild(0).GetComponent<Node>();
		}
		if (EndNode == null)
		{
			EndNode = transform.GetChild(1).GetComponent<Node>();
		}

		Debug.Log("pointt gen start");
        Length = 0;
        T_STEP = 1.0f / STEP_COUNT;
        Points = new Vector3[STEP_COUNT];
        PointLengths = new float[STEP_COUNT];
        float t =0;
        for(int i = 0;i<STEP_COUNT;i++)
        {
            Points[i] = GetPointPosition(t);
            t += T_STEP;
            if(i>0)
            {
                Length += Vector3.Distance(Points[i], Points[i - 1]);
                PointLengths[i] = Length;
            }
        }
		if (Changed != null)
			Changed.Invoke();
	}
    public Vector3 GetPointPosition(float t)
    {
		//if (t > 1) t = t / Length;
		if (t < 0 || t > 1)
            throw new ArgumentException("Time must be between 0 and 1. Given time was " + t);
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return
           StartNode.GetPosition()* (omt2 * omt) +
           GetInverseDirection(StartNode) * (3f * omt2 * t) +
           GetInverseDirection(EndNode) * (3f * omt * t2) +
           EndNode.GetPosition() * (t2 * t);
    }
    public Vector3 GetPointTangent(float t)
    {
	//	if (t > 1) t = t / Length;
		if (t < 0 || t > 1)
            throw new ArgumentException("Time must be between 0 and 1. Given time was " + t);
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent =
            StartNode.GetPosition() * (-omt2) +
            GetInverseDirection(StartNode) * (3 * omt2 - 2 * omt) +
            GetInverseDirection(EndNode) * (-3 * t2 + 2 * t) +
            EndNode.GetPosition() * (t2);
        return tangent.normalized;
    }
    public Vector3 GetInverseDirection(Node node)
    {
        return ((2 * node.GetPosition())) - ( node.GetDirection());
    }
}
