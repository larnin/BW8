using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum EquipementSlot
{
    LeftHand,
    RightHand,
}

public class PlayerEquipement : MonoBehaviour
{
    ItemType?[] m_equipement;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_equipement = new ItemType?[Enum.GetValues(typeof(EquipementSlot)).Length];

        m_subscriberList.Add(new Event<SetEquipementEvent>.Subscriber(SetEquipement));
        m_subscriberList.Add(new Event<DeleteEquipementEvent>.Subscriber(DeleteEquipement));
        m_subscriberList.Add(new Event<EquipItemEvent>.Subscriber(EquipItem));
        m_subscriberList.Add(new Event<UnequipItemEvent>.Subscriber(UnequipItem));
        m_subscriberList.Add(new Event<GetEquipementEvent>.Subscriber(GetEquipement));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Start()
    {
        SetDefaultEquipement();
    }

    void SetDefaultEquipement()
    {
        if(m_equipement == null)
            m_equipement = new ItemType?[Enum.GetValues(typeof(EquipementSlot)).Length];

        m_equipement[(int)EquipementSlot.LeftHand] = ItemType.Sword;
        m_equipement[(int)EquipementSlot.RightHand] = ItemType.Vacuum;
    }

    void SetEquipement(SetEquipementEvent e)
    {
        if (e.oldToInventory)
            AddToInventory(m_equipement[(int)e.slot]);
        m_equipement[(int)e.slot] = e.itemType;
    }

    void DeleteEquipement(DeleteEquipementEvent e)
    {
        m_equipement[(int)e.slot] = null;
    }

    void EquipItem(EquipItemEvent e)
    {
        AddToInventory(m_equipement[(int)e.slot]);
        m_equipement[(int)e.slot] = GetFromInventory(e.inventorySlot);
    }

    void UnequipItem(UnequipItemEvent e)
    {
        AddToInventory(m_equipement[(int)e.slot]);
        m_equipement[(int)e.slot] = null;
    }

    void GetEquipement(GetEquipementEvent e)
    {
        e.SetItem(m_equipement[(int)e.slot]);
    }

    void AddToInventory(ItemType? type)
    {
        if (type == null)
            return;

        AddInventoryItemEvent item = new AddInventoryItemEvent(type.Value, 1);
        Event<AddInventoryItemEvent>.Broadcast(item);
    }

    ItemType? GetFromInventory(int index)
    {
        GetIventorySlotEvent item = new GetIventorySlotEvent(index);

        Event<GetIventorySlotEvent>.Broadcast(item);

        if (item.stack == 0)
            return null;

        RemoveInventorySlotEvent removeItem = new RemoveInventorySlotEvent(index, 1);
        Event<RemoveInventorySlotEvent>.Broadcast(removeItem);

        return item.type;
    }

    [OnInspectorGUI]
    void DisplayEquipement()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
            SetDefaultEquipement();

        int nbEquipement = Enum.GetValues(typeof(EquipementSlot)).Length;

        GUILayout.Label("Equipement:");
        GUI.enabled = false;
        for (int i = 0; i < nbEquipement; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(((EquipementSlot)i).ToString());
            if (m_equipement[i] == null)
                GUILayout.TextField("No item");
            else GUILayout.TextField(m_equipement[i].ToString());
            
            GUILayout.EndHorizontal();
        }
        GUI.enabled = true;
#endif
    }
}
