using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    InputAction inventoryAction;
    [SerializeField]
    private UIInventoryPage inventoryUI;
    [SerializeField]
    private InventoryScriptableObject inventoryData;
    public List<InventoryItem> initialItems = new List<InventoryItem>();
    private void Start()
    {
        PrepareUI();
        
        inventoryAction = new InputAction("Inventory", binding: "<Keyboard>/i");
        PrepareInventoryData();
    }

    private void PrepareInventoryData()
    {
        inventoryData.Initialize();
        inventoryData.OnInventoryChanged += UpdateUI;
        foreach (InventoryItem item in initialItems)
        {
            if (item.IsEmpty)
                continue;
            inventoryData.AddItem(item);
        }
        inventoryAction.Enable();
    }

    private void UpdateUI(Dictionary<int, InventoryItem> inventoryState)
    {
        inventoryUI.ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
        }
        
    }

    private void PrepareUI()
    {
        inventoryUI.InitializeInventoryUI(inventoryData.Size);
        inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
        inventoryUI.OnSwapItems += HandleSwapItems;
        inventoryUI.OnStartDragging += HandleDragging;
        inventoryUI.OnItemActionRequested += HandleItemActionRequest;
    }

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if(inventoryItem.IsEmpty) 
        {
            inventoryUI.ResetSelection();
            return;
        }
        ItemScriptableObject item = inventoryItem.item;
        inventoryUI.UpdateDescription(itemIndex, item.Name, item.ItemImage, item.Description);
    }
    private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
    {
        inventoryData.SwapItems(itemIndex_1, itemIndex_2);
    }

    private void HandleDragging(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            return;
        }
        inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        
    }
    public void Update()
    {
        if (inventoryAction.triggered)
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
                }
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        
        else
            {
                inventoryUI.Hide();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        } 
    }  
}
