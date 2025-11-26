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
    // public string createdAt;
    // public bool completedSet;
    public Player() { }
    public Player(string playerName)
    {
        this.playerName = playerName;
    }
    public Player(string playerName, string email, string playeruid, float timeTaken/*, string createdAt, bool completedSet*/)
    {
        this.playerName = playerName;
        this.email = email;
        this.playeruid = playeruid;
        this.timeTaken = timeTaken;
        // this.createdAt = createdAt;
        // this.completedSet = completedSet;
    }
}
