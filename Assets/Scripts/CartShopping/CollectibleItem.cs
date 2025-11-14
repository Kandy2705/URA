using UnityEngine;

[CreateAssetMenu(fileName = "New Collectible Item", menuName = "My Game/Collectible Item")]
public class CollectibleItem : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite itemIcon;
}
