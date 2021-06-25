using System.Collections.Generic;
using Shapes;
using UnityEngine;
using DebugMenu.CustomAttribute.Runtime;

namespace DebugMenu.InGameDrawer.Runtime
{
    public class MeshBoundingBoxDisplay : MonoBehaviour
    {
        #region Constants

        public const float LINE_THICKNESS = 0.02f;

        #endregion
        
        
        #region Unity API

        private void Start()
        {
            _mesh = new List<MeshRenderer>();

            GetMeshInScene();
        }

        private void Update()
        {
            if (!_isDisplay) return;

            DrawMeshBoundingBoxes();
        }

        #endregion


        #region Utils

        [DebugMenu("Settings/Gizmos/MeshBoundingBox")]
        public static void ToggleMeshBoundingBoxDisplay()
        {
            _isDisplay = !_isDisplay;
            RefreshMeshes();
        }

        private static void RefreshMeshes()
        {
            _mesh.Clear();
            GetMeshInScene();
        }

        private static void GetMeshInScene()
        {
            foreach (var mesh in FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[])
            {
                _mesh.Add(mesh);
            }
        }

        #endregion


        #region DrawingShape

        private static void DrawMeshBoundingBoxes()
        {
            Camera cam = Camera.main;

            using (Draw.Command(cam))
            {
                Draw.Color = Color.yellow;

                foreach (var mesh in _mesh)
                {
                    SetUpBoundingBox(mesh);
                }
            }
        }

        private static void SetUpBoundingBox(MeshRenderer mesh)
        {
            Draw.LineThickness = LINE_THICKNESS;
            PolylinePath[] paths = new PolylinePath[6];

            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = new PolylinePath();
            }

            Vector3 halfSize = mesh.bounds.extents;
            Vector3 center = mesh.bounds.center;

            Vector3 upFrontRightVertices = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
            Vector3 upFrontLeftVertices = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);
            Vector3 upBackRightVertices = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            Vector3 upBackLeftVertices = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            Vector3 downFrontRightVertices = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            Vector3 downFrontLeftVertices = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            Vector3 downBackRightVertices = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            Vector3 downBackLeftVertices = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);

            paths[0].AddPoints(new Vector3[] { upFrontRightVertices, upFrontLeftVertices, upBackLeftVertices, upBackRightVertices });
            paths[1].AddPoints(new Vector3[] { downFrontRightVertices, downFrontLeftVertices, downBackLeftVertices, downBackRightVertices });
            paths[2].AddPoints(new Vector3[] { upFrontRightVertices, upFrontLeftVertices, downFrontLeftVertices, downFrontRightVertices });
            paths[3].AddPoints(new Vector3[] { upBackRightVertices, upBackLeftVertices, downBackLeftVertices, downBackRightVertices });
            paths[4].AddPoints(new Vector3[] { upFrontLeftVertices, upBackLeftVertices, downBackLeftVertices, downFrontLeftVertices });
            paths[5].AddPoints(new Vector3[] { upFrontRightVertices, upBackRightVertices, downBackRightVertices, downFrontRightVertices });

            for (int i = 0; i < paths.Length; i++)
            {
                Draw.Polyline(paths[i], true);
            }
        }

        #endregion


        #region private Members

        private static bool _isDisplay;
        private static List<MeshRenderer> _mesh;

        #endregion
    }
}