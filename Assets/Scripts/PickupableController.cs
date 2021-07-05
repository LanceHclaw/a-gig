using UnityEngine;

public class PickupableController : MonoBehaviour
{
    public string evidenceName;

    public int evidenceID;
    private MainQuestManager mqManager;

    private DataStorage dataStorage;
    private GameManager gameManager;
    private EvidenceManager evidenceManager;
    private FlashManager flashManager;
    private EvidenceCollectedController evidenceCollectedController;

    private bool isMouseOnObject = false;

    void Start()
    {
        mqManager = GameObject.Find("GameManager").GetComponent<MainQuestManager>();

        dataStorage = GameObject.Find("DataStorage").GetComponent<DataStorage>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        evidenceManager = GameObject.Find("UI/Journal/EvidenceCanvas/Evidence").GetComponent<EvidenceManager>();
        flashManager = GameObject.Find("UI/WhiteScreen").GetComponent<FlashManager>();
        evidenceCollectedController = GameObject.Find("UI/EvidenceCollected").GetComponent<EvidenceCollectedController>();
    }

    void Update()
    {
        if (mqManager.IsEvidenceCollected(evidenceID) || !gameManager.IsParentSceneActive(gameObject))
        {
            isMouseOnObject = false;
            return;
        }
        /*if (dataStorage.IsEvidenceCollected(evidenceName) || !gameManager.IsParentSceneActive(gameObject)) {
            isMouseOnObject = false;
            return;
        }*/

        if (Input.GetMouseButtonDown(0) && isMouseOnObject) {
            flashManager.PhotoFlash();
            evidenceCollectedController.ShowPhoto(evidenceID);
            gameManager.SetCursor("default");
            evidenceManager.AddEvidence(evidenceID);
        }
    }

    void OnMouseOver()
    {
        if (mqManager.IsEvidenceCollected(evidenceID) || !gameManager.IsParentSceneActive(gameObject)) return; 
        /*if (dataStorage.IsEvidenceCollected(evidenceName) || !gameManager.IsParentSceneActive(gameObject)) {
            return;
        }*/
        gameManager.SetCursor("camera");
        isMouseOnObject = true;
    }
    void OnMouseExit()
    {
        if (mqManager.IsEvidenceCollected(evidenceID) || !gameManager.IsParentSceneActive(gameObject)) return;
        /*if (dataStorage.IsEvidenceCollected(evidenceName) || !gameManager.IsParentSceneActive(gameObject)) {
            return;
        }*/
        gameManager.SetCursor("default");
        isMouseOnObject = false;
    }
}
