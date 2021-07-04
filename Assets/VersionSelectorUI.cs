using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionSelectorUI : MonoBehaviour
{
    public Canvas selectorCanvas;
    public GameObject GameManagerObject;

    public void CloseCanvas()
    {
        selectorCanvas.gameObject.SetActive(false);
    }

    public void ClickRestricted()
    {
        GameManagerObject.GetComponent<MainQuestManager>().Instantiate(restricted: true);
        CloseCanvas();
    }

    public void ClickUnrestricted()
    {
        GameManagerObject.GetComponent<MainQuestManager>().Instantiate(restricted: false);
        CloseCanvas();
    }
}
