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
    /*
    public static readonly Evidence MagnumShell = new Evidence(nameof(MagnumShell));
    public static readonly Evidence Letter = new Evidence(nameof(Letter));
    public static readonly Evidence Pillow = new Evidence(nameof(Pillow));
    public static readonly Evidence GunInBox = new Evidence(nameof(GunInBox));
    public static readonly Evidence Cigarette = new Evidence(nameof(Cigarette));
    public static readonly Evidence FloorMop = new Evidence(nameof(FloorMop));
    public static readonly Evidence CleanFloor = new Evidence(nameof(CleanFloor));
    public static readonly Evidence OpenWindow = new Evidence(nameof(OpenWindow));
    public static readonly Evidence Dummy = new Evidence(nameof(Dummy));
    public static readonly Evidence PackOfCigarettes = new Evidence(nameof(PackOfCigarettes));
    public static readonly Evidence BulletInBody = new Evidence(nameof(BulletInBody));
    public static readonly Evidence Journal = new Evidence(nameof(Journal));
    public static readonly Evidence Knife = new Evidence(nameof(Knife));
    */

    public static readonly Dictionary<int, Evidence> all_evidence = new Dictionary<int, Evidence>(); 

    public MQEvidence()
    {
        var jobject = JObject.Parse(File.ReadAllText(FileDirectory.EvidenceJsonFile)).Properties();
        var tuples = jobject.Select(x => (x.Name, x.Value.ToString())).ToList();

        foreach (var tuple in tuples)
        {
            Evidence e = JsonConvert.DeserializeObject<Evidence>(tuple.Item2);
            all_evidence.Add(e.id, e);
            //Debug.Log(e.id);
        }
    }
    public static IEnumerable<Evidence> GetAllEvidence()
    {
        return typeof(MQEvidence).GetFields().Select(x => (Evidence)x.GetValue(null));
        //return new List<Evidence> { MagnumShell, Letter, Pillow, GunInBox, Cigarette, FloorMop, CleanFloor,
        //    OpenWindow, Dummy, PackOfCigarettes, BulletInBody, Journal, Knife };
    }
}

public class MQEndings
{
    /*public static readonly Ending Davis = new Ending(nameof(Davis));
    public static readonly Ending Neighbour_Knife = new Ending(nameof(Neighbour_Knife));
    public static readonly Ending Neighbour_Pistol = new Ending(nameof(Neighbour_Pistol));
    public static readonly Ending Suicide = new Ending(nameof(Suicide));
    public static readonly Ending Contradictions = new Ending(nameof(Contradictions));
    public static readonly Ending HeartAttack = new Ending(nameof(HeartAttack));
    public static readonly Ending SlipUp = new Ending(nameof(SlipUp));
    public static readonly Ending WTF = new Ending(nameof(WTF));
    public static readonly Ending Worst = new Ending(nameof(Worst));
    */
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
            //Debug.Log(e.id);
        }
    }

    public static IEnumerable<Ending> GetAllEndings()
    {
        return typeof(MQEndings).GetFields().Select(x => (Ending)x.GetValue(null));
        // return new List<Ending> { Davis, Neighbour_Knife, Neighbour_Pistol, Suicide, Contradictions,
        // HeartAttack, SlipUp, WTF, Worst };
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
            //Evidence e = JsonConvert.DeserializeObject<Evidence>(connection.Item2);
            //all_evidence.Add(e.id, e);
            //Debug.Log(connection.ToString());
        }
    }
}
