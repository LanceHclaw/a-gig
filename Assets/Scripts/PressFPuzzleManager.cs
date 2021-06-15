using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressFPuzzleManager : MonoBehaviour
{
    public GameObject togglePrefab;

    private int fieldW = 6;
    private int fieldH = 9;
    private Vector3 toggleTopLeftPos = new Vector3(-0.885f, 1.23f, 0);
    private float horizontalSpacer = 0.04f;
    private float verticalSpacer = 0.042f;

    private GameManager gameManager;
    private GameSceneManager gameSceneManager;
    private GameObject buttonsContainer;
    private SoundManager soundManager;

    public bool isShown = false;
    private bool isGoingUp = false;
    private bool isGoingDown = false;
    private float transitionStartedAt = -100;
    private float transitionDuration = 0.3f;

    private bool won = false;
    private bool winPending = false;
    private float winStartedPendingAt = -100;
    private float winPendingDuration = 1.5f;

    private List<List<bool>> togglesState = new List<List<bool>>();
    private List<List<int>> winState = new List<List<int>>() {
        new List<int>() { 0, 0, 0, 0, 0, 0 },
        new List<int>() { 0, 1, 1, 1, 1, 0 },
        new List<int>() { 0, 1, 0, 0, 0, 0 },
        new List<int>() { 0, 1, 0, 0, 0, 0 },
        new List<int>() { 0, 1, 1, 1, 0, 0 },
        new List<int>() { 0, 1, 0, 0, 0, 0 },
        new List<int>() { 0, 1, 0, 0, 0, 0 },
        new List<int>() { 0, 1, 0, 0, 0, 0 },
        new List<int>() { 0, 0, 0, 0, 0, 0 },
    };

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        soundManager = Camera.main.GetComponent<SoundManager>();

        buttonsContainer = gameObject.transform.Find("Buttons").gameObject;
        GenerateButtons();

        gameObject.transform.position = isShown ? new Vector3(0, 0, 0) : new Vector3(0, -5, 0);
        gameObject.GetComponent<CanvasGroup>().alpha = isShown ? 1: 0;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameSceneManager.isJournalOpen) {
            Hide();
        }

        if (isGoingDown || isGoingUp) {
            UpdateAnimation();
        }

        if (winPending) {
            float progress = (Time.time - winStartedPendingAt) / winPendingDuration;
            if (0.5f <= progress && progress <= 1) {
                var c = gameObject.transform.Find("WinImage").GetComponent<Image>().color;
                c.a = (progress - 0.5f) * 2;
                gameObject.transform.Find("WinImage").GetComponent<Image>().color = c;
            } else if (progress > 1) {
                var c = gameObject.transform.Find("WinImage").GetComponent<Image>().color;
                c.a = 1;
                gameObject.transform.Find("WinImage").GetComponent<Image>().color = c;
                winPending = false;
                won = true;

                GameObject.Find("Rooms/LivingRoom/Canvas/Pickupables/GunInABox").SetActive(true);
                GameObject.Find("Rooms/LivingRoom/Canvas/POIs/LockedBox").SetActive(false);
            }
        }
    }

    public void Show() {
        if (isShown || isGoingDown || isGoingUp) {
            return;
        }

        isGoingUp = true;
        transitionStartedAt = Time.time;
    }

    public void Hide() {
        if (!isShown || isGoingDown || isGoingUp) {
            return;
        }

        isGoingDown = true;
        transitionStartedAt = Time.time;
    }

    void UpdateAnimation() {
        float progress = (Time.time - transitionStartedAt) / transitionDuration;
        if (progress <= 1) {
            if (isGoingUp) {
                progress = 1 - progress;
            }
            gameObject.transform.position = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, -5, 0), progress);
            gameObject.GetComponent<CanvasGroup>().alpha = 1 - progress;
        } else {
            isShown = isGoingUp;
            gameObject.transform.position = isShown ? new Vector3(0, 0, 0) : new Vector3(0, -5, 0);
            gameObject.GetComponent<CanvasGroup>().alpha = isShown ? 1: 0;

            isGoingDown = false;
            isGoingUp = false;
        }
    }

    void GenerateButtons() {
        for (var y = 0; y < fieldH; ++y) {
            togglesState.Add(new List<bool>());
            for (var x = 0; x < fieldW; ++x) {
                togglesState[y].Add(false);

                var b = Instantiate(togglePrefab);
                b.transform.SetParent(buttonsContainer.transform);
                b.transform.localPosition = toggleTopLeftPos + new Vector3(
                    (b.GetComponent<RectTransform>().rect.width * b.transform.localScale.x + horizontalSpacer) * x,
                    -(b.GetComponent<RectTransform>().rect.height * b.transform.localScale.y + verticalSpacer) * y,
                    togglePrefab.transform.position.z
                );

                int _x = x;
                int _y = y;
                b.GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) => { ToggleCell(isOn, _x, _y); });
            }
        }
    }

    void ToggleCell(bool isOn, int x, int y) {
        if (won || winPending) {
            return;
        }

        togglesState[y][x] = isOn;

        soundManager.Play(isOn ? "FP_tile_on" : "FP_tile_off");

        if (IsWinState()) {
            winPending = true;
            winStartedPendingAt = Time.time;
        }
    }

    bool IsWinState() {
        for (var y = 0; y < fieldH; ++y) {
            for (var x = 0; x < fieldW; ++x) {
                if ((togglesState[y][x] ? 1 : 0) != winState[y][x]) {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsVisible() {
        return isShown || isGoingDown || isGoingUp;
    }
}
