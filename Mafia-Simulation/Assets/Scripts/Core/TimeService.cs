using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeService
{
    readonly TimeSettingsSO timeSettingsSO;
    readonly TimeSpan sunriseTime;
    readonly TimeSpan sunsetTime;
    
    Observable<bool> isDayTime;
    Observable<int> currentHour;
    
    DateTime currentTime;

    public event Action OnSunRise;
    public event Action OnSunset;
    public event Action OnHourChanged;

    public TimeService  (TimeSettingsSO timeSettingsSO)
    {
        this.timeSettingsSO = timeSettingsSO;
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(timeSettingsSO.startHour);
        sunriseTime = TimeSpan.FromHours(timeSettingsSO.sunRiseHour);
        sunsetTime = TimeSpan.FromHours(timeSettingsSO.sunSetHour);

        isDayTime = new Observable<bool> (IsDayTime());
        currentHour = new Observable<int> (currentTime.Hour);

        isDayTime.onValueChanged += day => (day ? OnSunRise : OnSunset)?.Invoke();
        currentHour.onValueChanged += _ => OnHourChanged?.Invoke();
    }

    public void UpdateTime(float deltaTime)
    {
        if (IsDayTime())
        {
            currentTime = currentTime.AddSeconds(deltaTime * timeSettingsSO.timeMultiplier);
        }
        isDayTime.value = IsDayTime();
        currentHour.value = currentTime.Hour;
    }

    public float CalculateSunAngle() 
    {
        bool isDayTime = IsDayTime();
        float startDegree = isDayTime ? 0 : 180;
        TimeSpan start = isDayTime ? sunriseTime : sunsetTime;
        TimeSpan end = isDayTime ? sunsetTime : sunriseTime;

        TimeSpan totalTime = CalculateDifference(start, end);
        TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
        return Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);
    }

    public DateTime CurrentTime() => currentTime;

    public bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }
}
