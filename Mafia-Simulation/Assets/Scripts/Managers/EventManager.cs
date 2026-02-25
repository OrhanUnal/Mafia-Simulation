using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GetAvailableDialogueList()
    {
        //Buraya SO seklinde diyaloglarin idleri ve yapilip yapilmadiklarini ve gereken conditionlarin saglanip saglanmadiklarini kontrol eden ve bu SO lari liste haline getirip donderen bir sistem gerekiyor
        return;
    }
}
