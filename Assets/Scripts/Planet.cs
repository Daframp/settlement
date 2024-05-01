using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class Planet : MonoBehaviour {

    [Range(2,256)]
    public int resolution = 50;
    public bool autoUpdate = true;
    PlanetSettings planetSettings = new();
    //Allows MeshFilters to be saved in inspector
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;
    
    
    void OnValidate()
    {
        GeneratePlanet();
    }

    void Awake()
    {
        GeneratePlanet();
    }

    void FixedUpdate()
    {
        transform.Rotate(0.02f, 0.06f,0f);
    }

	void Initialize()
    {
        RandomizeValues();

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i], planetSettings);
        }
    }

    void RandomizeValues(){
        planetSettings.planetColour = Random.ColorHSV(0f,1f,0.5f,1f,0.2f,1f,1f,1f);
        planetSettings.radius = Random.Range(1f,5f);
        planetSettings.noiseScale = Random.Range(0.85f, 1.0f);
        planetSettings.noiseOffset.Set(Random.Range(-255, 255), Random.Range(-255, 255), Random.Range(-255, 255));
        planetSettings.noiseStrength = Random.Range(0.5f, 1.5f);
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
        Initialize();
        GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
        Initialize();
        GenerateColours();
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColours()
    {
        foreach(MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = planetSettings.planetColour;
        }
    }
}