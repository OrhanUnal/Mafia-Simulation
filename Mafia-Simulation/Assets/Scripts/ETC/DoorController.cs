using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public NPCInfosSO info;
    private DoorState _currentState = DoorState.Closed;
    private const float AnimationDuration = 1.5f;

    [SerializeField] private Transform _hingeTransform;

    private enum DoorState
    {
        Closed = -1,
        InAnimation,
        Open = 1
    }

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
        int direction;
        int today = TimeManager.instance.dayCounter;
        if (info.approachOnDays.Contains(today) && _currentState == DoorState.Closed)
            direction = 1;
        else if (_currentState == DoorState.Open)
            direction = -1;
        else return;
        StartCoroutine(AnimateDoor(direction, (DoorState)direction));
    }

    private IEnumerator AnimateDoor(int direction, DoorState targetState)
    {
        _currentState = DoorState.InAnimation;

        Quaternion startRotation = _hingeTransform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 90f * direction, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < AnimationDuration)
        {
            _hingeTransform.rotation = Quaternion.Slerp(
                startRotation,
                endRotation,
                elapsedTime / AnimationDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _hingeTransform.rotation = endRotation;
        _currentState = targetState;
    }
}
