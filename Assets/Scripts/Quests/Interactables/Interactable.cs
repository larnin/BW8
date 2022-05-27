using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract int GetInteractionTextID();
    public abstract void Interact(GameObject caster);
}