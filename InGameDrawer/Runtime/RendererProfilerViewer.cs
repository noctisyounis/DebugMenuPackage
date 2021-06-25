using UnityEngine;
using Unity.Profiling;
using System.Text;
using Shapes;
using DebugMenu.CustomAttribute.Runtime;

namespace DebugMenu.InGameDrawer.Runtime
{
    public class RendererProfilerViewer : ImmediateModeShapeDrawer
    {
        #region Unity API               

        private void Update()
        {
            if (!_isShowingProfiler) return;
            
            BuildStatisticsString();
            Display();
        }

        #endregion


        #region Main

        [DebugMenu("Settings/Performances/Renderer Profiler")]
        public static void ToggleDisplay()
        {
            _isShowingProfiler = !_isShowingProfiler;
            if (_isShowingProfiler)
            {
                StartRecords();
            }
            else
            {
                DisposeRecords();
            }
        }

        private void Display()
        {
            Camera camera = Camera.main;
            using (Draw.Command(camera))
            {
                var screenPosition = new Vector3(camera.pixelWidth - 20, camera.pixelHeight - 20, 1);
                var viewportPosition = camera.ScreenToViewportPoint(screenPosition);
                var worldPosition = camera.ViewportToWorldPoint(viewportPosition);

                Draw.Text(worldPosition, camera.transform.forward, _statsText, TextAlign.TopRight, 0.5f, Color.red);
            }
        }

        private void BuildStatisticsString()
        {
            var stringBuilder = new StringBuilder(500);
            if (_passCallsRecorder.Valid)
            {
                stringBuilder.AppendLine($"{_PASS_CALLS_NAME}: {_passCallsRecorder.LastValue}");
            }

            if (_drawCallsRecorder.Valid)
            {
                stringBuilder.AppendLine($"{_DRAW_CALLS_NAME}: {_drawCallsRecorder.LastValue}");
            }

            if (_verticesRecorder.Valid)
            {
                stringBuilder.AppendLine($"{_VERTICES_NAME}: {_verticesRecorder.LastValue}");
            }    
            
            _statsText = stringBuilder.ToString();
        }

        private static void StartRecords()
        {
            _passCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, $"{_PASS_CALLS_NAME} Count");
            _drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, $"{_DRAW_CALLS_NAME} Count");
            _verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, $"{_VERTICES_NAME} Count");
        }

        private static void DisposeRecords()
        {
            _passCallsRecorder.Dispose();
            _drawCallsRecorder.Dispose();
            _verticesRecorder.Dispose();
        }

        #endregion


        #region Private and Protected

        private static bool _isShowingProfiler;
        private string _statsText;
        private static ProfilerRecorder _passCallsRecorder;
        private static ProfilerRecorder _drawCallsRecorder;
        private static ProfilerRecorder _verticesRecorder;

        #endregion


        #region Constants
            
        private const string _PASS_CALLS_NAME = "SetPass Calls";
        private const string _DRAW_CALLS_NAME = "Draw Calls";
        private const string _VERTICES_NAME = "Vertices";

        #endregion
    }
}