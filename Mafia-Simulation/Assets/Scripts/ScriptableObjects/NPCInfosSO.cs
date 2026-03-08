using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCInfosSO", menuName = "Scriptable Objects/NPCInfosSO")]
public class NPCInfosSO : ScriptableObject
{
    [Header("Identity")]
    public string npcID;
    public string conversationTitle;

    [Header("Approach Days")]
    [Tooltip("The NPC will walk to the player on these days. Must match the day conditions you set in DS.")]
    public List<int> approachOnDays;

    [Header("Setting")]
    public int startingPoints = 0;
    public int minPoints = -100;
    public int maxPoints = 100;

    public bool ShouldApproachOnDay(int day)
    {
        return approachOnDays.Contains(day);
    }
}
