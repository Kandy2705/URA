using UnityEngine;

public class ListController : MonoBehaviour
{
    [SerializeField] private GameObject listItemPrefab;
    private bool isVisible;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleList()
    {
        isVisible = !isVisible;
        listItemPrefab.gameObject.SetActive(isVisible);
    }
}
