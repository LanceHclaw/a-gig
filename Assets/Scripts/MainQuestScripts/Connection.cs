using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProgressionVector;
using UnityEngine;

public class Evidence
{
    public Sprite sprite;

    public string name;
    public string description;

    public Evidence(string name)
    {
        JToken obj = JObject.Parse(File.ReadAllText(FileDirectory.EvidenceJsonFile))[name];
        this.name = obj["name"].ToString();
        this.description = obj["description"].ToString();
    }
}
public class Ending : IComparable
{
    public string epilogue;
    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        else return CompareTo(obj);
    }
}
public class Connection
{
    Evidence left;
    Evidence right;

    public string commonDescription;

    Dictionary<Option, bool> options;

    public Connection(Evidence left, Evidence right)
    {
        this.left = left;
        this.right = right;
    }
}

public class Option : PV_Action<Ending>
{
    public string description;

    public Option(Dictionary<Ending, int> weights) : base(weights)
    { }
}
