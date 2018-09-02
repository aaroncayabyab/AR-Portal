using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    Scene activeScene;

	// Use this for initialization
	void Start () {
        activeScene = SceneManager.GetActiveScene();
	}
	


    //Attach to ResetARButton
    public void ResetScene()
    {
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    //Attach to SwapPortalButton
    public void SwitchScene()
    {
        if(activeScene.buildIndex == 0)
        {
            //Only be able to swith scene safely if included in build index
            if(SceneManager.sceneCountInBuildSettings > 1)
                SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }


    }

}
