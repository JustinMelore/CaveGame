using UnityEngine;

public class InteractionPopup : MonoBehaviour
{

    private Canvas popupCanvas;

    private void Awake()
    {
        PlayerController.OnLookAtInteractable += ShowPopup;
        PlayerController.OnLookAwayFromInteractable += HidePopup;
        popupCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        popupCanvas.enabled = false;
    }

    private void OnDisable()
    {
        PlayerController.OnLookAtInteractable -= ShowPopup;
        PlayerController.OnLookAwayFromInteractable -= HidePopup;
    }

    private void ShowPopup(InteractionRange interaction)
    {
        popupCanvas.enabled = true;
    }

    private void HidePopup(InteractionRange interaction)
    {
        popupCanvas.enabled = false;
    }
}
