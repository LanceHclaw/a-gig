using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VersionSelectorUI : MonoBehaviour
{
    public Canvas selectorCanvas;

    public void CloseCanvas()
    {
        selectorCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void ClickRestricted()
    {
        GlobalVersionControl.restricted = true;
        CloseCanvas();
    }

    public void ClickUnrestricted()
    {
        GlobalVersionControl.restricted = false;
        CloseCanvas();
    }
}
