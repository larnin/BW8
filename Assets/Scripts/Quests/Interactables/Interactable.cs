using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract bool CanInteract();

    public abstract bool IsInstantInteraction();

    public abstract int GetInteractionTextID();

    public abstract Vector2 GetOffset();

    public abstract void Interact(GameObject caster);
}