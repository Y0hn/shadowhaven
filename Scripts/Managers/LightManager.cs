using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    List<LightSource> lights;
    void Start()
    {
        lights = new();
    }
    public void Register(GameObject light, string s)
    {
        int index = int.Parse(s);
        lights.Add(new LightSource(light, index));
        Debug.Log("Registred light " + light.name);
    }
    public void BossLightsTurnOn(int index)
    {
        foreach (LightSource light in lights)
            if (light.type == LightType.Boss && light.number == index)
                light.Light(true);
    }
    private class LightSource
    {
        public readonly LightType type;
        public readonly int number;
        private readonly SpriteRenderer flameSprite;
        private readonly Light2D light2D;
        public LightSource(GameObject torch, int index)
        {
            number = index;

            flameSprite = torch.GetComponent<SpriteRenderer>();

            light2D = torch.GetComponent<Light2D>();

            if (torch.name.Contains("Bos"))
            {
                type = LightType.Boss;
                light2D.enabled = false;
                flameSprite.enabled = false;
            }
            else
            {
                type = LightType.Normal;
            }
        }
        public void Light(bool turn)
        {
            light2D.enabled = turn;
            flameSprite.enabled = turn;
        }
    }
    private enum LightType
    {
        Boss, Normal
    }
}
