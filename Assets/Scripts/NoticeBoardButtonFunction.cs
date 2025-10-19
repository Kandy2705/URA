using System.Collections;
using UnityEngine;

public class NoticeBoardButtonFunction : MonoBehaviour
{
    [SerializeField] 
    private GameObject listItemToBuy;

    private bool isShowing = false;

    [SerializeField] private ListController listController;

    public void ClickShow()
    {
        listController.ShowList();
    }

    private IEnumerator ShowThenHide()
    {
        isShowing = true;
        listItemToBuy.SetActive(true);

        yield return new WaitForSeconds(10f);

        listItemToBuy.SetActive(false);
        isShowing = false;
    }
}