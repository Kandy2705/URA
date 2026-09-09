using System;
using System.Collections.Generic;

[Serializable]
public class ShoppingMission
{
    public string missionId = "mission";
    public string displayName = "Shopping mission";
    public List<ShoppingTaskItem> items = new List<ShoppingTaskItem>();
}
