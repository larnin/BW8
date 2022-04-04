using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    class PlayingAnimation
    {
        public bool currentlyPlaying;
        public int layer;
        public string name;
        public float time;
    }

    class Animation
    {
        public string name;
        public AnimationDirection direction;
        public bool loop;
    }

    class Layer
    {
        public List<Animation> animations;
    }

    SubscriberList m_subscriberList = new SubscriberList();
    Animator m_animator = null;

    List<Layer> m_layers = new List<Layer>();
    PlayingAnimation m_playingAnimation = new PlayingAnimation();

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        m_subscriberList.Add(new Event<PlayAnimationEvent>.LocalSubscriber(PlayAnim, gameObject));
        m_subscriberList.Add(new Event<StopAnimationEvent>.LocalSubscriber(StopAnim, gameObject));
        m_subscriberList.Add(new Event<GetAnimationCountEvent>.LocalSubscriber(GetAnimCount, gameObject));
        m_subscriberList.Add(new Event<GetAnimationEvent>.LocalSubscriber(GetAnim, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        //todo play next animation if current is ended
    }

    void PlayAnim(PlayAnimationEvent e)
    {
        if (e.layer < 0)
            return;

        while (m_layers.Count <= e.layer)
            m_layers.Add(new Layer());

        var anim = new Animation();

        if (!e.after)
            m_layers[e.layer].animations.Clear();

        m_layers[e.layer].animations.Add(anim);

        //play this anim maybe
    }

    void StopAnim(StopAnimationEvent e)
    {
        if (e.layer >= m_layers.Count || e.layer < 0)
            return;

        if (e.index >= m_layers[e.layer].animations.Count || e.index < 0)
            return;

        m_layers[e.layer].animations.RemoveAt(e.index);

        //todo change animation
    }

    void GetAnimCount(GetAnimationCountEvent e)
    {
        if (e.layer < m_layers.Count && e.layer >= 0)
            e.animationNb = m_layers[e.layer].animations.Count;
        else e.animationNb = 0;
    }

    void GetAnim(GetAnimationEvent e)
    {
        if(e.layer < m_layers.Count && e.layer >= 0)
        {
            if(e.index < m_layers[e.layer].animations.Count && e.index >= 0)
            {
                Animation anim = m_layers[e.layer].animations[e.index];

                e.name = anim.name;
                e.direction = anim.direction;
                e.loop = anim.loop;

                return;
            }
        }

        e.name = "";
        e.direction = AnimationDirection.none;
        e.loop = false;
    }
}