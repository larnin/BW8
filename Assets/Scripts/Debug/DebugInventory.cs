using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DebugInventory : MonoBehaviour
{

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2 m_scrollPos = Vector2.zero;
    bool[] m_selected;

    private void Awake()
    {
        m_selected = new bool[Enum.GetValues(typeof(EquipementSlot)).Length];

        m_subscriberList.Add(new Event<DrawDebugWindowEvent>.Subscriber(DebugDraw));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void DebugDraw(DrawDebugWindowEvent e)
    {
        if (e.type != DebugWindowType.Window_Inventory)
            return;

        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);

        foreach(var slot in (EquipementSlot[]) Enum.GetValues(typeof(EquipementSlot)))
        {
            GetEquipementEvent slotData = new GetEquipementEvent(slot);
            Event<GetEquipementEvent>.Broadcast(slotData);

            GUILayout.BeginHorizontal();
            GUILayout.Label(slot.ToString() + ": ");

            int index = 0;
            if(!slotData.IsEmpty())
                index = (int)(slotData.GetItem()) + 1;

            DebugPopupData[] windowsDatas = new DebugPopupData[Enum.GetValues(typeof(ItemType)).Length + 1];
            for(int i = 0; i < windowsDatas.Length; i++)
            {
                if (i == 0)
                    windowsDatas[i] = new DebugPopupData("Empty");
                else windowsDatas[i] = new DebugPopupData(((ItemType)(i - 1)).ToString());
            }

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x += rect.width / 2;
            rect.width -= rect.width / 2;

            int newIndex = DebugLayout.DrawPopup(rect, ref m_selected[(int)slot], windowsDatas[index].name, windowsDatas);
            if(newIndex == 0)
            {
                DeleteEquipementEvent unequipEvent = new DeleteEquipementEvent(slot);
                Event<DeleteEquipementEvent>.Broadcast(unequipEvent);
            }
            else if(newIndex > 0)
            {
                SetEquipementEvent equipData = new SetEquipementEvent(slot, (ItemType)(newIndex - 1));
                Event<SetEquipementEvent>.Broadcast(equipData);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
}
