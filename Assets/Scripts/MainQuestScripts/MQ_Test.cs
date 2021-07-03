using System.Collections;
using System.Collections.Generic;
using ProgressionVector;
using UnityEngine;

public class MQ_Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(var field in AllEvidence.GetAllEvidence())
        {
            Debug.Log(field.name + "\n" + field.description + "\n");
        }

        foreach(var end in AllEndings.GetAllEndings())
        {
            Debug.Log(end.name + "\n" + end.epilogue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
