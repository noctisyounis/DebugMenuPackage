using System.Collections.Generic;
using Shapes;
using UnityEngine;
using DebugMenu.CustomAttribute.Runtime;

namespace DebugMenu.InGameDrawer.Runtime
{
    public class CollidersOutliner : MonoBehaviour
    {
        #region Unity API

        private void Start()
        {
            _boxColliders = new List<BoxCollider>();
            _sphereColliders = new List<SphereCollider>();
            _capsuleColliders = new List<CapsuleCollider>();
            GetTypeOfColliders();
            SetShapesColor(new Color(0, 1, 0, 0.2f));
            SetShapesBlendMode(ShapesBlendMode.Transparent);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetState();
            }

            if (_state)
            {
                DisplayAllCollidersInScene();
                RefreshAllCollidersLists();
            }
        }
        
        #endregion


        #region Utils
        private static void RefreshAllCollidersLists()
        {
            _boxColliders.Clear();
            _sphereColliders.Clear();
            _capsuleColliders.Clear();
            GetTypeOfColliders();
        }

        [DebugMenu("Settings/Gizmos/Show Colliders")]
        public static void SetState()
        {
            _state = !_state;
        }

        private static List<Collider> GetAllCollidersInScene()
        {
            List<Collider> collidersInScene = new List<Collider>();

            foreach (var element in Resources.FindObjectsOfTypeAll(typeof(Collider)) as Collider[])
            {
                collidersInScene.Add(element);
            }

            return collidersInScene;
        }

        private static void GetTypeOfColliders()
        {
            foreach (var element in GetAllCollidersInScene())
            {
                if (element.GetType() == typeof(BoxCollider))
                {
                    _boxColliders.Add((BoxCollider) element);
                }
                else if (element.GetType() == typeof(SphereCollider))
                {
                    _sphereColliders.Add((SphereCollider) element);
                }
                else if (element.GetType() == typeof(CapsuleCollider))
                {
                    _capsuleColliders.Add((CapsuleCollider) element);
                }
            }
        }

        private static void SetShapesColor(Color color)
        {   
            Draw.Color = color;
        }

        private static void SetShapesBlendMode(ShapesBlendMode blendMode)
        {
            Draw.BlendMode = ShapesBlendMode.Transparent;
        }

        #endregion


        #region DrawingShape
        public static void DisplayAllCollidersInScene()
        {
            Camera cam = Camera.main;


            using (Draw.Command(cam))
            {
                DisplayAllBoxCollidersInScene();
                DisplayAllSphereCollidersInScene();
                DisplayAllCapsuleCollidersInScene();
            }
        }

        public static void DisplayAllBoxCollidersInScene()
        {
            foreach (var element in _boxColliders)
            {
                Draw.Cuboid(element.bounds.center, element.transform.rotation, element.bounds.size);
            }
        }

        public static void DisplayAllSphereCollidersInScene()
        {
            foreach (var element in _sphereColliders)
            {
                Draw.Sphere(element.bounds.center, element.radius);
            }
        }

        public static void DisplayAllCapsuleCollidersInScene()
        {
            foreach (var element in _capsuleColliders)
            {
                Draw.Cuboid(element.bounds.center, element.transform.rotation,
                    new Vector3(element.radius * 2, element.height,
                        element.radius * 2));
            }
        }

        #endregion


        #region private Members
        private static List<BoxCollider> _boxColliders;
        private static List<SphereCollider> _sphereColliders;
        private static List<CapsuleCollider> _capsuleColliders;
        private static bool _state;

        #endregion
    }
}