using System;
using UnityEngine;

public class GlobalVersionController : MonoBehaviour
{
    public static GlobalVersionController instance = null;

    public bool restricted = true;
    public Ending ending = null;

    public static bool IsRestricted()
    {
        return instance.restricted;
    }

    public static void SetRestricted(bool r)
    {
        instance.restricted = r;
    }

    public static void SetEnding(Ending e)
    {
        instance.ending = e;
    }

    public static Ending GetEnding()
    {
        return instance.ending;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
