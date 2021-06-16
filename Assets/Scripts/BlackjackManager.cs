using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlackjackManager : MonoBehaviour
{
    private GameObject cardSlots;
    private TextMeshProUGUI display;

    private List<int> slotValues = new List<int>() { 0, 0, 0, 0 };

    private bool won = false;
    private bool winPending = false;
    private float winStartedPendingAt = -100;
    private float winPendingDuration = 0.5f;

    private GameManager gameManager;
    private GameSceneManager gameSceneManager;

    void Start()
    {
        cardSlots = gameObject.transform.Find("Canvas/CardSlots").gameObject;
        display = gameObject.transform.Find("Canvas/Display").GetComponent<TextMeshProUGUI>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    void Update()
    {
        if (winPending) {
            if (Time.time - winStartedPendingAt > winPendingDuration) {
                winPending = false;
                won = true;
                Camera.main.GetComponent<SoundManager>().Play("BJP_door_open");
                gameManager.isSecretRoomUnlocked = true;
                gameSceneManager.LoadScene("secretroom");
            }
            return;
        }

        if (won || gameSceneManager.GetCurrentSceneName() != "blackjack") { 
            return;
        }

        var cs = CurrentSum();
        if (cs <= 21) {
            if (cs == 21 && !slotValues.Contains(0) && !won && !winPending) {
                winPending = true;
                winStartedPendingAt = Time.time;
            }

            display.text = IntToStr(cs);
        } else {
            display.text = "BUST";
        }
    }

    int CurrentSum() {
        int sum = 2;
        foreach (int v in slotValues) {
            sum += v;
        }
        return sum;
    }

    string IntToStr(int v) {
        if (v < 10) {
            return "0" + v.ToString();
        } else {
            return v.ToString();
        }
    }

    public (bool, int, Vector2) AlignToSlotIfExists(int numericValue, Vector2 pos) {
        for (var i = 0; i < cardSlots.transform.childCount; ++i) {
            var cardSlot = cardSlots.transform.GetChild(i);
            var cardSlotRect = cardSlot.GetComponent<RectTransform>().rect;
            var topLeft = cardSlot.position - new Vector3(cardSlotRect.width / 2, cardSlotRect.height / 2);
            var bottomRight = cardSlot.position + new Vector3(cardSlotRect.width / 2, cardSlotRect.height / 2);

            if (topLeft.x <= pos.x && pos.x <= bottomRight.x && topLeft.y <= pos.y && pos.y <= bottomRight.y && slotValues[i] == 0) {
                slotValues[i] = numericValue;
                Camera.main.GetComponent<SoundManager>().Play("BJP_stick");
                return (true, i, new Vector2(cardSlot.position.x, cardSlot.position.y));
            }
        }

        return (false, 0, new Vector2(0, 0));
    }

    public void RemoveFromSlot(int idx) {
        slotValues[idx] = 0;
    }
}
