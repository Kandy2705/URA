using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public PlayerMove playerMove;
    public CameraLook cameraLook;
    public TouchController touchController;
    public GameObject shopPanel;  

    public void OnTimeUp()
    {
        if (playerMove != null) playerMove.enabled = false;
        if (cameraLook != null) cameraLook.enabled = false;
        if (touchController != null) touchController.enabled = false;

        if (shopPanel != null) shopPanel.SetActive(true);

        Debug.Log("Gameplay đã bị khóa. Mở shop.");
    }

    public void RestoreControl()
    {
        if (playerMove != null) playerMove.enabled = true;
        if (cameraLook != null) cameraLook.enabled = true;
        if (touchController != null) touchController.enabled = true;

        if (shopPanel != null) shopPanel.SetActive(false);

        Debug.Log("▶️ Đã trả control cho người chơi.");
    }
}
