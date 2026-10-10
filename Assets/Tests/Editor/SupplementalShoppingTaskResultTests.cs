using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class SupplementalShoppingTaskResultTests
{
    private readonly List<GameObject> createdObjects = new List<GameObject>();

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject created in createdObjects)
            Object.DestroyImmediate(created);
        createdObjects.Clear();
    }

    [Test]
    public void MainRowsAndTwoSeparateRequestsAreRequiredWithoutChangingMainRows()
    {
        ListController list = CreateObject("List").AddComponent<ListController>();
        list.choicedItems.Add(CreateRow("Apple", 2));
        list.choicedItems.Add(CreateRow("Banana", 5));
        ListResultCompare results = CreateObject("Results").AddComponent<ListResultCompare>();
        results.listController = list;

        Invoke(results, "HandleMainListGenerated");
        Invoke(results, "HandleMainListGenerated"); // A late callback must not double the main requirements.
        Invoke(results, "HandleSupplementalTaskAdded", new ShoppingTaskItem("Grape", 1));
        Invoke(results, "HandleSupplementalTaskAdded", new ShoppingTaskItem("Orange", 1));

        Assert.That(list.choicedItems, Has.Count.EqualTo(2));
        AssertRequired(results, "Apple", 2, 0);
        AssertRequired(results, "Banana", 5, 0);
        AssertRequired(results, "Grape", 1, 0);
        AssertRequired(results, "Orange", 1, 0);

        PokeManager inventory = CreateObject("Inventory").AddComponent<PokeManager>();
        BillEntry grape = new BillEntry("Grape", 10, 1);
        inventory.inventory.Add("Grape", grape);
        Invoke(results, "HandleBillChanged", grape, true);
        AssertRequired(results, "Grape", 1, 1);
        AssertRequired(results, "Orange", 1, 0);
    }

    private GameObject CreateObject(string name)
    {
        GameObject result = new GameObject(name);
        createdObjects.Add(result);
        return result;
    }

    private GameObject CreateRow(string name, int quantity)
    {
        GameObject row = CreateObject(name + " row");
        GameObject nameObject = new GameObject("Name", typeof(RectTransform), typeof(TextMeshProUGUI));
        nameObject.transform.SetParent(row.transform);
        nameObject.GetComponent<TextMeshProUGUI>().text = name;
        GameObject quantityObject = new GameObject("Quantity", typeof(RectTransform), typeof(TextMeshProUGUI));
        quantityObject.transform.SetParent(row.transform);
        quantityObject.GetComponent<TextMeshProUGUI>().text = quantity.ToString();
        return row;
    }

    private static void Invoke(object target, string methodName, params object[] arguments)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null);
        method.Invoke(target, arguments);
    }

    private static void AssertRequired(ListResultCompare results, string name, int expected, int current)
    {
        CompareResult result = results.compareResults.Find(item => item.itemName == name);
        Assert.That(result.required, Is.True, name);
        Assert.That(result.expectedQuantity, Is.EqualTo(expected), name);
        Assert.That(result.currentQuantity, Is.EqualTo(current), name);
    }
}
