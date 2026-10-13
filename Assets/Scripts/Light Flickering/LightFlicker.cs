using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] Light pointLight;
    [SerializeField] float maxIntensity = 2f;
    [SerializeField] float startOnTime = 5f;
    [SerializeField] float startOffTime = 1f;
    [SerializeField] float endOnTime = 1f;
    [SerializeField] float endOffTime = 0.5f;

    float randomOnTime = 5f;
    float randomOffTime = 0.1f;

    bool isLightOn = true;

    float currentTime = 0;

    void Start()
    {
        if(pointLight != null)
        {
            pointLight.intensity = maxIntensity;
        }
        else
        {
            Debug.Log("assign point light in inspector");
        }

        randomOffTime = startOffTime;
        randomOnTime = startOnTime;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        SimpleFlicker();
    }

    void SimpleFlicker()
    {
        if(isLightOn)
        {
            if(currentTime >= randomOnTime)
            {
                //turn off light
                randomOffTime = Random.Range(endOffTime, startOffTime);
                ToggleLight();
            }
        }
        else
        {
            if(currentTime >= randomOffTime)
            {
                //turn on light
                randomOnTime = Random.Range(endOnTime, startOnTime);
                ToggleLight();
            }
        }
    }

    void ToggleLight()
    {
        currentTime = 0;
        if(isLightOn)
        {
            isLightOn = false;
            pointLight.intensity = maxIntensity;
        }
        else
        {
            isLightOn = true;
            pointLight.intensity = 0;
        }

        pointLight.intensity = isLightOn ? maxIntensity : 0;
    }
}
