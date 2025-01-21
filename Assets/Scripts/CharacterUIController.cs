using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterUIController : MonoBehaviour
{
    public CharacterLogicController characterLogic;

    private VisualElement root;
    private VisualElement RightPanel;
    private VisualElement LeftPanel;
    private Joystick joystick;
    private Button Shootbtn;
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
        characterLogic.SetMovementDirection(joystick.GetJoystickDirection());
    }
    private void SetupVisualElements()
    {
        RightPanel = root.Q("RightPanel");
        Debug.Assert(RightPanel != null, "RightPanel is null");

        LeftPanel = root.Q("LeftPanel");
        Debug.Assert(LeftPanel != null, "LeftPanel is null");

        Shootbtn = RightPanel.Q("Shootbtn") as Button;

        var JoystickBase = LeftPanel.Q("JoystickBase");

        joystick = new Joystick(JoystickBase);
    }
    private void SetupButtonCallbacks()
    {
        Shootbtn.clicked += ShootbtnClicked;
    }
    private void ShootbtnClicked()
    {
        characterLogic.Shoot();
    }
}
