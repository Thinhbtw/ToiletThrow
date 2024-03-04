using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance => _instance;
    protected void Awake()
    {
        if (Instance == null)
        {
            _instance = this.GetComponent<T>();
        }
        return;

    }
}