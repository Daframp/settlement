using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    GameObject planet;
    public Shader planetShader;
    void Start()
    {
        GenerateNewPlanet();
    }

    // Update is called once per frame
    public void GenerateNewPlanet()
    {   
        if (planet != null)
        {
            Destroy(planet);
        }
        //spawn object
        planet = new GameObject("planet");
        //Add Components
        planet.AddComponent<MeshRenderer>();
        planet.AddComponent<Planet>();
        planet.AddComponent<MeshFilter>();
        planet.AddComponent<SphereCollider>();
    }
}
