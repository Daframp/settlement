using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class Planet : MonoBehaviour {

    [Range(2,256)]
    public int resolution = 150;
    public bool autoUpdate = true;
    PlanetSettings planetSettings = new();
    //Allows MeshFilters to be saved in inspector
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    ColourGenerator colourGenerator = new ColourGenerator();
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
        colourGenerator.UpdateSettings(planetSettings);
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

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = planetSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i], ref planetSettings);
        }
    }

    void RandomizeValues(){
        planetSettings.planetMaterial = new Material(Resources.Load("planetShader", typeof(Shader)) as Shader);
        planetSettings.planetColour = Random.ColorHSV(0f,1f,0.5f,1f,0.2f,1f,1f,1f);
        planetSettings.radius = Random.Range(1f,3f);
        planetSettings.simpleNoiseScale = Random.Range(0.5f, 1.0f);
        planetSettings.ridgeNoiseScale = Random.Range(0.5f, 1f);
        planetSettings.simpleNoiseStrength = Random.Range(0.2f, 0.4f);
        planetSettings.ridgeNoiseScale = Random.Range(0.2f,0.4f);
        planetSettings.noiseOffset.Set(Random.Range(-255, 255), Random.Range(-255, 255), Random.Range(-255, 255));
    }

    void CreateGradient()
    {
        GradientColorKey[] gck = new GradientColorKey[5];
        GradientAlphaKey[] gak = new GradientAlphaKey[5];

        gck[0].color = Color.blue;
        gck[0].time = -1;
        gak[0].time = -1;
        gak[0].alpha = 0.0F;

        gck[1].color = new Color32(253,238,115, 1); //sandy yellow
        gck[1].time = 0f;
        gak[1].time = 0f;
        gak[1].alpha = 0.0F;

        gck[2].color = new Color32(205,220,57, 1); //grassy light green
        gck[2].time = 0.1f;
        gak[2].time = 0.1f;
        gak[2].alpha = 0.0F;

        gck[3].color = Color.green;
        gck[3].time = 0.4f;
        gak[3].time = 0.4f;
        gak[3].alpha = 0.0F;

        gck[4].color = Color.grey;
        gck[4].time = 0.9f;
        gak[4].time = 0.9f;
        gak[4].alpha = 0.0F;
        planetSettings.gradient.SetKeys(gck, gak);
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

        colourGenerator.UpdateElevation(planetSettings.radius, planetSettings.maxElevation);
    }

    void GenerateColours()
    {
        CreateGradient();
        colourGenerator.UpdateColours();
    }
}