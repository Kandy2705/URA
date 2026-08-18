using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SelectableItem : MonoBehaviour
{
    public string itemName;
    public int price;
    

    void Start() {
        ItemsManager.RegisterSelectableItem(this);
    }

    void OnDestroy() {
        ItemsManager.UnregisterSelectableItem(this);
    }
#if UNITY_EDITOR
    // Fast task-testing fallback. It deliberately uses the same inventory path as XR poke
    // and does not destroy the shelf product, so repeated clicks can test quantities.
    private void OnMouseDown()
    {
        if (Application.isPlaying && PokeManager.Instance != null)
            PokeManager.Instance.PokingItem(this);
    }
#endif
}
