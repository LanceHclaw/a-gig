using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private MainQuestManager mqManager;

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
        mqManager = gameObject.GetComponent<MainQuestManager>();
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
        /*
        string allMajor = "";
        string allMinor = "";
        Dictionary<Ending, int> weights = new Dictionary<Ending, int>();
        string weightsString = "";

        foreach (var action in mqManager.playerProgress.actionFlags.Where(x => x.Value == true).Select(x => x.Key).ToList())
        {
            allMajor += action.ToString() + "\n";
            foreach(var weight in action.weights)
            {
                if (!weights.ContainsKey(weight.Key)) 
                    weights.Add(weight.Key, 0);
                weights[weight.Key] += weight.Value;
            }
        }
        foreach (var action in mqManager.playerProgress.minorActionFlags.Where(x => x.Value == true).Select(x => x.Key).ToList())
        {
            allMinor += action.ToString() + "\n";
        }
        
        Debug.Log(allMajor);
        Debug.Log(allMinor);
        foreach(var w in weights)
        {
            weightsString += w.Key.name + " " + w.Value + " \n";
        }
        Debug.Log(weightsString);
        */
        GlobalVersionControl.ending = mqManager.playerProgress.FinishQuest(mqManager.questData);
        //Debug.Log(GlobalVersionControl.ending.name);
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
