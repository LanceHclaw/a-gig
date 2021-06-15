using UnityEngine;

public class PoiController : MonoBehaviour
{
    public string poiName;
    public Sprite poiImage;

    private GameManager gameManager;
    private DataStorage dataStorage;
    private PoiFoundController poiFoundController;

    private bool isMouseOnObject = false;
    private bool wasTriggered = false;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataStorage = GameObject.Find("DataStorage").GetComponent<DataStorage>();
        poiFoundController = GameObject.Find("UI/PoiFound").GetComponent<PoiFoundController>();
    }

    void Update()
    {
        if (!gameManager.IsParentSceneActive(gameObject)) {
            isMouseOnObject = false;
            return;
        }

        if (Input.GetMouseButtonDown(0) && isMouseOnObject) {
            if (poiName == "pressF") {
                GameObject.Find("Rooms/Bedroom/Canvas/Puzzles/PressFPuzzle").GetComponent<PressFPuzzleManager>().Show();
                gameManager.SetCursor("default");
                return;
            }
            if (poiName == "MapPiece") {
                gameManager.isSecretRoomShown = true;
            }
            poiFoundController.ShowPOI(poiName, poiImage);
            if (!wasTriggered) {
                if (poiName == "BedroomDrawerKey") {
                    GameObject.Find("Rooms/Bedroom/Canvas/POIs/LockedDrawer").SetActive(false);
                    GameObject.Find("Rooms/Bedroom/Canvas/Pickupables/PackOfSigarettes").SetActive(true);
                }
                wasTriggered = true;
            }
        }

        if (!isMouseOnObject) {
            poiFoundController.Hide();
        }
    }

    void OnMouseOver()
    {
        if (!gameManager.IsParentSceneActive(gameObject)) {
            return;
        }
        gameManager.SetCursor("magglass");
        isMouseOnObject = true;
    }
    void OnMouseExit()
    {
        if (!gameManager.IsParentSceneActive(gameObject)) {
            return;
        }
        gameManager.SetCursor("default");
        isMouseOnObject = false;
    }
}
