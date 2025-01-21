using UnityEngine;
using UnityEngine.UIElements;

public class Joystick
{
    private VisualElement joystickBase;
    private VisualElement joystickThumb;
    private Vector2 joystickCenter;
    private Vector2 joystickDirection;
    private bool isTouching;
    public Joystick(VisualElement joystickBase)
    {
        SetupVisualElements(joystickBase);
        joystickBase.RegisterCallback<GeometryChangedEvent>(OnJoystickGeometryChanged);

    }
    private void OnJoystickGeometryChanged(GeometryChangedEvent evt)
    {
        joystickCenter = GetJoystickCenter();
    }
    private void SetupVisualElements(VisualElement joystickBase)
    {
        this.joystickBase = joystickBase;
        Debug.Assert(joystickBase != null, "joystickBase is null");

        joystickThumb = joystickBase.Q("JoystickThumb");
        Debug.Assert(joystickBase != null, "joystickBase is null");
    }

    public void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Debug.Log("touch.phase " + touch.phase + touch.position);

            switch (touch.phase)
            {

                case TouchPhase.Began:
                    if (IsTouchWithinJoystick(touch.position))
                    {
                        isTouching = true;
                    }
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isTouching)
                    {
                        UpdateJoystick(touch.position);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isTouching)
                    {
                        ResetJoystick();
                    }
                    break;
            }
        }
    }
    public Vector2 GetJoystickDirection()
    {
        var WorldCoordsDirection = new Vector2( joystickDirection.x, -joystickDirection.y );
        return WorldCoordsDirection;
    }
    private bool IsTouchWithinJoystick(Vector2 touchPositionInScreenCoord)
    {
        Vector2 touchPositionInUIToolkit = new Vector2(touchPositionInScreenCoord.x, Screen.height - touchPositionInScreenCoord.y);
        var joystickBasePosition = joystickBase.worldBound.position;

        Rect rect = new Rect(
            joystickBasePosition.x,
            joystickBasePosition.y,
            joystickBase.resolvedStyle.width,
            joystickBase.resolvedStyle.height
        );

        return rect.Contains(touchPositionInUIToolkit);
    }



    private Vector2 GetJoystickCenter()
    {
        if (joystickBase.resolvedStyle.width == 0 || joystickBase.resolvedStyle.height == 0)
        {
            Debug.LogWarning("Размеры джойстика еще не установлены!");
            return Vector2.zero;
        }

        var joystickBasePosition = joystickBase.worldBound.position;

        // Вычисляем центр джойстика
        float joystickCenterX = joystickBasePosition.x + joystickBase.resolvedStyle.width / 2;
        float joystickCenterY = joystickBasePosition.y + joystickBase.resolvedStyle.height / 2;
            
        return new Vector2(joystickCenterX, joystickCenterY);
    }

    private void UpdateJoystick(Vector2 touchPositionInScreenCoord)
    {
        Vector2 touchPositionInUIToolkit = new Vector2(touchPositionInScreenCoord.x, Screen.height - touchPositionInScreenCoord.y);
        Vector2 direction = touchPositionInUIToolkit - joystickCenter;
        float maxDistance = joystickBase.resolvedStyle.width / 2;

        if (direction.magnitude > maxDistance)
        {
            direction = direction.normalized * maxDistance;
        }

        joystickDirection = direction / maxDistance;

        joystickThumb.style.left = joystickCenter.x - joystickBase.worldBound.position.x + direction.x -  joystickThumb.resolvedStyle.width / 2;
        joystickThumb.style.top = joystickCenter.y - joystickBase.worldBound.position.y + direction.y - joystickThumb.resolvedStyle.height / 2;
        //joystickThumb.style.left = joystickCenter.x + direction.x - joystickThumb.resolvedStyle.width / 2;
        //joystickThumb.style.top = joystickCenter.y + direction.y - joystickThumb.resolvedStyle.height / 2;

        Debug.Log("joystickThumb.style.left " + joystickThumb.style.left);
        Debug.Log("joystickThumb.style.right " + joystickThumb.style.right);

    }

    private void ResetJoystick()
    {
        joystickThumb.style.left = joystickCenter.x - joystickBase.worldBound.position.x - joystickThumb.resolvedStyle.width / 2;
        joystickThumb.style.top = joystickCenter.y - joystickBase.worldBound.position.y - joystickThumb.resolvedStyle.height / 2;
        Debug.Log("joystickThumb.style.left " + joystickThumb.style.left);
        Debug.Log("joystickThumb.style.top " + joystickThumb.style.top);

        joystickDirection = Vector2.zero;

        isTouching = false;
    }
}