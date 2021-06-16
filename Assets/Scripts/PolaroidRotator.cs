using UnityEngine;

public class PolaroidRotator : MonoBehaviour
{
    void Start()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(-5f, 5f));
    }
}
