using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    private bool isShowingItem = false;
    private bool isShowingConnection = false;

    private TextMeshProUGUI text;

    private DataStorage dataStorage;

    void Start()
    {
        dataStorage = GameObject.Find("DataStorage").GetComponent<DataStorage>();
        text = gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Hide();
    }

    void Update()
    {
        if (isShowingItem || isShowingConnection) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }

    public void ShowPhoto(string name) {
        isShowingItem = true;
        text.text = dataStorage.evidenceDescriptions[name];
    }

    public void ShowThread(string key, int idx, int opt) {
        isShowingConnection = true;
        if (dataStorage.connectionDescriptions[key][idx].Item2.Item1 != "") {
            text.text = dataStorage.connectionDescriptions[key][idx].Item2.Item1 + "\n" + dataStorage.connectionDescriptions[key][idx].Item2.Item2[opt].Item1;
        } else {
            text.text = dataStorage.connectionDescriptions[key][idx].Item2.Item2[opt].Item1;
        }
    }

    public void Hide() {
        isShowingItem = false;
        isShowingConnection = false;
        gameObject.transform.position = new Vector3(1000, 0, 0);
    }

    public bool IsShown() {
        return isShowingItem || isShowingConnection;
    }
}
