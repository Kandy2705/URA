using UnityEngine;
using System.Collections.Generic;

public class ListResultCompare : MonoBehaviour
{   
    public ListController listController;
    public CartManager cartManager;
    [SerializeField] private GameObject dataContainer;
    [SerializeField] private Transform obtainProductContainer; 
    [SerializeField] private Transform wrongProductContainer;
    
    private List<GameObject> choicedItems;
    private float itemHeight = 5f;   
    private float startY = 10f;      
    public List<CompareResult> compareResults = new List<CompareResult>();

    void Start()
    {
        if (listController == null)
    {
        listController = FindObjectOfType<ListController>();
        if (listController == null)
        {
            Debug.LogError("Không tìm thấy ListController trong scene!");
            return;
        }    
        Debug.Log($"ListResultCompare: Loaded {choicedItems?.Count ?? 0} choicedItems");

    }

    choicedItems = listController.choicedItems;
    }

    void OnEnable()
    {
        Debug.Log("ListResultCompare: OnEnable được gọi, đăng ký event");
        cartManager.OnBillChanged += HandleBillChanged;

    }

    void OnDisable()
    {
        cartManager.OnBillChanged -= HandleBillChanged;
    }

    public void CompareData(BillEntry entry, int currentQuantity, int IndexQuantity, ref string quantityText)
    {
        Debug.Log($"CompareData() called for {entry.itemName}: {currentQuantity}/{IndexQuantity}");

        if (currentQuantity < IndexQuantity)
        {
            int shortage = IndexQuantity - currentQuantity;
            quantityText += $" (Thiếu {shortage})";
        }
        else if (currentQuantity == IndexQuantity)
        {
            quantityText += " (Đủ)";
        }
        else
        {
            int surplus = currentQuantity - IndexQuantity;
            quantityText += $" (Dư {surplus})";
        }
        CompareResult result = new CompareResult(entry.itemName, currentQuantity, IndexQuantity, quantityText);
        compareResults.Add(result);
        DataManager.Instance.AddCompareResult(result);

    }

    public void CreateNewInstantData(BillEntry entry, Transform parentContainer, ref string quantityText)
    {
        Debug.Log("Creating new instant data for: " + entry.itemName);
        GameObject data = Instantiate(dataContainer, parentContainer, false);
        data.name = entry.itemName;
        data.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text = entry.itemName;
        data.transform.Find("Quantity").GetComponent<TMPro.TMP_Text>().text = quantityText;

        Vector3 newPos = new Vector3(0f, startY, 0f);
        if (parentContainer.childCount > 1)
        {
            Transform lastItem = parentContainer.GetChild(parentContainer.childCount - 2);
            newPos.y = lastItem.localPosition.y - itemHeight;
        }
        data.transform.localPosition = newPos;
    }

    void HandleBillChanged(BillEntry entry, bool isNew)
    {
        Debug.Log("Bill has been updated: " + entry.itemName + ", Quantity: " + entry.quantity);
        Transform parentContainer;

        string addItem = entry.itemName;
        GameObject matchedItem = choicedItems.Find(item =>
            item.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text == addItem
        );

        if (isNew)
        {
            string quantityText = entry.quantity.ToString();
            if (matchedItem)
            {
                parentContainer = obtainProductContainer;
                Debug.Log($"Found matching item in choicedItems: {addItem}");
                Debug.Log("Số sản phẩm hiện tại là: " + entry.quantity);

                int choiceQuantity = int.Parse(matchedItem.transform.Find("Quantity").GetComponent<TMPro.TMP_Text>().text);
                CompareData(entry, entry.quantity, choiceQuantity, ref quantityText);
            }
            else
            {
                parentContainer = wrongProductContainer;
                Debug.LogWarning($"No matching item found in choicedItems for: {addItem}");
            }
            CreateNewInstantData(entry, parentContainer, ref quantityText);
        }
        else
        {
            int currentQuantity = entry.quantity;
            string quantityText = currentQuantity.ToString();

            if (matchedItem)
            {
                parentContainer = obtainProductContainer;
                Debug.Log($"Found matching item in choicedItems: {addItem}");
                Debug.Log("Số sản phẩm hiện tại là: " + entry.quantity);
                int choiceQuantity = int.Parse(matchedItem.transform.Find("Quantity").GetComponent<TMPro.TMP_Text>().text);
                CompareData(entry ,currentQuantity, choiceQuantity, ref quantityText);
            }
            else
            {
                parentContainer = wrongProductContainer;
                Debug.LogWarning($"No matching item found in choicedItems for: {addItem}");
            }
            
            // Cập nhật UI
            for (int i = 0; i < parentContainer.childCount; i++)
            {
                Transform child = parentContainer.GetChild(i);
                if (child.Find("Name").GetComponent<TMPro.TMP_Text>().text == entry.itemName)
                {
                    child.Find("Quantity").GetComponent<TMPro.TMP_Text>().text = quantityText;
                    break;
                }
            }
        }
        DataManager.Instance.ExportCSV(compareResults);

    }
}
