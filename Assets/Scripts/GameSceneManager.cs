using System.Collections.Generic;
using UnityEngine;


public class GameSceneManager : MonoBehaviour
{
    public float journalAnimationDuration;
    public float sceneTransitionDuration;

    private Dictionary<string, string> SCENE_PATH_BY_NAME = new Dictionary<string, string> {
        { "kitchen", "Rooms/Kitchen" },
        { "bedroom", "Rooms/Bedroom" },
        { "livingroom", "Rooms/LivingRoom" },
        { "secretroom", "Rooms/SecretRoom" },
        { "blackjack", "Rooms/Blackjack" }
    };
    // private float sceneAlphaInJournal = 1.0f;
    private float sceneAlphaInJournal = 0.3f;

    private string currentSceneName = "livingroom";

    private GameManager gameManager;
    private GameObject journalCanvasGroup;
    private CanvasGroup blackScreen; 
    private GameObject currentScene;

    public bool isJournalOpen = true;
    public bool isJournalMovingUp = false;
    private bool isJournalMovingDown = false;
    private float journalStartedMovingAt = -100;

    private bool isTransitioningScene = false;
    private float sceneTransitionStartedAt = -100;
    private GameObject targetScene;
    private string targetSceneName;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        journalCanvasGroup = GameObject.Find("UI/Journal");
        blackScreen = GameObject.Find("UI/BlackScreen").GetComponent<CanvasGroup>();
        blackScreen.alpha = (1 - sceneAlphaInJournal);

        currentScene = GameObject.Find(SCENE_PATH_BY_NAME[currentSceneName]);
        currentScene.GetComponent<CanvasGroup>().alpha = 1;
        currentScene.GetComponent<CanvasGroup>().interactable = true;
        currentScene.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    void Update()
    {
        if (isJournalMovingUp || isJournalMovingDown) {
            UpdateJournalAnimation();
        }

        if (isTransitioningScene) {
            UpdateSceneTransition();
        }
    }

    public void LoadScene(string name) {
        if (isTransitioningScene || isJournalMovingUp || isJournalMovingDown || journalCanvasGroup.GetComponent<JournalManager>().IsAnimating()) {
            return;
        }

        if (currentSceneName != name) {
            if (name == "secret") {
                if (gameManager.isSecretRoomUnlocked) {
                    StartSceneTransition("secretroom");
                } else {
                    StartSceneTransition("blackjack");
                }
            } else {
                StartSceneTransition(name);
            }
        }

        if (isJournalOpen) {
            StartJournalAnimation();
        }
    }

    void StartSceneTransition(string transitionTo) {
        isTransitioningScene = true;
        sceneTransitionStartedAt = Time.time;
        targetSceneName = transitionTo;
        targetScene = GameObject.Find(SCENE_PATH_BY_NAME[transitionTo]);
    }

    void UpdateSceneTransition() {
        float progress = (Time.time - sceneTransitionStartedAt) / sceneTransitionDuration;
        if (progress <= 1) {
            targetScene.GetComponent<CanvasGroup>().alpha = progress;
            currentScene.GetComponent<CanvasGroup>().alpha = 1 - progress;
        } else {
            targetScene.GetComponent<CanvasGroup>().alpha = 1;
            targetScene.GetComponent<CanvasGroup>().interactable = true;
            targetScene.GetComponent<CanvasGroup>().blocksRaycasts = true;
            currentScene.GetComponent<CanvasGroup>().alpha = 0;
            currentScene.GetComponent<CanvasGroup>().interactable = false;
            currentScene.GetComponent<CanvasGroup>().blocksRaycasts = false;

            currentScene = targetScene;
            currentSceneName = targetSceneName;
            isTransitioningScene = false;
        }

        gameManager.SetCursor("default");
    }

    public void ToggleJournal() {
        if (isJournalMovingUp || isJournalMovingDown || journalCanvasGroup.GetComponent<JournalManager>().IsAnimating() || GameObject.Find("Rooms/Bedroom/Canvas/Puzzles/PressFPuzzle").GetComponent<PressFPuzzleManager>().isShown) {
            return;
        }

        gameManager.SetCursor("default");
        StartJournalAnimation();
    }

    void StartJournalAnimation() {
        isJournalMovingUp = !isJournalOpen;
        isJournalMovingDown = isJournalOpen;
        journalStartedMovingAt = Time.time;

        journalCanvasGroup.GetComponent<CanvasGroup>().interactable = !isJournalOpen;
    }

    void UpdateJournalAnimation() {
        float progress = (Time.time - journalStartedMovingAt) / journalAnimationDuration;
        if (progress <= 1) {
            if (isJournalMovingUp) {
                progress = 1 - progress;
            }

            journalCanvasGroup.transform.position = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, -8, 0), progress);
            journalCanvasGroup.GetComponent<CanvasGroup>().alpha = 1 - progress;
            blackScreen.alpha = (1 - sceneAlphaInJournal) * (1 - progress);
        } else {
            journalCanvasGroup.transform.position = isJournalMovingUp ? new Vector3(0, 0, 0) : new Vector3(0, -8, 0);
            journalCanvasGroup.GetComponent<CanvasGroup>().alpha = isJournalMovingUp ? 1 : 0;
            blackScreen.alpha = isJournalMovingUp ? (1 - sceneAlphaInJournal) : 0;

            isJournalMovingUp = false;
            isJournalMovingDown = false;
            isJournalOpen = !isJournalOpen;

            if (!isJournalOpen) {
                GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
            }
        }
    }

    public bool IsAnimating() {
        return isJournalMovingDown || isJournalMovingUp || isTransitioningScene;
    }

    public string GetCurrentSceneName() {
        return currentSceneName;
    }
}
