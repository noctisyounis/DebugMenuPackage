using UnityEngine;
using UnityEngine.SceneManagement;

namespace DebugMenu.UI.Runtime
{
    public class SceneFacade : MonoBehaviour
    {
        #region Main

        public void LoadSceneAdditive(string sceneName)
        {
            if (!isLoaded)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                isLoaded = true;
            }
        }

        public void RemoveAdditiveScene(string sceneName)
        {
            if (isLoaded)
            {
                SceneManager.UnloadSceneAsync(sceneName);
                isLoaded = false;
            }
        }

        #endregion Main


        #region private

        private bool isLoaded;

        #endregion private
    }
}