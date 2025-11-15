using System;

[Serializable]
public class Inventory
{
    public string itemName;
    public int qty;

    public Inventory(string itemName, int qty)
    {
        this.itemName = itemName;
        this.qty = qty;
    }
}
