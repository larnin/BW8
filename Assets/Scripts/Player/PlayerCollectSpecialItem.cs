using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCollectSpecialItem : MonoBehaviour
{
    const string startAnim = "Display_Item_Start";
    const string loopAnim = "Display_Item_Loop";
    const string endAnim = "Display_Item_End";

    [Serializable]
    class ItemDialog
    {
        public ItemType item;
        public DialogObject dialog;
    }

    enum State
    {
        disabled,
        start,
        loop,
        end
    }

    [SerializeField] List<ItemDialog> m_itemTexts = new List<ItemDialog>();
    [SerializeField] GameObject m_displayItemPrefab = null;

    SubscriberList m_subscriberList = new SubscriberList();

    ItemType m_currentItem;
    State m_state = State.disabled;
    float m_timer = 0;

    GameObject m_displayObject = null;

    private void Awake()
    {
        m_subscriberList.Add(new Event<DisplaySpecialItemEvent>.Subscriber(StartDisplay)); 
        m_subscriberList.Add(new Event<GetStatusEvent>.LocalSubscriber(GetStatus, gameObject));
        m_subscriberList.Add(new Event<GetOffsetVelocityEvent>.LocalSubscriber(GetVelocity, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void StartDisplay(DisplaySpecialItemEvent e)
    {
        if (m_state != State.disabled)
            return;

        var dialog = m_itemTexts.Find(x => { return x.item == e.item; });
        if (dialog == null)
            return;

        m_currentItem = e.item;

        m_state = State.start;

        GetAnimationDurationEvent duration = new GetAnimationDurationEvent(startAnim);
        Event<GetAnimationDurationEvent>.Broadcast(duration, gameObject);
        m_timer = duration.duration;

        DialogPopup.StartDialog(dialog.dialog);

        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(startAnim, 5), gameObject);
        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(loopAnim, 5, true, true));
    }

    private void Update()
    {
        if (m_state == State.disabled)
            return;

        m_timer -= Time.deltaTime;

        switch(m_state)
        {
            case State.start:
                {
                    if (m_timer <= 0)
                    {
                        m_state = State.loop;
                        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(loopAnim, 5, true));
                        InstantiateDisplayObject();
                    }
                    break;
                }
            case State.loop:
                {
                    if(!DialogPopup.IsOpened())
                    {
                        m_state = State.end;
                        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(endAnim, 5), gameObject);

                        GetAnimationDurationEvent duration = new GetAnimationDurationEvent(endAnim);
                        Event<GetAnimationDurationEvent>.Broadcast(duration, gameObject);
                        m_timer = duration.duration;

                        if (m_displayObject != null)
                            Destroy(m_displayObject);
                    }

                    break;
                }
            case State.end:
                {
                    if(m_timer <= 0)
                    {
                        m_state = State.disabled;
                    }
                    break;
                }
        }
    }

    void GetStatus(GetStatusEvent e)
    {
        if(m_state != State.disabled)
            e.lockActions = true;
    }

    void GetVelocity(GetOffsetVelocityEvent e)
    {
        if(m_state != State.disabled)
        {
            e.offsetVelocity = Vector2.zero;
            e.velocityMultiplier = 0;
        }
    }

    void InstantiateDisplayObject()
    {
        if (m_displayItemPrefab == null)
            return;

        var sprite = World.lootType.GetSprite(m_currentItem);
        if (sprite == null)
            return;

        var obj = Instantiate(m_displayItemPrefab);
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;

        var itemElt = obj.transform.Find("Item");
        if(itemElt != null)
        {
            var render = itemElt.GetComponent<SpriteRenderer>();
            if (render != null)
                render.sprite = sprite;
        }

        m_displayObject = obj;
    }
}
