#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Diagnostics;
using DebugMenu.CustomAttribute.Runtime;

namespace DebugMenu.CustomAttribute.Editor
{
    public class DictionaryValidatorMenuItem
    {
#if UNITY_EDITOR
        [MenuItem("Debug Menu/Validate Methods")]
        [Conditional("DEBUG")]
#endif

        #region Utils

        public static void TryValidate()
        {
            DebugAttributeRegistry.ValidateMethods();
        }

        #endregion
    }
}