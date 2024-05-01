using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    PlanetSettings planetSettings;
    Vector3 axisA;
    Vector3 axisB;
    Noise noise = new();
    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp, PlanetSettings planetSettings)
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

                // Vector3 pointOnUnitSphere = pointOnUnitCube.normalized ;
                // Vector3 pointOnPlanet = pointOnUnitSphere * planetSettings.radius * planetSettings.minRadiusPercent;
                // if (noise.Evaluate(pointOnUnitSphere * planetSettings.noiseScale + planetSettings.noiseOffset) * planetSettings.noiseStrength > planetSettings.minRadiusPercent)
                // {
                //     pointOnPlanet = pointOnUnitSphere * planetSettings.radius *  noise.Evaluate(pointOnUnitSphere * planetSettings.noiseScale + planetSettings.noiseOffset)  * planetSettings.noiseStrength;
                // }
                // vertices[i] = pointOnPlanet;

                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized ;
                Vector3 pointOnPlanet = pointOnUnitSphere * planetSettings.radius * Math.Max(0.8f, noise.Evaluate(pointOnUnitSphere * planetSettings.noiseScale + planetSettings.noiseOffset) * planetSettings.noiseStrength);
                
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
}