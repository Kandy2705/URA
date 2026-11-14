using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class ListControllerStockTests
{
    private readonly List<GameObject> createdObjects = new List<GameObject>();
    private PokeManager previousInventory;
    private PokeManager inventory;
    private UnityEngine.Random.State previousRandomState;

    [SetUp]
    public void SetUp()
    {
        previousRandomState = UnityEngine.Random.state;
        UnityEngine.Random.InitState(1234);
        previousInventory = PokeManager.Instance;
        SetInventoryInstance(null);
        inventory = CreateObject("Inventory").AddComponent<PokeManager>();
        SetInventoryInstance(inventory);
        SetStock("Out of stock", 0);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject created in createdObjects)
            Object.DestroyImmediate(created);
        createdObjects.Clear();
        SetInventoryInstance(previousInventory);
        UnityEngine.Random.state = previousRandomState;
    }

    [Test]
    public void InitialList_UsesOnlyStockedProductsAndCapsRequestedQuantities()
    {
        SetStock("Pan", 5);
        SetStock("Single", 1);
        SetStock("Plenty", 20);
        ListController list = CreateList("Pan", "Single", "Plenty", "Out of stock", "Missing");

        list.EnsureMainListGenerated();

        Assert.That(list.MainListGenerated, Is.True);
        Assert.That(list.choicedItems, Has.Count.EqualTo(3));
        foreach (GameObject row in list.choicedItems)
        {
            string name = row.transform.Find("Name").GetComponent<TMP_Text>().text;
            int quantity = int.Parse(row.transform.Find("Quantity").GetComponent<TMP_Text>().text);
            Assert.That(quantity, Is.InRange(1, Mathf.Min(9, inventory.GetAvailableQuantity(name))), name);
        }
    }

    [Test]
    public void Replacement_UsesCurrentStockAndDoesNotDuplicateAnotherTask()
    {
        SetStock("Pan", 5);
        ListController list = CreateList("Pan", "New product", "Out of stock");
        list.EnsureMainListGenerated();
        SetStock("New product", 1);
        int changes = 0;
        list.OnListChanged += (_, prefab, quantity) =>
        {
            changes++;
            Assert.That(prefab.name, Is.EqualTo("New product"));
            Assert.That(quantity, Is.EqualTo(1));
        };

        list.ReplaceRandomItemWithUniquePrefab();
        list.ReplaceRandomItemWithUniquePrefab();

        Assert.That(changes, Is.EqualTo(1));
        Assert.That(list.choicedItems[0].transform.Find("Name").GetComponent<TMP_Text>().text,
            Is.EqualTo("New product"));
    }

    [Test]
    public void Replacement_WhenRemainingProductsAreDepleted_KeepsOriginalTask()
    {
        SetStock("Pan", 5);
        ListController list = CreateList("Pan", "Depleted");
        list.EnsureMainListGenerated();
        SetStock("Depleted", 0);
        string originalName = list.choicedItems[0].transform.Find("Name").GetComponent<TMP_Text>().text;
        int changes = 0;
        list.OnListChanged += (_, __, ___) => changes++;

        list.ReplaceRandomItemWithUniquePrefab();

        Assert.That(changes, Is.Zero);
        Assert.That(list.choicedItems[0].transform.Find("Name").GetComponent<TMP_Text>().text,
            Is.EqualTo(originalName));
    }

    private ListController CreateList(params string[] names)
    {
        ListController list = CreateObject("List").AddComponent<ListController>();
        list.enabled = false; // Avoid UI coroutines in Edit Mode.
        SetField(list, "listContainer", CreateObject("Rows"));
        List<GameObject> prefabs = new List<GameObject>();
        foreach (string name in names)
        {
            GameObject row = CreateObject(name);
            foreach (string field in new[] { "Name", "Quantity" })
            {
                GameObject label = new GameObject(field, typeof(RectTransform), typeof(TextMeshProUGUI));
                label.transform.SetParent(row.transform);
            }
            prefabs.Add(row);
        }
        SetField(list, "availablePrefabs", prefabs);
        return list;
    }

    private void SetStock(string name, int quantity)
    {
        var stock = (Dictionary<string, int>)typeof(PokeManager)
            .GetField("remainingProductStock", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(inventory);
        stock[name] = quantity;
    }

    private GameObject CreateObject(string name)
    {
        GameObject result = new GameObject(name);
        createdObjects.Add(result);
        return result;
    }

    private static void SetField(object target, string name, object value) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

    private static void SetInventoryInstance(PokeManager value) =>
        typeof(PokeManager).GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic)
            .SetValue(null, value);
}
