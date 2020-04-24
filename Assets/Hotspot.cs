using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Hotspot : MonoBehaviour
{

    public bool IsFree = true;
    public Vector3 Position => transform.position;

    public Vector3 GizmoCubeSize = new Vector3(0.5f, 0.5f, 0.5f);

    public Vector3 PointOfInterest;

    void Start()
    {
        PointOfInterest = transform.parent.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.parent.position);
        Gizmos.color = IsFree ? Color.green : Color.red;
        Gizmos.DrawCube(transform.position, GizmoCubeSize);
        Gizmos.color = Color.white;
    }


    [MenuItem("GameObject/Navigation/Hotspot", false, 0)]
    static void AddHotspot(MenuCommand menuCommand)
    {
        GameObject GO = new GameObject("HotspotLocation");
        GO.AddComponent<Hotspot>();
        GameObjectUtility.SetParentAndAlign(GO, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(GO, "Create " + GO.name);
        Selection.activeObject = GO;
    }
}
