using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu]
public class InventoryScriptableObject : ScriptableObject
{
  [SerializeField]
  private List<InventoryItem> inventoryItems;

  [field: SerializeField]
  public int Size { get; private set; } = 20;

  public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;

  public void Initialize()
    {
        inventoryItems = new List<InventoryItem>();
        for (int i = 0; i < Size; i++)
        {
            inventoryItems.Add(InventoryItem.GetEmptyItem());
        }
    }

    public int AddItem(ItemScriptableObject item, int quantity)
        {
            if (item.isStackable == false)
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    while (quantity > 0 && IsInventoryFull() == false)
                    {
                        quantity -= AddItemToFirstFreeSlot(item, 1);
                    }
                }
                InformAboutChange();
                return quantity;
            }
            quantity = AddStackableItem(item, quantity);
            InformAboutChange();
            return quantity;
        }

    private int AddItemToFirstFreeSlot(ItemScriptableObject item, int quantity)
    {
        InventoryItem newItem = new InventoryItem
        {
            item = item,
            quantity = quantity
        };

        for (int i = 0; i < inventoryItems.Count; i++)
            if(inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = newItem;
                return quantity;
            }
        return 0;
    }

    private bool IsInventoryFull()
     => inventoryItems.Where(item => item.IsEmpty).Any() == false;

    private int AddStackableItem(ItemScriptableObject item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
                continue;
            if (inventoryItems[i].item.ID == item.ID)
            {
                int amountCanBeAdded = inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;
                    if (quantity > amountCanBeAdded)
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                    quantity -= amountCanBeAdded;
                }
                else
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + quantity);
                    InformAboutChange();
                    return 0;
                }
            }  
        }
        while (quantity > 0 && IsInventoryFull() == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(item, newQuantity);
        }
        return quantity;
    }

    internal void AddItem(InventoryItem item)
    {
        AddItem(item.item, item.quantity);
    }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty) continue;
                returnValue[i] = inventoryItems[i];
            }
        return returnValue;
    }

    public InventoryItem GetItemAt(int itemIndex)
    {
        return inventoryItems[itemIndex];
    }

    internal void SwapItems(int itemIndex_1, int itemIndex_2)
    {
        InventoryItem item1 = inventoryItems[itemIndex_1];
        inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
        inventoryItems[itemIndex_2] = item1;
        InformAboutChange();
    }

   private void InformAboutChange()
   {
        OnInventoryChanged?.Invoke(GetCurrentInventoryState());
   }
}



[Serializable]
public struct InventoryItem
{
    public int quantity;
    public ItemScriptableObject item;
    public bool IsEmpty => item == null;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
        return new InventoryItem
        {
            item = this.item,
            quantity = newQuantity
        };
    }

    public static InventoryItem GetEmptyItem()
        => new InventoryItem
        {
            item = null,
            quantity = 0
        };
}