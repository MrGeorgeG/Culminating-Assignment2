using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManagement : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
