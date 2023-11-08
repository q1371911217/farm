using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class gggg : MonoBehaviour
{
    [MenuItem("Tools/Clear")]
    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }
}
