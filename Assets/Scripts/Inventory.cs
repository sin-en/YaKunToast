using System;

[Serializable]
public class Inventory
{
    public string itemId;
    public string itemName;
    public string collectedAt; // Timestamp

    public Inventory() { }

    public Inventory(string itemId, string itemName)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.collectedAt = System.DateTime.UtcNow.ToString("o");
    }
}
