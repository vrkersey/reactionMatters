using System;
using System.Collections.Generic;
using UnityEngine;


    [ExecuteInEditMode]
    public class LevelEditorBox:MonoBehaviour
    {
        private LevelEditor editor;
        public int X, Z;
    
        private void Start()
        {
            editor = GameObject.Find("LevelEditor").GetComponent<LevelEditor>();
        }
        private void OnMouseDown()
        {
        SendCoord();
        }
    public void SendCoord()
    {
        editor.MoveToCell(X, Z);
    }
    }

