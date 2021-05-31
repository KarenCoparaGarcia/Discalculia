using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class cambioEscena : MonoBehaviour {

    public void LoadScene(string escena)
    {
        SceneManager.LoadScene(escena);
    }

}
