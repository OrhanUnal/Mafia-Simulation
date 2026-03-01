using UnityEngine;

[CreateAssetMenu(fileName = "TimeSettingsSO", menuName = "Scriptable Objects/TimeSettingsSO")]
public class TimeSettingsSO : ScriptableObject
{
    public float timeMultiplier = 2000f;
    public float startHour = 12f;
    public float sunSetHour = 18f;
    public float sunRiseHour = 6f;
}
