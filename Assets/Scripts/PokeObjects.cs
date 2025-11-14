using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PokeObjects : MonoBehaviour
{
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

