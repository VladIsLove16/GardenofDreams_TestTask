using UnityEngine;
using UnityEngine.UIElements;
[RequireComponent(typeof(HealthComponent))]
public class HealthBar : MonoBehaviour
{
    [SerializeField]
    bool AlwaysVisible;
    protected VisualElement root;
    protected ProgressBar healthProgress;
    VisualElement healthProgressBar;
    protected HealthComponent healthComponent;
    #if UNITY_EDITOR
    [SerializeField]
    float width;
    [SerializeField]
    float offset;
    [SerializeField]
    float upper;
    #endif
    protected virtual void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        healthProgress = root.Q<ProgressBar>("healthProgress");
        healthProgressBar = healthProgress.Q<VisualElement>("unity-progress-bar");

        healthComponent = GetComponent<HealthComponent>();
        healthComponent.HealthChanged += UpdateHealthBarProgress;

        UpdatehealthBarWidth();
        UpdateHealthBarProgress(healthComponent.health, healthComponent.maxHealth);
        UpdateHealthBarVisibility(healthComponent.health, healthComponent.maxHealth);
    }

    protected void Update()
    {
        UpdateHealthBarPosition();
        UpdatehealthBarWidth();

    }
    public void Show()
    {
        root.style.visibility = Visibility.Visible;
    }
    public void Hide()
    {
        root.style.visibility = Visibility.Hidden;
    }
    protected void UpdateHealthBarProgress(float health, float maxHealth)
    {
        float healthRatio = health / maxHealth;
        healthProgress.value = healthRatio * 100;
        UpdateHealthBarVisibility( health,  maxHealth);

    }

    protected virtual void UpdatehealthBarWidth()
    {
        healthProgressBar.style.width = width;
        healthProgressBar.style.maxWidth = width;
    }

    protected virtual void UpdateHealthBarVisibility(float health, float maxHealth)
    {
        if (AlwaysVisible)
        {
            Show();
            return;
        }
        if (health == maxHealth)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    protected virtual void UpdateHealthBarPosition()
    {
        Vector3 worldPos = transform.position + Vector3.up * upper;  // Смещаем полоску здоровья выше врага
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        root.style.left = screenPos.x - offset;
        root.style.top = Screen.height - screenPos.y;  // Инвертируем Y
    }
}