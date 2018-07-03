using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

/// <summary>
/// A component that create a deformed mesh from a given one, according to a cubic Bézier curve and other parameters.
/// The mesh will always be bended along the X axis. Extreme X coordinates of source mesh verticies will be used as a bounding to the deformed mesh.
/// The resulting mesh is stored in a MeshFilter component and automaticaly updated each time the cubic Bézier curve control points are changed.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MeshAlter : MonoBehaviour
{
	private Mesh source, result;
	private readonly List<Vertex> vertices = new List<Vertex>();

	private Quaternion sourceRotation;
	private Vector3 sourceTranslation;

	public BezierCurve curve;
	private float startScaleY = 1, endScaleY = 1;
	private float startScaleZ = 1, endScaleZ = 1;
	private float startRoll, endRoll;
	private float CurveStart = 1;
	private int wallCount = 1;
	private Vector3 offset;
	private int wall = 0;
	public float Length;

	private void OnEnable()
	{
		result = new Mesh();
		GetComponent<MeshFilter>().sharedMesh = result;
		

	}
	public void SetOffset(Vector3 Offset)
	{
		offset = Offset;

	}
	public void SetWallPercent(int WallCount, int WallNumber)
	{
		//Debug.Log("MA " + WallCount + " " + WallNumber);
		wallCount = WallCount;
		wall = WallNumber;

	}
	public void SetCurve(BezierCurve curve, bool update = true)
	{
		if (this.curve != null)
		{
			this.curve.Changed.RemoveListener(() => Compute());
		}
		this.curve = curve;
		curve.Changed.AddListener(() => Compute());
		if (update) Compute();
	}

	public void SetStartScaleY(float scale, bool update = true)
	{
		this.startScaleY = scale;
		if (update) Compute();
	}

	public void SetEndScaleY(float scale, bool update = true)
	{
		this.endScaleY = scale;
		if (update) Compute();
	}
	
	public void SetStartScaleZ(float scale, bool update = true)
	{
		this.startScaleZ = scale;
		if (update) Compute();
	}
	public void SetEndScaleZ(float scale, bool update = true)
	{
		this.endScaleZ = scale;
		if (update) Compute();
	}
	public void SetStartRoll(float roll, bool update = true)
	{
		this.startRoll = roll;
		if (update) Compute();
	}
	public void SetEndRoll(float roll, bool update = true)
	{
		this.endRoll = roll;
		if (update) Compute();
	}
	
	public void SetSourceMesh(Mesh mesh, bool update = true)
	{
		if (source != mesh)
		{
			this.source = mesh;
			vertices.Clear();
			int i = 0;
			foreach (Vector3 vert in source.vertices)
			{
				Vertex v = new Vertex();
				v.v = vert;
				v.n = source.normals[i++];
				vertices.Add(v);
			}
		}
		if (update) Compute();

	}

	/// <summary>
	/// Set the rotation to apply to the source mesh before anything happens. Because source mesh will always be bended along the X axis but may be oriented differently.
	/// </summary>
	/// <param name="rotation"></param>
	/// <param name="update">If let to true, update the resulting mesh immediatly.</param>
	public void SetRotation(Quaternion rotation, bool update = true)
	{
		this.sourceRotation = rotation;
		if (update) Compute();
	}

	/// <summary>
	/// Set an offset to bend the mesh outside the spline.
	/// </summary>
	/// <param name="translation"></param>
	/// <param name="update"></param>
	public void SetTranslation(Vector3 translation, bool update = true)
	{
		sourceTranslation = translation;
		if (update) Compute();
	}


	private void Compute()
	{
		if (source == null)
			return;
		int nbVert = source.vertices.Length;
		// find the bounds along x
		float minX = float.MaxValue;
		float maxX = float.MinValue;
		foreach (Vertex vert in vertices)
		{
			Vector3 p = vert.v;
			if (sourceRotation != Quaternion.identity)
			{
				p = sourceRotation * p;
			}
			if (sourceTranslation != Vector3.zero)
			{
				p += sourceTranslation;
			}
			maxX = Math.Max(maxX, p.x);
			minX = Math.Min(minX, p.x);
		}
		Length = Math.Abs(maxX - minX);

		List<Vector3> deformedVerts = new List<Vector3>(nbVert);
		List<Vector3> deformedNormals = new List<Vector3>(nbVert);
		Color[] colors = new Color[nbVert];
		int i = 0;
		CurveStart = (curve.Length / wallCount * wall) / curve.Length;
		// for each mesh vertex, we found its projection on the curve
		foreach (Vertex vert in vertices)
		{
			Vector3 p = vert.v;
			Vector3 n = vert.n;
			//  application of rotation
			if (sourceRotation != Quaternion.identity)
			{
				p = sourceRotation * p;
				n = sourceRotation * n;
			}
			if (sourceTranslation != Vector3.zero)
			{
				p += sourceTranslation;
			}
			// Debug.Log(wall);
			float distanceRate = Math.Abs(p.x - minX) / Length / wallCount;

			
			float tPoint =  distanceRate + CurveStart;
			Vector3 curvePoint = curve.GetPointPosition(tPoint) -offset;
			//Debug.Log(tPoint + " cp " + curvePoint);
			colors[i++] = Color.Lerp(Color.red, Color.green, tPoint);
			Vector3 curveTangent = curve.GetPointTangent(tPoint);
			Quaternion q = CubicBezierCurve.GetRotationFromTangent(curveTangent) * Quaternion.Euler(0, -90, 0);

			// application of scale
			float scaleAtDistanceY = startScaleY + (endScaleY - startScaleY) * distanceRate;
			float scaleAtDistanceZ = startScaleZ + (endScaleZ - startScaleZ) * distanceRate;
			p = new Vector3(p.x * scaleAtDistanceZ, p.y * scaleAtDistanceY, p.z * scaleAtDistanceZ);

			// application of roll
			float rollAtDistance = startRoll + (endRoll - startRoll) * distanceRate;
			p = Quaternion.AngleAxis(rollAtDistance, Vector3.right) * p;
			n = Quaternion.AngleAxis(rollAtDistance, Vector3.right) * n;

			// reset X value of p
			p = new Vector3(0, p.y, p.z);

			deformedVerts.Add(q * p + curvePoint);
			if (p.x == minX || p.x == maxX)
			{

			}
			else
			{
				deformedNormals.Add(q * n);
			}

		}

		result.vertices = deformedVerts.ToArray();
		result.normals = deformedNormals.ToArray();
		result.uv = source.uv;
		result.triangles = source.triangles;
		result.colors = colors;
		GetComponent<MeshFilter>().mesh = result;
	}

	private struct Vertex
    {
        public Vector3 v;
        public Vector3 n;
    }

    private void OnDestroy()
    {
        curve.Changed.RemoveListener(() => Compute());
    }

}