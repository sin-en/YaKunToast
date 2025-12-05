using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string playerName;
    public string email;
    public string playeruid;
    public float timeTaken;
    public List<Inventory> itemsCollected = new List<Inventory>();
    public bool completedSet;
    public string completedAt; //when all items collected
    public Player() { }
    public Player(string playerName)
    {
        this.playerName = playerName;
        this.itemsCollected = new List<Inventory>();
        this.completedSet = false;
    }
     public Player(string playerName, string email, string playeruid, float timeTaken)
    {
        this.playerName = playerName;
        this.email = email;
        this.playeruid = playeruid;
        this.timeTaken = timeTaken;
        this.itemsCollected = new List<Inventory>();
        this.completedSet = false;
    }
}
