using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemCollection")]
public class ItemCollection : ScriptableObject {


    List<Item> _collectedItems = new List<Item>();

    public int Count => _collectedItems.Count;

    public void add(Item item){

    }
    
}

public class Item{

}