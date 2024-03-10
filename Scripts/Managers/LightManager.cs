using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public Light2D globalLight;
    List<LightSource> lights;

    public void Start()
    {
        lights = new();
    }
    void Update()
    {
        bool en = false;
        
        foreach (var light in lights) 
        { 
            if (light.Time != 0)
            {
                if (light.Time <= Time.time)
                {
                    if (light.ChangeIntensity(Time.deltaTime))
                    {
                        light.time = 0;
                    }
                }

                en = true;
            }
        }
        if (!en)
            enabled = false;
    }
    public void Register(GameObject light, string s)
    {
        int index = int.Parse(s);
        lights.Add(new LightSource(light, index));
        //Debug.Log("Registred light " + light.name);
    }
    public void LightTypeTurn(float time, LightType type, bool on = true)
    {
        //Debug.Log("Zapinanie svetiel");
        foreach (LightSource l in lights)
            if (l.type == type)
            {
                l.Time = Time.time + time;
                l.turn = on;
            }
        enabled = true;
    }
    public void Remove(Light2D light)
    {
        for (int i = 0; i < lights.Count; i++) 
        { 
            if (lights[i].GetLight2D() == light)
            {
                lights.RemoveAt(i); 
                break;
            }
        }
    }
    private class LightSource
    {
        private readonly SpriteRenderer flameSprite;
        private readonly Light2D light2D;
        private readonly float turnTime;
        public readonly LightType type;
        public readonly int number;
        public float Time
        {
            get { return time; }
            set { time = value + number / turnTime; }
        }
        public float time;
        private int Turn
        {
            get
            {
                if (turn)
                    return 1;
                else
                    return -1;
            }
            set
            {
                if (value > 0)
                    turn = true;
                else
                    turn = false;
            }
        }
        public bool turn;

        public LightSource(GameObject torch, int index)
        {
            number = index;

            flameSprite = torch.GetComponent<SpriteRenderer>();

            light2D = torch.GetComponent<Light2D>();

            if (torch.name.Contains("Bos"))
            {
                flameSprite.enabled = false;
                light2D.enabled = false;
                type = LightType.Boss;
                turnTime = 10f;
                turn = false;
            }
            else
            {
                type = LightType.Normal;
                turnTime = 0;
            }
            Light(turn);
            time = 0;
        }
        public void Light(bool turn)
        {
            flameSprite.enabled = turn;
            light2D.enabled = turn;
            this.turn = turn;
            if (!turn)
                SetIntensity(0);
        }
        public void SetIntensity(float intensity)
        {
            light2D.intensity = intensity;
        }
        public bool ChangeIntensity(float deltaIntensity)
        {
            if (light2D.intensity == 0 || 1 == light2D.intensity)
                if (turn)
                    Light(true);
                else
                    Light(false);

            light2D.intensity += deltaIntensity * turnTime * Turn;

            if (light2D.intensity <= 0)
                Light(false);

            return light2D.intensity >= 1;
        }
        public Light2D GetLight2D()
        {
            return light2D;
        }
    }
    public enum LightType
    {
        Boss, Normal
    }
}
