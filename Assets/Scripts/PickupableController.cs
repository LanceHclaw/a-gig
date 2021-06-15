using UnityEngine;

public class PickupableController : MonoBehaviour
{
    public string evidenceName;

    private DataStorage dataStorage;
    private GameManager gameManager;
    private EvidenceManager evidenceManager;
    private FlashManager flashManager;
    private EvidenceCollectedController evidenceCollectedController;

    private bool isMouseOnObject = false;

    void Start()
    {
        dataStorage = GameObject.Find("DataStorage").GetComponent<DataStorage>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        evidenceManager = GameObject.Find("UI/Journal/EvidenceCanvas/Evidence").GetComponent<EvidenceManager>();
        flashManager = GameObject.Find("UI/WhiteScreen").GetComponent<FlashManager>();
        evidenceCollectedController = GameObject.Find("UI/EvidenceCollected").GetComponent<EvidenceCollectedController>();
    }

    void Update()
    {
        if (dataStorage.IsEvidenceCollected(evidenceName) || !gameManager.IsParentSceneActive(gameObject)) {
            isMouseOnObject = false;
            return;
        }

        if (Input.GetMouseButtonDown(0) && isMouseOnObject) {
            flashManager.PhotoFlash();
            evidenceCollectedController.ShowPhoto(evidenceName);
            gameManager.SetCursor("default");
            evidenceManager.AddEvidence(evidenceName);
        }
    }

    void OnMouseOver()
    {
        if (dataStorage.IsEvidenceCollected(evidenceName) || !gameManager.IsParentSceneActive(gameObject)) {
            return;
        }
        gameManager.SetCursor("camera");
        isMouseOnObject = true;
    }
    void OnMouseExit()
    {
        if (dataStorage.IsEvidenceCollected(evidenceName) || !gameManager.IsParentSceneActive(gameObject)) {
            return;
        }
        gameManager.SetCursor("default");
        isMouseOnObject = false;
    }
}
