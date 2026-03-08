using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [SerializeField] TimeSettingsSO settings;
    [SerializeField] Light Sun;
    [SerializeField] Light Moon;
    [SerializeField] AnimationCurve LightIntensityCurve;
    [SerializeField] float maxSunIntensity = 3f;
    [SerializeField] float maxMoonIntensity = 1.5f;
    [SerializeField] public int dayCounter = 0;
    [SerializeField] Color dayAmbientLight;
    [SerializeField] Color nightAmbientLight;
    [SerializeField] Volume volume;
    
    ColorAdjustments colorAdjustments;

    TimeService timeService;

    public static TimeManager instance;

    public Action OnSunRise;
    public Action OnSunset;
    public Action OnHourChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        timeService = new TimeService(settings);
        //volume.profile.TryGet(out colorAdjustments);
        OnSunRise += UpdateDayCounter;
    }

    private void Update()
    {
        // Bu fonksiyonu simdilik calistirmayalim shaderlar ve isiklandirmalar tam bitince volume falan eklenince buraya geri donus yapilir
        //UpdateLightSetting();
        UpdateDayTime();
        RotateSun();
    }

    void UpdateLightSetting()
    {
        float dotProduct = Vector3.Dot(Sun.transform.forward, Vector3.down);
        Sun.intensity = Mathf.Lerp(0, maxSunIntensity, LightIntensityCurve.Evaluate(dotProduct));
        Moon.intensity = Mathf.Lerp(0, maxMoonIntensity, LightIntensityCurve.Evaluate(dotProduct));
        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, LightIntensityCurve.Evaluate(dotProduct));
    }

    void RotateSun()
    {
        float rotation = timeService.CalculateSunAngle();
        Sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
    }

    void UpdateDayTime()
    {
        timeService.UpdateTime(Time.deltaTime);
    }

    void UpdateDayCounter() => dayCounter++;
}
