using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

public class RelationSystemManager : MonoBehaviour
{
    [Tooltip("Every NPC that occurs in game has a Scriptable Object which should appear in this list.")]
    public List<NPCInfosSO> listOfNPC;

    public static RelationSystemManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAllNPCs();
        RegisterLuaFunctions();
        
    }

    private void OnDestroy()
    {
        UnregisterLuaFunctions();
    }

    private void InitializeAllNPCs()
    {
        foreach (NPCInfosSO npc in listOfNPC)
        {
            string nameOfNPC = $"Relation_{npc.npcID}";
            string raw = DialogueLua.GetVariable(nameOfNPC).asString;
            if (string.IsNullOrEmpty(raw) || raw == "nil")
                DialogueLua.SetVariable(nameOfNPC, npc.startingPoints);
        }
    }

    public int GetPoints(string id) => DialogueLua.GetVariable($"Relation_{id}").asInt;
    public void AddPoints(string id, double amount)
    {
        NPCInfosSO npc = listOfNPC.Find(n => n.npcID == id);
        if (npc == null) return;
        int current = GetPoints(id);
        int next = Mathf.Clamp(current + (int)amount, npc.minPoints, npc.maxPoints);
        DialogueLua.SetVariable($"Relation_{id}", next);
    }

    public void ReducePoints(string id, double amount)
    {
        AddPoints(id, -Mathf.Abs((int)amount));
    }

    public void SetPoints(string id, double value)
    {
        NPCInfosSO npc = listOfNPC.Find(n => n.npcID == id);
        if (npc == null) return;
        int clamped = Mathf.Clamp((int)value, npc.minPoints, npc.maxPoints);
        DialogueLua.SetVariable($"Relation_{id}", clamped);
    }

    #region LuaFunctions
    private void RegisterLuaFunctions()
    {
        Lua.RegisterFunction(
            "GetRelationPoints",
            this,
            SymbolExtensions.GetMethodInfo(() => GetPoints(string.Empty))
        );
        Lua.RegisterFunction(
            "AddRelationPoints",
            this,
            SymbolExtensions.GetMethodInfo(() => AddPoints(string.Empty, 0))
        );
        Lua.RegisterFunction(
            "ReduceRelationPoints",
            this,
            SymbolExtensions.GetMethodInfo(() => ReducePoints(string.Empty, 0))
        );
        Lua.RegisterFunction(
            "SetRelationPoints",
            this,
            SymbolExtensions.GetMethodInfo(() => SetPoints(string.Empty, 0))
        );
    }

    private void UnregisterLuaFunctions()
    {
        Lua.UnregisterFunction("GetRelationPoints");
        Lua.UnregisterFunction("AddRelationPoints");
        Lua.UnregisterFunction("ReduceRelationPoints");
        Lua.UnregisterFunction("SetRelationPoints");
    }

    #endregion
    // BASIC SAVE/LOAD (call these from your UI / Save Manager):
    //   PixelCrushers.SaveSystem.SaveToSlot(1);      // Save to slot 1
    //   PixelCrushers.SaveSystem.LoadFromSlot(1);    // Load from slot 1
    //   PixelCrushers.SaveSystem.DeleteSavedGame(1); // Delete slot 1
}