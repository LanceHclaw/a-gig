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

    public Evidence(string name)
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
    public string name;
    public string epilogue;
    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        else return CompareTo(obj);
    }

    public Ending(string name)
    {
        //add proper parsint + add indexing for parsing weights array from connections
        this.name = name;
        JToken obj = JObject.Parse(File.ReadAllText(FileDirectory.EpiloguesFile))[name];
        this.epilogue = obj["epilogue"].ToString();
    }
}
public class Connection
{
    public string commonDescription;

    Dictionary<Option, bool> options;

    public Connection()
    {
    }
}

public class Option : PV_Action<Ending>
{
    public string description;

    public Option(Dictionary<Ending, int> weights) : base(weights)
    { }
}