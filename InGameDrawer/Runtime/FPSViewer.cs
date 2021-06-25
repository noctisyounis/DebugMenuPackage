using UnityEngine;
using Shapes;
using DebugMenu.CustomAttribute.Runtime;

namespace DebugMenu.InGameDrawer.Runtime
{
    public class FPSViewer : ImmediateModeShapeDrawer
    {
        #region Exposed

        [Range(0.1f, 1.5f)]
        public float m_refreshIntervalInSeconds;

        #endregion


        #region Unity API

        private void Awake()
        {
            _timeOfLastDisplayUpdate = Time.time;
        }

        private void Update()
        {
            if (!_isShowingFps) return;

            if (RefreshTimerReached())
            {
                RefreshFPS();
                _timeOfLastDisplayUpdate = Time.time;
            }

            DisplayFPS();
        }
                
        #endregion


        #region Main

        [DebugMenu("Settings/Performances/Show Framerate")]
        public static void ToggleFPSDisplay()
        {
            _isShowingFps = !_isShowingFps;
        }

        private void RefreshFPS()
        {
            _fps = (int)(1f / Time.unscaledDeltaTime);
        }

        private void DisplayFPS()
        {
            Camera camera = Camera.main;
            using (Draw.Command(camera))
            {
                var screenPosition = new Vector3(camera.pixelWidth - 20, camera.pixelHeight - 20, 1);
                var viewportPosition = camera.ScreenToViewportPoint(screenPosition);
                var worldPosition = camera.ViewportToWorldPoint(viewportPosition);

                Draw.Text(worldPosition, camera.transform.forward, $"Framerate: {_fps} FPS", TextAlign.TopRight, 0.5f, Color.red);            
            }
        }

        #endregion


        #region Utils

        private bool RefreshTimerReached()
        {
            return Time.time > _timeOfLastDisplayUpdate + m_refreshIntervalInSeconds;
        }

        #endregion
                     

        #region Private Fields

        private static bool _isShowingFps;
        private int _fps;
        private float _timeOfLastDisplayUpdate;

        #endregion
    }    
}