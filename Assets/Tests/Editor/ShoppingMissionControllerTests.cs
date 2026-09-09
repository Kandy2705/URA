using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ShoppingMissionControllerTests
{
    private readonly List<Object> createdObjects = new List<Object>();

    [TearDown]
    public void TearDown()
    {
        foreach (Object created in createdObjects)
            Object.DestroyImmediate(created);
        createdObjects.Clear();
    }

    [Test]
    public void SupplementalTask_DoesNotMutateInitialTasks()
    {
        ShoppingMissionController controller = CreateController(
            new List<ShoppingTaskItem> { new ShoppingTaskItem("Apple", 1) });
        ShoppingTaskItem supplemental = new ShoppingTaskItem("Banana", 2);

        bool added = controller.TryAddSupplementalTask(supplemental);

        Assert.That(added, Is.True);
        Assert.That(controller.InitialTasks, Has.Count.EqualTo(1));
        Assert.That(controller.InitialTasks[0].itemName, Is.EqualTo("Apple"));
        Assert.That(controller.SupplementalTasks, Has.Count.EqualTo(1));
        Assert.That(controller.SupplementalTasks[0].itemName, Is.EqualTo("Banana"));
    }

    [Test]
    public void SupplementalTask_CannotBeAddedTwiceWhileItsPopupIsShowing()
    {
        ShoppingMissionController controller = CreateController(new List<ShoppingTaskItem>());
        ShoppingTaskItem supplemental = new ShoppingTaskItem("Banana", 1);

        Assert.That(controller.TryAddSupplementalTask(supplemental), Is.True);
        Assert.That(controller.TryAddSupplementalTask(supplemental), Is.False);
        Assert.That(controller.SupplementalTasks, Has.Count.EqualTo(1));
    }

    private ShoppingMissionController CreateController(List<ShoppingTaskItem> initial)
    {
        GameObject controllerObject = new GameObject("ShoppingMissionController (test)");
        createdObjects.Add(controllerObject);
        ShoppingMissionController controller = controllerObject.AddComponent<ShoppingMissionController>();
        controller.ConfigureForTests(initial);
        return controller;
    }
}
