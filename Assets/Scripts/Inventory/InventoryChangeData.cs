using Assets.WUG.Scripts;
using System.Collections.Generic;

public class InventoryChangeData
{
    public Dictionary<string, int> Items { get; set; }
    public InventoryChangeType ChangeType { get; set; }
    public InventorySlot InventorySlot { get; set; }
}
