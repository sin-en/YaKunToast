/*
* Author: Kwek Sin En
* Date: 15/11/2025
* Description: Represents a player with unique ID, name, email, time taken, and collected items.
*/
using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string playeruid;
    public string playerName;
    public string email;
    public float timeTaken;
    public List<Inventory> itemsCollected = new List<Inventory>();
    public bool completedSet;
    public string completedAt;

    // Default constructor
    public Player() 
    {
        this.itemsCollected = new List<Inventory>();
        this.completedSet = false;
        this.timeTaken = 0f;
    }

    // Constructor for simple player creation
    public Player(string playerName)
    {
        this.playerName = playerName;
        this.itemsCollected = new List<Inventory>();
        this.completedSet = false;
        this.timeTaken = 0f;
    }

    // Full constructor
    public Player(string playeruid, string playerName, string email)
    {
        this.playeruid = playeruid;
        this.playerName = playerName;
        this.email = email;
        this.itemsCollected = new List<Inventory>();
        this.completedSet = false;
        this.timeTaken = 0f;
    }
}