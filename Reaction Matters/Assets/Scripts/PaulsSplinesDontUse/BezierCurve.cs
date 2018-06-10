using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct Node
{
    public GameObject Position, Direction;
    public Node(string Name)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = Name+" ControlPoint";
        
        Position = sphere;
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = Name+" DirectionControlPoint";
        Direction = sphere;
    }
    public bool HasMoved()
    {
        if(Position.transform.hasChanged|| Direction.transform.hasChanged)
        {
            Position.transform.hasChanged = false;
            Direction.transform.hasChanged = false;
            return true;
        }
        return false;
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
    private int STEP_COUNT = 2000;
    private float T_STEP;

    public float Length;
    public float LengthPerWall;
    public int Walls;
    public float Mod1 = 1, Mod2 = 1;
    private float mod1 = 1, mod2 = 1;
    public Node StartNode, EndNode;
    public Vector3[] Points;
    public Vector3[] Tangents;
    private float[] PointLengths;

    public UnityEvent Changed = new UnityEvent();

    private void Update()
    {
        if(EndNode.HasMoved()||StartNode.HasMoved()||Mod1!=mod1||mod2!=Mod2)
        {
            mod1 = Mod1;
            mod2 = Mod2;

            GenertatePoints();
            Changed.Invoke();
        }
    }
    public void CreateBezierCurve(Node startNode,Node endNode)
    {
      
        StartNode = startNode;
        EndNode = endNode;

    }
    private GameObject CreateNode(string name)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.parent = transform;
        sphere.transform.localPosition = Vector3.zero;
        return sphere;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach(Vector3 pos in Points)
        {
            Gizmos.DrawSphere(pos, .1f);
        }
        
    }
    public CurvePoint[][] GetDeformPoints()
    {
        CurvePoint[][] CurvePoints = new CurvePoint[Walls][];
        float step = LengthPerWall / 10;
        int wall = 0;
        int i = 0;
        bool nextwall = true;
        for(int p =0;p<Points.Length;p++)
        {
            if(nextwall)
            {
                i = 0;
                
                nextwall = false;
                CurvePoints[wall] = new CurvePoint[11];
                CurvePoints[wall][i++] = new CurvePoint(Points[p], Tangents[p]);
            }
            else
            {
                if(PointLengths[p]>(i*step+LengthPerWall*wall))
                {
                    if(Mathf.Abs(PointLengths[p]-step*i)> Mathf.Abs(PointLengths[p-1] - step * i))
                    {
                        CurvePoints[wall][i++] = new CurvePoint(Points[p-1], Tangents[p-1]);
                    }
                    else
                    {
                        CurvePoints[wall][i++] = new CurvePoint(Points[p], Tangents[p]);
                    }
                    if(i>=11)
                    {
                        nextwall = true;
                        wall++;
                    }
                }
                else if(p>=Points.Length-1)
                {
                    CurvePoints[wall][i++] = new CurvePoint(Points[p], Tangents[p]);
                }
            }
        }
        return CurvePoints;
            
        
    }
    public void GenertatePoints()
    {
        
        Length = 0;
        T_STEP = 1.0f / STEP_COUNT;
        Points = new Vector3[STEP_COUNT];
        Tangents = new Vector3[STEP_COUNT];
        PointLengths = new float[STEP_COUNT];
        float t =0;
        for(int i = 0;i<STEP_COUNT;i++)
        {
            Points[i] = GetPointPosition(t);
            Tangents[i] = GetPointTangent(t);
            t += T_STEP;
            if(i>0)
            {
                Length += Vector3.Distance(Points[i], Points[i - 1]);
                PointLengths[i] = Length;
            }
        }
        Walls = Mathf.RoundToInt(Length / 2 / 10);


    }
    public Vector3 GetPointPosition(float t)
    {
        if (t < 0 || t > 1)
            throw new ArgumentException("Time must be between 0 and 1. Given time was " + t);
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return
           StartNode.Position.transform.position * (omt2 * omt) +
           GetInverseDirection(StartNode,mod1) * (3f * omt2 * t) +
           GetInverseDirection(EndNode, mod2) * (3f * omt * t2) +
           EndNode.Position.transform.position * (t2 * t);
    }
    public Vector3 GetPointTangent(float t)
    {
        if (t < 0 || t > 1)
            throw new ArgumentException("Time must be between 0 and 1. Given time was " + t);
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent =
            StartNode.Position.transform.position * (-omt2) +
            GetInverseDirection(StartNode, mod1) * (3 * omt2 - 2 * omt) +
            GetInverseDirection(EndNode, mod2) * (-3 * t2 + 2 * t) +
            EndNode.Position.transform.position * (t2);
        return tangent.normalized;
    }
    public Vector3 GetInverseDirection(Node node,float mod)
    {
        return ((2 * node.Position.transform.position)) - (mod*node.Direction.transform.position);
    }
}
