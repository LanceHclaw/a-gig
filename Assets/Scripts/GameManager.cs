using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private EvidenceCollectedController evidenceCollectedController;
    private GameSceneManager gameSceneManager;

    public bool isSecretRoomShown = false;
    public bool isSecretRoomUnlocked = false;

    public Texture2D cursorMain;
    public Texture2D cursorMagGlass;
    public Texture2D cursorCamera;
    public string currentCursor = "default";

    void Start()
    {
        evidenceCollectedController = GameObject.Find("UI/EvidenceCollected").GetComponent<EvidenceCollectedController>();
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Escape)) {
            gameSceneManager.ToggleJournal();
        }


        if (Input.GetKeyDown(KeyCode.P)) {
            isSecretRoomShown = true;
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            isSecretRoomUnlocked = true;
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            SwitchToEpilogue();
        }
    }

    public void SwitchToEpilogue() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public bool IsParentSceneActive(GameObject obj) {
        return (
            obj.transform.parent.parent.parent.GetComponent<RoomNameContainer>().roomName == gameSceneManager.GetCurrentSceneName() &&
            !gameSceneManager.isJournalOpen && !gameSceneManager.isJournalMovingUp &&
            !evidenceCollectedController.isShown && !evidenceCollectedController.showPending &&
            !GameObject.Find("Rooms/Bedroom/Canvas/Puzzles/PressFPuzzle").GetComponent<PressFPuzzleManager>().IsVisible()
        );
    }

    public void SetCursor(string name) {
        if (name == currentCursor) {
            return;
        }

        currentCursor = name;
        if (name == "camera") {
            Cursor.SetCursor(cursorCamera, new Vector2(268, 179), CursorMode.Auto);
        } else if (name == "magglass") {
            Cursor.SetCursor(cursorMagGlass, new Vector2(255, 255), CursorMode.Auto);
        } else {
            Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        }
    }
}
