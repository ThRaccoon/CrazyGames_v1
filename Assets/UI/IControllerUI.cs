using UnityEngine.UIElements;

public interface IControllerUI
{
    void Init(VisualElement root);

    void OnActivate();

    void OnDeactivate();
}