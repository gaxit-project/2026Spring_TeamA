using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }

    string GetInteractPrompt(); // 画面に出す文字を教えてもらう

    void Interact(GameObject interactor);
}