using UnityEngine;
using Unity.Profiling;
using System.Text;
using Shapes;
using DebugMenu.CustomAttribute.Runtime;

namespace DebugMenu.InGameDrawer.Runtime
{
    public class RenderCountProfiler : ImmediateModeShapeDrawer
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

        [DebugMenu("Settings/Performances/GPU Memory Counter")]
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

        private static void Display()
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
            if (_batchesCount.Valid)
            {
                stringBuilder.AppendLine($"Batches Count: {_batchesCount.LastValue}");
            }

            if (_renderTexturesCount.Valid)
            {
                stringBuilder.AppendLine($"Render Textures Count: {_renderTexturesCount.LastValue}");
            }

            if (_shadowCastersCount.Valid)
            {
                stringBuilder.AppendLine($"Shadow Casters Count: {_shadowCastersCount.LastValue}");
            }

            if (_indexBufferUploadInFrameCount.Valid)
            {
                stringBuilder.AppendLine($"Index Buffer Upload In Frame Count: {_indexBufferUploadInFrameCount.LastValue}");
            }            

            _statsText = stringBuilder.ToString();
        }

        private static void StartRecords()
        {
            _batchesCount = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Batches Count");
            _renderTexturesCount = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Render Textures Count");
            _shadowCastersCount = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Shadow Casters Count");
            _indexBufferUploadInFrameCount = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Index Buffer Upload In Frame Count");
        }

        private static void DisposeRecords()
        {
            _batchesCount.Dispose();
            _renderTexturesCount.Dispose();
            _shadowCastersCount.Dispose();
            _indexBufferUploadInFrameCount.Dispose();
        }

        #endregion


        #region Private and Protected

        private static bool _isShowingProfiler;
        private static string _statsText;
        private static ProfilerRecorder _batchesCount;
        private static ProfilerRecorder _renderTexturesCount;
        private static ProfilerRecorder _shadowCastersCount;
        private static ProfilerRecorder _indexBufferUploadInFrameCount;

        #endregion
    }
}