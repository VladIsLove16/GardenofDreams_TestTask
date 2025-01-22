using System.Collections.Generic;
using System.Linq;

using UnityEngine.UIElements;
using UnityEngine;

namespace Assets.WUG.Scripts
{
    public class InventoryUIController 
    {
        public List<InventorySlot> InventorySlots = new List<InventorySlot>();

        private  VisualElement m_Root;
        private  VisualElement m_SlotContainer;
        private  VisualElement m_GhostIcon;
        
        private  bool m_IsDragging;
        private  InventorySlot m_OriginalSlot;
        private InventoryController inventoryController;
        public void Setup(VisualElement inventoryRoot, InventoryController inventoryController)
        {
            //Store the root from the UI Document component
            this.inventoryController  = inventoryController;
            m_Root = inventoryRoot;
            m_GhostIcon = m_Root.Query<VisualElement>("GhostIcon");

            //Search the root for the SlotContainer Visual Element
            m_SlotContainer = m_Root.Q<VisualElement>("SlotContainer");

            //Create 20 InventorySlots and add them as children to the SlotContainer
            for (int i = 0; i < inventoryController.MaxSlotsCount; i++)
            {
                InventorySlot slot = new InventorySlot();
                slot.Init(this);

                InventorySlots.Add(slot);

                m_SlotContainer.Add(slot);
            }

            //Register event listeners
            inventoryController.OnInventoryChanged += GameController_OnInventoryChanged;
            GameController_OnInventoryChanged(inventoryController.GetAllItemDetails().Select(item=>item.GUID).ToArray(), InventoryChangeType.Pickup);
            m_GhostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            m_GhostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

            if(inventoryController.OpenOnStart)
                m_Root.style.visibility = Visibility.Visible;
            else
                m_Root.style.visibility = Visibility.Hidden;
        }        
        
        /// <summary>
        /// Initiate the drag
        /// </summary>
        /// <param name="ScreenPosition">Mouse Position</param>
        /// <param name="originalSlot">Inventory Slot that the player has selected</param>
        public  void StartDrag(Vector2 ScreenPosition, InventorySlot originalSlot)
        {
            //Set tracking variables
            m_IsDragging = true;
            m_OriginalSlot = originalSlot;

            //Set the new position
            //Vector2 UIToollkitPostion = new Vector2(ScreenPosition.x, Screen.height - ScreenPosition.y);

            m_GhostIcon.style.top = ScreenPosition.y  - m_GhostIcon.layout.height / 2;
            m_GhostIcon.style.left = ScreenPosition.x  - m_GhostIcon.layout.width / 2;

            //Set the image
            m_GhostIcon.style.backgroundImage = inventoryController.GetItemByGuid(originalSlot.ItemGuid).Icon.texture;

            //Flip the visibility on
            m_GhostIcon.style.visibility = Visibility.Visible;

        }

        /// <summary>
        /// Perform the drag
        /// </summary>
        private void OnPointerMove(PointerMoveEvent evt)
        {
            //Only take action if the player is dragging an item around the screen
            if (!m_IsDragging)
            {
                return;
            }
            Debug.Log("Pointer Move");
            //Vector2 UIToolkitPos = new Vector2(evt.position.x, Screen.height - evt.position.y);
            //Set the new position
            m_GhostIcon.style.top = evt.position.y - m_GhostIcon.layout.height / 2;
            m_GhostIcon.style.left = evt.position.x  - m_GhostIcon.layout.width / 2;

        }

        /// <summary>
        /// Finish the drag and compute whether the item should be moved to a new slot
        /// </summary>
        private void OnPointerUp(PointerUpEvent evt)
        {

            if (!m_IsDragging)
            {
                return;
            }

            //Check to see if they are dropping the ghost icon over any inventory slots.
            IEnumerable<InventorySlot> slots = InventorySlots.Where(x => x.worldBound.Overlaps(m_GhostIcon.worldBound));
            Debug.Log(slots.Count() + " slots.Count()");
            //Found at least one
            if (slots.Count() != 0)
            {
                InventorySlot closestSlot = slots.OrderBy(x => Vector2.Distance(x.worldBound.position, m_GhostIcon.worldBound.position)).First();
                if(closestSlot.IsHoldingItem())
                {
                    string ClosestSlotItemGuid = closestSlot.GetGuid();
                    closestSlot.HoldItem(inventoryController.GetItemByGuid(m_OriginalSlot.ItemGuid));
                    m_OriginalSlot.HoldItem(inventoryController.GetItemByGuid(ClosestSlotItemGuid));
                }
                else
                {
                    closestSlot.HoldItem(inventoryController.GetItemByGuid(m_OriginalSlot.ItemGuid));
                    m_OriginalSlot.DropItem();
                }
            }
            else
            {
                m_OriginalSlot.Icon.image = inventoryController.GetItemByGuid(m_OriginalSlot.ItemGuid).Icon.texture;
            }


            //Clear dragging related visuals and data
            m_IsDragging = false;
            m_OriginalSlot = null;
            m_GhostIcon.style.visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Listen for changes to the players inventory and act
        /// </summary>
        /// <param name="itemGuid">Reference ID for the Item Database</param>
        /// <param name="change">Type of change that occurred. This could be extended to handle drop logic.</param>
        private void GameController_OnInventoryChanged(string[] itemGuid, InventoryChangeType change)
        {
            //Loop through each item and if it has been picked up, add it to the next empty slot
            foreach (string item in itemGuid)
            {
                if (change == InventoryChangeType.Pickup)
                {
                   var emptySlot = InventorySlots.FirstOrDefault(x => x.ItemGuid.Equals(""));
                    
                    if (emptySlot != null)
                    {
                        emptySlot.HoldItem(inventoryController.GetItemByGuid(item));
                    }
                }
            }
        }
    }
}
