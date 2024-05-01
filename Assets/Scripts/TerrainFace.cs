using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainFace {
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    PlanetSettings planetSettings;
    Vector3 axisA;
    Vector3 axisB;
    Noise noise = new();

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp, ref PlanetSettings planetSettings)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.planetSettings = planetSettings;


        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                //percents are adjusted to -1 to 1 with (0.5f * 2)
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;

                Vector3 pointOnPlanet = pointOnUnitCube.normalized;
                float noiseSum = 0f;

                noiseSum += simpleNoise(ref pointOnPlanet);
                noiseSum += rigidNoise(ref pointOnPlanet);

                float elevation = Mathf.Max(0, noiseSum);
                pointOnPlanet = pointOnPlanet * (planetSettings.radius + elevation);
                planetSettings.maxElevation = Mathf.Max(planetSettings.maxElevation, planetSettings.radius + elevation);

                vertices[i] = pointOnPlanet;

                
                //define both triangles from current vertex clockwise
                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private float simpleNoise(ref Vector3 pointOnPlanet) 
    {
        float noiseSum = 0f;
                for (int layer = 0; layer < 7; layer++)
                {
                    float noiseValue = noise.Evaluate(pointOnPlanet * planetSettings.simpleNoiseScale * Mathf.Pow(2, layer -1)+ planetSettings.noiseOffset);
                    noiseValue *= planetSettings.simpleNoiseStrength / Mathf.Pow(2, layer - 1);;
                    // divide by five for appropriate size. * radius / 3 gives a percent of maximum radius
                    noiseSum += noiseValue / 5 * planetSettings.radius / 3;
                }
                
                return noiseSum;
    }
    private float rigidNoise(ref Vector3 pointOnPlanet)
    {
        float noiseSum = 0f;
        float weight = 1;
                for (int layer = 0; layer < 5; layer++)
                {
                    float noiseValue = 1-Mathf.Abs(noise.Evaluate(pointOnPlanet * planetSettings.ridgeNoiseScale * Mathf.Pow(2, layer -1)+ planetSettings.noiseOffset));
                    noiseValue *= planetSettings.rigidNoiseStrength;
                    noiseValue *= noiseValue;
                    noiseValue *= weight;
                    weight = noiseValue;
                    // divide by five for appropriate size. * radius / 3 gives a percent of maximum radius
                    noiseSum += noiseValue / 5 * planetSettings.radius / 3;
                }
              
                return noiseSum;
    }
}