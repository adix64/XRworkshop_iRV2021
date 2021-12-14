using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Google.XR.Cardboard;
public class DirtyFix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("myFloatVariableName", 3.14f);
        float someSavedFloat = PlayerPrefs.GetFloat("myFloatVariableName");

        PlayerPrefs.SetString("myVariableName", "bla bla bla TOPSECRET HERE");
        string someSavedString = PlayerPrefs.GetString("myVariableName");

        PlayerPrefs.SetInt("myVariableName", 42);
        int someSavedInt = PlayerPrefs.GetInt("myVariableName");

    }

    // Update is called once per frame
    void Update()
    {
        var skyboxMaterial = RenderSettings.skybox;
        skyboxMaterial.SetFloat("_Rotation", Mathf.Sin(Time.time) * 180 + 180);
    }
}
