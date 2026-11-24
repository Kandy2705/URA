using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PokeObjects : MonoBehaviour
{
    private void Awake()
    {
        // Contact offset is a runtime Collider property, not a serialized prefab setting.
        // Small retail units have millimetre gaps; the 1 cm default creates false contacts.
        if (GetComponent<SelectableItem>() != null && GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() != null)
            foreach (var collider in GetComponents<Collider>()) collider.contactOffset = .001f;
    }
    // Khi bắt đầu poke (chạm)
    public void OnPokeStart(SelectEnterEventArgs args)
    {
        GameObject pokedObject = args.interactableObject.transform.gameObject;

        if (pokedObject != null)
        {
            // Debug.Log($"👉 Đã poke vào: {pokedObject.name}");
            SelectableItem item = pokedObject.GetComponent<SelectableItem>();
            if (item != null && PokeManager.Instance != null)
            {
                PokeManager.Instance.PokingItem(item);
            }
        }
    }

    // Khi rời khỏi vật thể
    public void OnPokeEnd(SelectExitEventArgs args)
    {
        GameObject pokedObject = args.interactableObject.transform.gameObject;

        if (pokedObject != null)
        {
            Debug.Log($"👋 Rời khỏi vật thể");
        }
    }
}

