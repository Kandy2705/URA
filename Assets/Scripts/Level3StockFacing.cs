using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Scene-authored Level 3 physical units share one existing SelectableItem stock source.
// Keeping the object alive preserves the startup registry even when its visible unit is consumed.
[DisallowMultipleComponent, RequireComponent(typeof(SelectableItem))]
public sealed class Level3StockFacing : MonoBehaviour
{
    public SelectableItem stockOwner;
    public bool Consumed { get; private set; }
    public bool IsStockOwner => stockOwner == GetComponent<SelectableItem>();
    public bool CanPurchase => !Consumed && stockOwner != null && gameObject.activeInHierarchy;
    Vector3 shelfPosition;
    Quaternion shelfRotation;
    bool waitingForRelease;
    void Awake()
    {
        shelfPosition = transform.position;
        shelfRotation = transform.rotation;
    }
    public void Consume()
    {
        Consumed = true;
        // Keep the purchased physical unit in the controller until its selection ends.
        // Programmatic purchases (including the editor click fallback) still hide immediately.
        var grab = GetComponent<XRGrabInteractable>();
        waitingForRelease = grab != null && grab.isSelected;
        if (!waitingForRelease) SetVisible(false);
    }
    void LateUpdate()
    {
        if (!waitingForRelease) return;
        var grab = GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected) return;
        waitingForRelease = false;
        if (Consumed) SetVisible(false);
    }
    public void Restore()
    {
        // Returning a held unit must not make a second purchase possible mid-selection.
        var grab = GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected)
            grab.interactionManager.SelectCancel(grab.interactorsSelecting[0], grab);
        waitingForRelease = false;
        transform.SetPositionAndRotation(shelfPosition, shelfRotation);
        var body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.position = shelfPosition;
            body.rotation = shelfRotation;
        }
        Consumed = false;
        SetVisible(true);
    }
    void SetVisible(bool visible)
    {
        foreach(var renderer in GetComponentsInChildren<Renderer>()) renderer.enabled = visible;
        foreach(var collider in GetComponentsInChildren<Collider>()) collider.enabled = visible;
        foreach(var interactable in GetComponentsInChildren<XRBaseInteractable>()) interactable.enabled = visible;
    }
    public static void RestoreReturnedUnit(SelectableItem owner)
    {
        var unit = FindObjectsByType<Level3StockFacing>(FindObjectsSortMode.None)
            .FirstOrDefault(f => f.stockOwner == owner && f.Consumed);
        if(unit != null) unit.Restore();
    }
}
