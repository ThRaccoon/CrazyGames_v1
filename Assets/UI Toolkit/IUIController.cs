using UnityEngine.UIElements;

public interface IUIController
{
    void Init(VisualElement root);

    void OnActivate();

    void OnDeactivate();
}