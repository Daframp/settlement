using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;
using UnityEngine;

public class ColourGenerator
{
    PlanetSettings planetSettings;
    Texture2D texture;
    const int textureResolution = 50;

    public void UpdateSettings(PlanetSettings planetSettings)
    {
        this.planetSettings = planetSettings;
        if (texture == null)
        {
        texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(float minElevation, float maxElevation)
    {
        planetSettings.planetMaterial.SetVector("_elevationMinMax", new Vector4(minElevation, maxElevation));
    }

    public void UpdateColours()
    {
        Color[] colours = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++)
        {
            colours[i] = planetSettings.gradient.Evaluate(i / (textureResolution - 1f));
        }
        texture.SetPixels(colours);
        texture.Apply();
        planetSettings.planetMaterial.SetTexture("_texture", texture);
    }
}
