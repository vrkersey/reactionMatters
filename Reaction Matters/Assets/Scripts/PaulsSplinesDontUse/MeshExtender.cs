using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Exemple of component to bend a mesh along a spline with an offset.
/// 
/// This component can be used as-is but will most likely be a base for your own component. For explanations of the base component, <see cref="ExemplePipe"/>
/// 
/// In this component, we use the MeshBender translation parameter.
/// It allows you move the source mesh on the Y and Z axis, considering that X axis as spline tangent.
/// 
/// This is usefull to align a mesh that is not centered without reworking it in a modeling tool.
/// It is also useful to offset the mesh from the spline, like in the case of raillings on road sides.
/// </summary>
[ExecuteInEditMode]
[SelectionBase]
[Serializable]
public class MeshExtender : MonoBehaviour {

	public GameObject MeshObj;
	public Mesh mesh;
	public Material mat;
    public Vector3 rotation;
    public float YOffset;
    public float ZOffset;
    public float scaleY = 1;
    public float scaleZ = 1;
    public int MeshCount=1;

    private BezierCurve curve = null;
    public List<GameObject> meshes = new List<GameObject>();
    private bool toUpdate = true;

    private void OnEnable() {
		curve = GetComponent<BezierCurve>();
		curve.Changed.AddListener(() => toUpdate = true);
		if (MeshObj.GetComponent<MeshRenderer>() != null)
		{

			mesh = MeshObj.GetComponent<MeshFilter>().sharedMesh;
			mat = MeshObj.GetComponent<MeshRenderer>().sharedMaterial;
		}
		else
		{
			mesh = MeshObj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
			mat = MeshObj.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
		}
		CreateMeshes();
    }

    private void OnValidate() {
        toUpdate = true;
		//Debug.Log(MeshObj.GetComponent<MeshRenderer>() != null);
		if (MeshObj.GetComponent<MeshRenderer>()!=null&&mesh==null)
		{
			
			mesh = MeshObj.GetComponent<MeshFilter>().sharedMesh;
			mat = MeshObj.GetComponent<MeshRenderer>().sharedMaterial;
		}
		else if(mesh==null)
		{
			mesh = MeshObj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
			mat = MeshObj.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
		}

	}

    private void Update() {
        if (toUpdate) {
            CreateMeshes();
            toUpdate = false;
        }
    }

    public void CreateMeshes() {
        foreach (GameObject go in meshes) {
            if (gameObject != null) {
                if (Application.isPlaying) {
                    Destroy(go);
                } else {
                    DestroyImmediate(go);
                }
            }
        }
        meshes.Clear();
		if(mesh==null||mat==null)
		{
			return;
		}
        int i = 0;
        
            for(int j =0;j<MeshCount;j++)
            {
                GameObject go = new GameObject("SplineMesh" + i++, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshAlter), typeof(MeshCollider));
                go.transform.parent = transform;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                //go.hideFlags = HideFlags.NotEditable;

                go.GetComponent<MeshRenderer>().material = mat;
                MeshAlter mb = go.GetComponent<MeshAlter>();
			if(mb==null)
			{
				mb = go.AddComponent<MeshAlter>();
			}
			mb.SetOffset( transform.position);
			mb.SetWallPercent(MeshCount, j);
                mb.SetSourceMesh(mesh, false);
                mb.SetRotation(Quaternion.Euler(rotation), false);
                mb.SetTranslation(new Vector3(j*10, YOffset, ZOffset), false);
                mb.SetCurve(curve, false);
                mb.SetStartScaleY(scaleY, false);
                mb.SetEndScaleY(scaleY);
                mb.SetStartScaleZ(scaleZ, false);
                mb.SetEndScaleZ(scaleZ);
                meshes.Add(go);
            }
            
        
    }
}
