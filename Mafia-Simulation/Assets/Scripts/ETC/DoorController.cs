using UnityEngine;

public class DoorController : MonoBehaviour
{
    public NPCInfosSO info;

    void Start()
    {
        TimeManager.instance.OnSunRise += CheckDoor;
        CheckDoor();
    }

    void OnDestroy()
    {
        TimeManager.instance.OnSunRise -= CheckDoor;
    }

    void CheckDoor()
    {
        int today = TimeManager.instance.dayCounter;
        if (info.approachOnDays.Contains(today))
            OpenDoor();
        else
            CloseDoor();
    }

    void OpenDoor()
    {
        gameObject.SetActive(false);
    }

    void CloseDoor()
    {
        gameObject.SetActive(true);
    }
}
