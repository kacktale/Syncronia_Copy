using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class GeneralSceneManager : MonoBehaviour
{
    public static GeneralSceneManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
