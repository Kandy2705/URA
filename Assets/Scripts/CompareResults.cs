using System;

[System.Serializable]
public struct CompareResult
{
    public string itemName;
    public int currentQuantity;
    public int expectedQuantity;
    public string status;

    public CompareResult(string name, int current, int expected, string s)
    {
        itemName = name;
        currentQuantity = current;
        expectedQuantity = expected;
        status = s;
    }
}
