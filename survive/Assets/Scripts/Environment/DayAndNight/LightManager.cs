using UnityEngine;

[ExecuteAlways]
public class LightManager : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;

    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField, Range(0.001f, 1)] private float DayAndNightModifier;

    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            TimeOfDay += (Time.deltaTime * DayAndNightModifier);
            TimeOfDay %= 24;
        }

        UpdateLighting(TimeOfDay / 24f);
    }


    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            if (TimeOfDay >= 0.0f && TimeOfDay <= 6.0f)
                DirectionalLight.intensity = 0.1f;
            else if (TimeOfDay >= 6.0f && TimeOfDay <= 11.0f)
                DirectionalLight.intensity = 0.5f;
            else if (TimeOfDay >= 11.0f && TimeOfDay <= 15.0f)
                DirectionalLight.intensity = 1.0f;
            else if (TimeOfDay >= 15.0f && TimeOfDay <= 20.0f)
                DirectionalLight.intensity = 0.4f;
            else if (TimeOfDay >= 20.0f && TimeOfDay <= 24.0f)
                DirectionalLight.intensity = 0.0f;

            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0f));
        }
    }

    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            var lights = GameObject.FindObjectsOfType<Light>();

            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                }
            }
        }
    }
}