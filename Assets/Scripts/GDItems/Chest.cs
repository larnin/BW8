using NLocalization;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Chest : Interactable
{
    static string SavePrefix = "Chest";
    static string LockedAnim = "Locked";
    static string UnlockingAnim = "Unlocking";
    static string UnlockedAnim = "Unlocked";

    [SerializeField] bool m_needKey = false;
    [ShowIf("m_needKey")]
    [SerializeField] ItemType m_keyItem;
    [SerializeField] string m_saveKey = "";
    [SerializeField] [HideInInspector] bool m_keyGenerated = false;
    [SerializeField] ItemType m_lootItem;
    [SerializeField] int m_lootStack;

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

        var chests = UnityEngine.Object.FindObjectsOfType<Chest>();
        foreach (var c in chests)
        {
            if (c == this)
                continue;

            if (c.m_saveKey == m_saveKey)
            {
                GUILayout.Box("An other chest have the same save key");
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
        if (!m_needKey)
            return true;

        var itemData = new GetInventoryItemEvent(m_keyItem);
        Event<GetInventoryItemEvent>.Broadcast(itemData);

        return itemData.stack >= 1;
    }

    bool ConsumeKey()
    {
        if (!m_needKey)
            return true;

        var removeData = new RemoveInventoryItemEvent(m_keyItem, 1);
        Event<RemoveInventoryItemEvent>.Broadcast(removeData);

        return removeData.removedStack > 0;
    }

    void Lock()
    {
        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(LockedAnim, true), gameObject);
    }

    void Unlock(bool instant, bool consumeKey)
    {
        if (consumeKey)
        {
            if (!ConsumeKey())
                return;

            Event<AddInventoryItemEvent>.Broadcast(new AddInventoryItemEvent(m_lootItem, m_lootStack));
        }

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
