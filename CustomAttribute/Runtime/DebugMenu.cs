using System;

namespace DebugMenu.CustomAttribute.Runtime
{
    [AttributeUsage(AttributeTargets.Method,
                    Inherited = true,
                    AllowMultiple = false)]
    public class DebugMenuAttribute : Attribute
    {
        #region Public

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public bool IsQuickMenu { get; set; }

        #endregion Public


        #region Main

        public DebugMenuAttribute(string path)
        {
            _path = path;
        }

        #endregion Main


        #region Private

        private string _path;

        #endregion Private
    }
}