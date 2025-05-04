using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject startCanvas;
    public GameObject selectCharacterCanvas;

    public void ChangeToSelectCanvas()
    {
        startCanvas.SetActive(false);
        selectCharacterCanvas.SetActive(true);
    }

    public void ResumeButton()
    {
        pauseCanvas.SetActive(false);
    }
}
