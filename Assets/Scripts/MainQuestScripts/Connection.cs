using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProgressionVector;
using UnityEngine;

public class Evidence
{
    static int currId = 0;

    public int id;
    public Sprite sprite;

    public string name;
    public string description;

    public Evidence()
    {
        this.id = currId++;/*
        JToken obj = JObject.Parse(File.ReadAllText(FileDirectory.EvidenceJsonFile))[name];
        var p = JObject.Parse(File.ReadAllText(FileDirectory.EvidenceJsonFile)).Properties().Select(x => x.Name).ToList();
        //var tmp = new List<Evidence>();
        foreach (var i in p)
            Debug.Log(i);
            //tmp.Add(JsonConvert.DeserializeObject<Evidence>(i));
        this.name = obj["name"].ToString();
        this.description = obj["description"].ToString();*/
    }
}
public class Ending : IComparable
{
    static int currID = 0;

    public int id;

    public string name;
    public string epilogue;
    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        else return CompareTo(obj);
    }

    public Ending()
    {
        id = currID++;
        /*add proper parsint + add indexing for parsing weights array from connections
        this.name = name;
        JToken obj = JObject.Parse(File.ReadAllText(FileDirectory.EpiloguesFile))[name];
        this.epilogue = obj["epilogue"].ToString();*/
    }
}
public class Connection
{
    static int currID = 0;

    public int id;

    public int evidence1ID;
    public int evidence2ID;

    public string commonDescription;

    public Dictionary<Option, bool> options;

    public Connection(JsonConnection jsonConnection, MQEndings mqEndings)
    {
        id = currID++;
        options = new Dictionary<Option, bool>();

        this.evidence1ID = jsonConnection.evidence1ID;
        this.evidence2ID = jsonConnection.evidence2ID;

        commonDescription = jsonConnection.description;
        foreach (var joption in jsonConnection.options)
        {
            options.Add(new Option(joption, mqEndings), false);
        }
    }
}

public class Option : PV_Action<Ending>
{
    public string description;

    public Option(Dictionary<Ending, int> weights) : base(weights)
    { }

    public Option (JsonOption joption, MQEndings mQEndings) : base()
    {
        this.weights = new Dictionary<Ending, int>();
        this.description = joption.description;
        this.PV_name = joption.name;
        for (var i = 0; i < joption.weights.Length; i++)
        {
            this.weights.Add(mQEndings.endingsById[i], joption.weights[i]);
        }
    }
}

public class JsonConnection
{
    public int evidence1ID;
    public int evidence2ID;
    public string description;
    public JsonOption[] options;
}

public class JsonOption
{
    public string name;
    public string description;
    public int[] weights;
}