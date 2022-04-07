using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    class PlayingAnimation
    {
        public bool currentlyPlaying;
        public bool loop;
        public int layer;
        public string name;
        public float time;
        public bool timeSet;
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
        m_subscriberList.Add(new Event<GetPlayingAnimationEvent>.LocalSubscriber(GetPlayingAnim, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        if (!m_playingAnimation.currentlyPlaying || m_animator == null)
            return;
        if (m_playingAnimation.loop)
            return;

        if(!m_playingAnimation.timeSet)
        {
            var clips = m_animator.GetCurrentAnimatorClipInfo(0);
            if (clips.Length <= 0)
            {
                return;
            }
            if(clips[0].clip.name == m_playingAnimation.name)
            {
                m_playingAnimation.timeSet = true;
                m_playingAnimation.time = clips[0].clip.length;
            }
        }

        if (!m_playingAnimation.timeSet)
            return;

        m_playingAnimation.time -= Time.deltaTime;
        if (m_playingAnimation.time > 0)
            return;

        if (m_layers.Count > m_playingAnimation.layer && m_layers[m_playingAnimation.layer].animations.Count > 0)
            m_layers[m_playingAnimation.layer].animations.RemoveAt(0);

        PlayNext();
    }

    void PlayNext()
    {
        int layer = m_playingAnimation.layer;

        do
        {
            if (m_layers[layer].animations.Count > 0)
                break;
        } while (layer > 0);

        if(m_layers[layer].animations.Count == 0)
            StopAnim();
        else Play(layer);
    }

    void Play(int layer)
    {
        Animation anim = m_layers[layer].animations[0];

        string fullName = anim.name;
        if(anim.direction != AnimationDirection.none)
            fullName += "_" + anim.direction.ToString();

        m_animator.enabled = true;
        m_animator.Play(fullName);

        m_playingAnimation.currentlyPlaying = true;
        m_playingAnimation.layer = layer;
        m_playingAnimation.loop = anim.loop;
        m_playingAnimation.name = fullName;

        m_playingAnimation.timeSet = false;
    }

    void StopAnim()
    {
        m_animator.enabled = false;
    }

    void PlayAnim(PlayAnimationEvent e)
    {
        if (e.layer < 0)
            return;

        while (m_layers.Count <= e.layer)
        {
            Layer l = new Layer();
            l.animations = new List<Animation>();
            m_layers.Add(l);
        }

        var anim = new Animation();
        anim.direction = e.direction;
        anim.loop = e.loop;
        anim.name = e.name;

        if (!e.after)
            m_layers[e.layer].animations.Clear();

        m_layers[e.layer].animations.Add(anim);

        if(e.layer >= m_playingAnimation.layer)
            Play(e.layer);
    }

    void StopAnim(StopAnimationEvent e)
    {
        if (e.layer >= m_layers.Count || e.layer < 0)
            return;

        if (e.index >= m_layers[e.layer].animations.Count || e.index < 0)
            return;

        if (e.layer == m_playingAnimation.layer)
            PlayNext();
        else m_layers[e.layer].animations.RemoveAt(e.index);
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

    void GetPlayingAnim(GetPlayingAnimationEvent e)
    {
        if (!m_playingAnimation.currentlyPlaying)
            return;

        if (m_layers.Count < m_playingAnimation.layer)
            return;

        if (m_layers[m_playingAnimation.layer].animations.Count == 0)
            return;

        var anim = m_layers[m_playingAnimation.layer].animations[0];

        e.direction = anim.direction;
        e.layer = m_playingAnimation.layer;
        e.loop = anim.loop;
        e.name = anim.name;
    }
}