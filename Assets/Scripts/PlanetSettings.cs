using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlanetSettings
{
    public Gradient gradient = new Gradient();
    public Material planetMaterial;
    public Color planetColour;
    public Color waterColour = Color.blue;
    public Color lowColour = Color.green;
    public Color highColour = Color.grey;
    public float radius = 1f;
    public float maxElevation = 0f;
    public float simpleNoiseScale = 1f;
    public float ridgeNoiseScale = 1f;
    public float simpleNoiseStrength = 1f;
    public float rigidNoiseStrength = 1f;
    public Vector3 noiseOffset = Vector3.zero;

}
