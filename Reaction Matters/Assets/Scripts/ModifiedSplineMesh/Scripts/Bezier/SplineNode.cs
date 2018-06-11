using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Spline node storing a position and a direction (tangent).
/// Note : you shouldn't modify position and direction manualy but use dedicated methods instead, to insure event raising.
/// </summary>
[Serializable]
public class SplineNode {

    /// <summary>
    /// Node position
    /// Note : you shouldn't modify position and direction manualy but use dedicated methods instead, to insure event raising.
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// Node direction
    /// Note : you shouldn't modify position and direction manualy but use dedicated methods instead, to insure event raising.
    /// </summary>
    public Vector3 direction;
    public SplineNodeHolder snh;
    public Spline spline;
    public SplineNode(Vector3 position, Vector3 direction,Spline _spline, bool sphere = true) {
        SetPosition(position);
        SetDirection(direction);
        spline = _spline;

        if (sphere)
        {
            CreateSphere();
        }
    }
    public float DistanceMod=1;
    public float Dmod = 1;
    public bool UseInverseN1 = true,UseInverseN2 = true;
    public void CreateSphere()
    {
        if(snh!=null)
        {
            snh.node = null;
        }
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        snh = g.AddComponent<SplineNodeHolder>();
        snh.node = this;
    }
    /// <summary>
    /// Sets the new position and raises an event.
    /// </summary>
    /// <param name="p"></param>
    public void SetPosition(Vector3 p) {
        Debug.Log("Setting");
        if (!position.Equals(p)) {
            position.x = p.x;
            position.y = p.y;
            position.z = p.z;
            Debug.Log("Setted "+ position);
            if (Changed != null)
                Changed.Invoke();
        }
        else
        {
            Debug.Log("no Set");
        }
    }

    /// <summary>
    /// Sets the new direction and raises an event.
    /// </summary>
    /// <param name="d"></param>
    public void SetDirection(Vector3 d) {
        if (!direction.Equals(d)) {
            direction.x = d.x;
            direction.y = d.y;
            direction.z = d.z;
            if (Changed != null)
                Changed.Invoke();
        }
    }

    /// <summary>
    /// Event raised when position or direct changes.
    /// </summary>
    [HideInInspector]
    public UnityEvent Changed = new UnityEvent();
}
