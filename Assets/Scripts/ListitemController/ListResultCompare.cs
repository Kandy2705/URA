using UnityEngine;
using System;
using System.Collections.Generic;

public class ListResultCompare : MonoBehaviour
{   
    public ListController listController;
    [SerializeField] private GameObject dataContainer;
    [SerializeField] private Transform obtainProductContainer; 
    [SerializeField] private Transform wrongProductContainer; 

    [SerializeField] private GameTimer gameTimer;
    private List<GameObject> choicedItems;
    private float itemHeight = 5f;   
    private float startY = 10f;      
    public List<CompareResult> compareResults = new List<CompareResult>();

    public bool loadStart = true;

    public static bool compareResultListUpdated = false;
    public Action LoadCSV;

    void Start()
    {
        if (listController == null)
        {
            listController = FindFirstObjectByType<ListController>();
            if (listController == null)
            {
                Debug.LogError("Không tìm thấy ListController trong scene!");
                return;
            }    
            Debug.Log($"ListResultCompare: Loaded {choicedItems?.Count ?? 0} choicedItems");
        }

        listController.OnListChanged += HandleListChanged;

        choicedItems = listController.choicedItems;

        string quantityText = "";

        foreach(GameObject item in choicedItems)
        {
            ProcessItem(item, ref quantityText);
        }

        LoadCSV += LoadCompareResult;
        loadStart = false;
    }

    void Update()
    {
        if (!gameTimer.isRunning)
        {
            LoadCSV?.Invoke();
        }
    }

    private void ProcessItem(GameObject item, ref string quantityText)
    {
        Debug.Log("Processing item: " + item.name);
        quantityText = "0";
        string itemName = item.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text;
        int itemQuantity = int.Parse(item.transform.Find("Quantity").GetComponent<TMPro.TMP_Text>().text);
        BillEntry entry = new BillEntry(itemName, 0, itemQuantity);

        
        CompareData(entry, 0, itemQuantity, ref quantityText);
        CreateNewInstantData(entry, obtainProductContainer, ref quantityText);
    }

    public void HandleListChanged(string oldName, GameObject newData, int newQuantity)
    {
        BillEntry entry = null;
        if (CartManager.Instance.bill.ContainsKey(newData.name))
        {
            Debug.Log("Vao if la an lon");
            entry = CartManager.Instance.bill[newData.name];
        }
        else
        {
            Debug.Log("Vao else la an cac");
            entry = new BillEntry(newData.name, 0, 0);
            Debug.Log($"Số lượng mới của vật phẩm là {entry.quantity}");
        }

        int index = compareResults.FindIndex(r => r.itemName == oldName);
        Debug.Log($"GIá trị index hiện tại là {index} cho item: {oldName}");


        string quantityText = "0";

        string status = StatusChange(entry.quantity, newQuantity, ref quantityText);

        CompareResult newResult = new CompareResult(entry.itemName, entry.quantity, newQuantity, status, entry.price, true);
        compareResults[index] = newResult;

        Debug.Log("Dữ liệu của compare list đã được thay đổi !");
    }

    private void LoadCompareResult()
    {
        Debug.Log("Dữ liệu đang được chạy");

        foreach(CompareResult result in compareResults)
        {
            if(result.required == false && result.currentQuantity == 0) break;
            DataManager.Instance.AddCompareResult(result);
        }
    
        DataManager.Instance.Report();
        LoadCSV -= LoadCompareResult;
        Debug.Log("Đã chạy xong hàm Load các compare data");
    }

    void OnEnable()
    {
        Debug.Log("ListResultCompare: OnEnable được gọi, đăng ký event");
        if (PokeManager.Instance != null)
        {
            PokeManager.Instance.OnInventoryChanged += HandleBillChanged;
        }
        else
        {
            Debug.LogError("ListResultCompare không tìm thấy PokeManager!");
        }
    }

    void OnDisable()
    {
        if (PokeManager.Instance != null)
        {
            PokeManager.Instance.OnInventoryChanged -= HandleBillChanged;
        }
    }

    public void CompareData(BillEntry entry, int currentQuantity, int IndexQuantity, ref string quantityText)
    {
        Debug.Log($"CompareData() called for {entry.itemName}: {currentQuantity}/{IndexQuantity}");

        string status = StatusChange(currentQuantity, IndexQuantity, ref quantityText);

        HandleCompareResultList(entry.itemName, currentQuantity, IndexQuantity, status, entry.price);
    }

    public string StatusChange(int currentQuantity, int IndexQuantity, ref string quantityText)
    {
        string status = "";
        if (currentQuantity < IndexQuantity)
        {
            int shortage = IndexQuantity - currentQuantity;
            status = $"Thiếu {shortage}";
            quantityText = $"{currentQuantity} (Thiếu {shortage})";
        }
        else if (currentQuantity == IndexQuantity)
        {
            status = "Đủ";
            quantityText = $"{currentQuantity} (Đủ)";
        }
        else
        {
            int surplus = currentQuantity - IndexQuantity;
            status = $"Dư {surplus}";
            quantityText = $"{currentQuantity} (Dư {surplus})";
        }
        return status;
    }

    private void HandleCompareResultList(string itemName, int currentQuantity, int indexQuantity, string status, int price)
    {

        int existingIndex = compareResults.FindIndex(r => r.itemName == itemName);

        if (loadStart)
        {
            CompareResult newResult = new CompareResult(itemName, currentQuantity, indexQuantity, status, price, true);
            compareResults.Add(newResult);
        }
        else
        {
            if (existingIndex != -1)
            {
                bool lastRequireStatus = compareResults[existingIndex].required;
                CompareResult newResult = new CompareResult(itemName, currentQuantity, indexQuantity, status, price, lastRequireStatus);
                compareResults[existingIndex] = newResult;
            }
            else
            {
                CompareResult newResult = new CompareResult(itemName, currentQuantity, indexQuantity, status, price, false);
                compareResults.Add(newResult);
            }
        }

        
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
        if (entry == null) return; 
        
        Transform parentContainer;

        string addItem = entry.itemName;
        int currentQuantity = entry.quantity;

        GameObject matchedItem = choicedItems.Find(item =>
            item.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text == addItem
        );

        if (isNew)
        {
            string quantityText = entry.quantity.ToString();
            if (matchedItem)
            {
                parentContainer = obtainProductContainer;
                // Debug.Log($"Found matching item in choicedItems: {addItem}");
                // Debug.Log("Số sản phẩm hiện tại là: " + entry.quantity);

                // int choiceQuantity = int.Parse(matchedItem.transform.Find("Quantity").GetComponent<TMPro.TMP_Text>().text);
                // CompareData(entry, entry.quantity, choiceQuantity, ref quantityText);

                int choiceQuantity = int.Parse(matchedItem.transform.Find("Quantity").GetComponent<TMPro.TMP_Text>().text);
                CompareData(entry, currentQuantity, choiceQuantity, ref quantityText);
                
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
            else
            {
                parentContainer = wrongProductContainer;
                Debug.LogWarning($"No matching item found in choicedItems for: {addItem}");
                CompareData(entry, entry.quantity, 0, ref quantityText);
                CreateNewInstantData(entry, parentContainer, ref quantityText);
            }
        }
        else
        {
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
                CompareData(entry, entry.quantity, 0, ref quantityText);

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
        // DataManager.Instance.ExportCSV(compareResults);

    }
}
