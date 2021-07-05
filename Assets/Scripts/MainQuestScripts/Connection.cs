using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProgressionVector;
using UnityEngine;

public class Evidence : PV_Action<Ending>
{
    static int currId = 0;

    public int id;
    public Sprite sprite;

    public string name;
    public string description;

    public Evidence()
    {
        this.id = currId++;
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

    /// <summary>
    /// Creates default "makes no sense" connection.
    /// </summary>
    public Connection()
    {
        this.commonDescription = "I don't see a connection here.";
        this.id = currID++;
        this.options = new Dictionary<Option, bool>();
    }
}

public class Option : PV_Action<Ending>
{
    public string choiceDescription;
    public string snippet;

    public Option(Dictionary<Ending, int> weights) : base(weights)
    { }

    public Option (JsonOption joption, MQEndings mQEndings) : base()
    {
        this.weights = new Dictionary<Ending, int>();
        this.choiceDescription = joption.choiceDescription;
        this.snippet = joption.snippet;
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
    public string choiceDescription;
    public string snippet;
    public int[] weights;
}