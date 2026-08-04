using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SelectableItem : MonoBehaviour
{
    public string itemName;
    public int price;
    [Min(1)] public int stockQuantity = 1;
    

    void Start() {
        ItemsManager.RegisterSelectableItem(this);
    }

    void OnDestroy() {
        ItemsManager.UnregisterSelectableItem(this);
    }
    // private void OnMouseDown()
    // {
    //     GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
    //     if (playerObj == null)
    //     {
    //         Debug.LogWarning("Không tìm thấy Player với tag 'Player'");
    //         return;
    //     }
    //
    //     float dist = Vector3.Distance(transform.position, playerObj.transform.position);
    //
    //     if (CartManager.Instance != null)
    //     {
    //         CartManager.Instance.AddItem(this);
    //     }
    //
    //     Destroy(gameObject);
    // }
}
