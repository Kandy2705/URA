using UnityEngine;
public class CartItemInstance
{
    public CollectibleItem itemData;
    public GameObject sceneObjectReference;

    public CartItemInstance(CollectibleItem data, GameObject sceneObj)
    {
        itemData = data;
        sceneObjectReference = sceneObj;
    }
}