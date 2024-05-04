using NLocalization;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LockedDoor : Interactable
{
    static string SavePrefix = "LockedDoor";
    static string LockedAnim = "Locked";
    static string UnlockingAnim = "Unlocking";
    static string UnlockedAnim = "Unlocked";

    [SerializeField] ItemType m_keyItem;
    [SerializeField] string m_saveKey = "";
    [SerializeField] [HideInInspector] bool m_keyGenerated = false;

    bool m_unlocked = false;

    public override bool CanInteract()
    {
        return !m_unlocked;
    }

    public override int GetInteractionTextID()
    {
        if (CanUnlock())
            return Loc.GetTextID("Open");
        else return Loc.GetTextID("Locked");
    }

    public override Vector2 GetOffset()
    {
        return new Vector2(0, 1);
    }

    public override void Interact(GameObject caster)
    {
        if (!CanUnlock())
            return;

        Unlock(false, true);
    }

    public override bool IsInstantInteraction()
    {
        return false;
    }

    [Button("Generate")]
    void GenerateKey()
    {
        m_keyGenerated = true;

        m_saveKey = Guid.NewGuid().ToString("N");
    }

    [OnInspectorGUI]
    void OnInspector()
    {
        if (m_keyGenerated == false)
            GenerateKey();

        var doors = UnityEngine.Object.FindObjectsOfType<LockedDoor>();
        foreach(var d in doors)
        {
            if (d == this)
                continue;

            if(d.m_saveKey == m_saveKey)
            {
                GUILayout.Box("An other door have the same save key");
                break;
            }
        }
    }

    private void Start()
    {
        CheckSaveUnlocked();
        if (m_unlocked)
            Unlock(true, false);
        else Lock();

    }

    void CheckSaveUnlocked()
    {
        m_unlocked = SaveSystem.instance.GetDatas().GetBool(SavePrefix + '/' + m_saveKey + "/lock");
    }

    bool CanUnlock()
    {
        var itemData = new GetInventoryItemEvent(m_keyItem);
        Event<GetInventoryItemEvent>.Broadcast(itemData);

        return itemData.stack >= 1;
    }

    bool ConsumeKey()
    {
        var removeData = new RemoveInventoryItemEvent(m_keyItem, 1);
        Event<RemoveInventoryItemEvent>.Broadcast(removeData);

        return removeData.removedStack > 0;
    }

    void Lock()
    {
        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
            col.enabled = true;

        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(LockedAnim, true), gameObject);
    }

    void Unlock(bool instant, bool consumeKey)
    {
        if(consumeKey)
        {
            if (!ConsumeKey())
                return;
        }

        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
            col.enabled = false;

        if (instant)
            Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(UnlockedAnim, true), gameObject);
        else
        {
            Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(UnlockingAnim), gameObject);
            Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(UnlockedAnim, true, true), gameObject);

            m_unlocked = true;
            SaveSystem.instance.GetDatas().Set(SavePrefix + '/' + m_saveKey + "/lock", m_unlocked);
        }
    }
}
