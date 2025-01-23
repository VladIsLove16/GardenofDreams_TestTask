using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.WUG.Scripts;
public class GameUI : MonoBehaviour
{
    public CharacterLogicController characterLogicController;

    private VisualElement root;
    private VisualElement InventoryRoot;
    private InventoryUIController InventoryUIController = new();
    private VisualElement RightPanel;
    private VisualElement LeftPanel;
    private Joystick joystick;
    private Button Shootbtn;
    private Button DeleteItembtn;
    private Button Inventorybtn;
    [SerializeField]
    private bool isTouching; 

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        SetupVisualElements();
        SetupButtonCallbacks();

    }
    private void Update()
    {
        joystick.Update();
        characterLogicController.SetMovementDirection(joystick.GetJoystickDirection());
    }
    private void SetupVisualElements()
    {
        InventoryRoot = root.Q("Inventory");
        InventoryUIController.Setup(InventoryRoot, characterLogicController.GetInventoryController());

        RightPanel = root.Q("RightPanel");
        Debug.Assert(RightPanel != null, "RightPanel is null");

        LeftPanel = root.Q("LeftPanel");
        Debug.Assert(LeftPanel != null, "LeftPanel is null");

        Shootbtn = RightPanel.Q("Shootbtn") as Button;
        DeleteItembtn = RightPanel.Q("DeleteItembtn") as Button;
        Inventorybtn = RightPanel.Q("Inventorybtn") as Button;

        var JoystickBase = LeftPanel.Q("JoystickBase");

        joystick = new Joystick(JoystickBase);
    }
    private void SetupButtonCallbacks()
    {
        Shootbtn.clicked += ShootbtnClicked;
        Inventorybtn.clicked += InventorybtnClicked;
        DeleteItembtn.clicked += DeleteItembtnClicked;
    }

    private void DeleteItembtnClicked()
    {
        InventorySlot inventorySlot = InventoryUIController.GetSelectedSlot();
        InventoryController inventoryController = characterLogicController.GetInventoryController();
        ItemDetails item = inventoryController.GetItemByGuid(inventorySlot.ItemGuid);
        inventoryController.RemoveItem(item);
    }

    private void ShootbtnClicked()
    {
        characterLogicController.Shoot();
    }
    private void InventorybtnClicked()
    {
        if (InventoryRoot.style.visibility == Visibility.Visible)
        {
            InventoryRoot.style.visibility = Visibility.Hidden;
        }
        else
        {
            InventoryRoot.style.visibility = Visibility.Visible;
        }
    }
}
