using System.Collections;
using System.Collections.Generic;
using ProgressionVector;
using UnityEngine;

public class MQ_Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MQEndings ae = new MQEndings();
        MQConnections ac = new MQConnections(FileDirectory.ConnectionsRestrictedJsonFile, ae);

        foreach (var ending in ae.endingsById)
        {
            Debug.Log(ending.Key + " " + ending.Value.id + " " + ending.Value.name + " " + ending.Value.epilogue);
        }

        foreach(var cc in ac.ConnectionsByID)
        {
            Debug.Log(cc.Key + " " + cc.Value.id + " " + cc.Value.commonDescription + " " + cc.Value.options.Count);
        }
        /*foreach (var field in AllEvidence.GetAllEvidence())
        {
            Debug.Log(field.id);
            //Debug.Log(field.name + "\n" + field.description + "\n");
        }*/

        /*foreach(var end in AllEndings.GetAllEndings())
        {
            Debug.Log(end.name + "\n" + end.epilogue);
        }*/

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
