using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity.Profiling;
using Shapes;
using DebugMenu.CustomAttribute.Runtime;

namespace DebugMenu.InGameDrawer.Runtime
{
    public class MemoryProfiler : ImmediateModeShapeDrawer
    {
        #region Unity API

        private void Awake() 
        {
            _profilerRecorders = new Dictionary<string, ProfilerRecorder>();
            GenerateProfilers();
        }

        private void Update()
        {
            _stringBuilder = new StringBuilder(500);
            
            foreach(var item in _profilerRecorders)
            {
                AddProfilerToStringBuilder(item.Key, item.Value);
            }

            _statsText = _stringBuilder.ToString();
            if (!_isShowingProfiler) return;
            ShowMemoryProfiler();
        }

        #endregion


        #region Utils

        private static void GenerateProfilers()
        {
            AddNewProfilerRecorder(TOTAL_RESERVED_MEMORY_NAME);
            AddNewProfilerRecorder(GC_RESERVED_MEMORY_NAME);
            AddNewProfilerRecorder(TEXTURE_MEMORY_NAME);
            AddNewProfilerRecorder(MESH_MEMORY_NAME);
        }

        [DebugMenu("Settings/Performances/Memory Profiler")]
        public static void ToggleShowProfiler()
        {
            _isShowingProfiler = !_isShowingProfiler;

            if(_isShowingProfiler)
            {
                EnableAllProfilers();
            }
            else
            {
                DisableAllProfilers();
            }
        }

        private static void ShowMemoryProfiler()
        {
            Camera cam = Camera.main;
            using (Draw.Command(cam))
            {
                var pos = cam.ScreenToViewportPoint(new Vector3(cam.pixelWidth - 20, cam.pixelHeight - 20, 1));
                var goodPos = cam.ViewportToWorldPoint(pos);
                Draw.Text(goodPos, cam.transform.forward, _statsText, TextAlign.TopRight, 0.5f, Color.red);
            }     
        }

        private static float ConvertByteToMegaByte(float byteValue)
        {
            return (byteValue / 1_000_000f);
        }

        private static void AddNewProfilerRecorder(string statName)
        {
            if(_profilerRecorders.ContainsKey(statName)) return;

            var profiler = new ProfilerRecorder(ProfilerCategory.Memory, statName);
            _profilerRecorders.Add(statName, profiler);
        }

        private static void AddProfilerToStringBuilder(string statName, ProfilerRecorder profiler)
        {
            if(profiler.Valid)
            {
                _stringBuilder.AppendLine($"{statName}: {ConvertByteToMegaByte(profiler.LastValue):0.00} MB");
            }
        }

        private static void EnableAllProfilers()
        {
            foreach (var item in _profilerRecorders)
            {
                item.Value.Start();
            }
        }

        private static void DisableAllProfilers()
        {
            foreach (var item in _profilerRecorders)
            {
                item.Value.Stop();
            }
        }

        #endregion    


        #region Private and Protected

        #region Constants
        
        private const string TOTAL_RESERVED_MEMORY_NAME = "Total Reserved Memory";
        private const string GC_RESERVED_MEMORY_NAME = "GC Reserved Memory";
        private const string TEXTURE_MEMORY_NAME = "Texture Memory";
        private const string MESH_MEMORY_NAME = "Mesh Memory";

        #endregion

        private static StringBuilder _stringBuilder;
        private static bool _isShowingProfiler;
        private static string _statsText;
        private static Dictionary<string, ProfilerRecorder> _profilerRecorders;

        #endregion
    }
}