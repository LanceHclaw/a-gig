using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MQEvidence
{
    public static readonly Dictionary<int, Evidence> all_evidence = new Dictionary<int, Evidence>(); 

    public MQEvidence()
    {
        var jobject = JObject.Parse(File.ReadAllText(FileDirectory.EvidenceJsonFile)).Properties();
        var tuples = jobject.Select(x => (x.Name, x.Value.ToString())).ToList();

        foreach (var tuple in tuples)
        {
            Evidence e = JsonConvert.DeserializeObject<Evidence>(tuple.Item2);
            e.sprite = Resources.Load<Sprite>(tuple.Name);
            all_evidence.Add(e.id, e);
        }
    }
}

public class MQEndings
{
    public Dictionary<int, Ending> endingsById = new Dictionary<int, Ending>();

    public MQEndings()
    {
        var jobject = JObject.Parse(File.ReadAllText(FileDirectory.EpiloguesFile)).Properties();
        var tuples = jobject.Select(x => (x.Name, x.Value.ToString())).ToList();

        foreach (var tuple in tuples)
        {
            Ending e = JsonConvert.DeserializeObject<Ending>(tuple.Item2);
            e.name = tuple.Name;
            endingsById.Add(e.id, e);
        }
    }
}

public class MQConnections
{
    public int[,] connectionMatrix;
    public Dictionary<int, Connection> ConnectionsByID = new Dictionary<int, Connection>();

    public MQConnections(string filename, MQEndings mqEndings)
    {
        connectionMatrix = new int[mqEndings.endingsById.Count, mqEndings.endingsById.Count];

        var jobject = JObject.Parse(File.ReadAllText(filename)).Properties();
        var jcvalues = jobject.Select(x => x.Value).ToList()[0];

        foreach (var jcstring in jcvalues)
        {
            var jconnection = JsonConvert.DeserializeObject<JsonConnection>(jcstring.ToString());
            var connection = new Connection(jconnection, mqEndings);
            connectionMatrix[connection.evidence1ID, connection.evidence2ID] = connection.id;
            connectionMatrix[connection.evidence2ID, connection.evidence1ID] = connection.id;

            ConnectionsByID.Add(connection.id, connection);
        }
    }
}
