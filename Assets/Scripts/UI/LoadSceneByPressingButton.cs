using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectRaoni
{
    public class LoadSceneByPressingButton : MonoBehaviour
    {
        public string sceneToLoad = "Cutscene2";
        
        public void LoadScene()
        {
            SceneManager.LoadScene(this.sceneToLoad);
        }
    }
}
