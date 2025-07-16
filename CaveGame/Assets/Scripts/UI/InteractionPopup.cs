using UnityEngine;

public class InteractionPopup : MonoBehaviour
{
    [SerializeField] private GameObject interactionPopup;

    private void Awake()
    {
        PlayerController.OnLookAtInteractable += ShowPopup;
        PlayerController.OnLookAwayFromInteractable += HidePopup;
        interactionPopup.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerController.OnLookAtInteractable -= ShowPopup;
        PlayerController.OnLookAwayFromInteractable -= HidePopup;
    }

    private void ShowPopup(InteractionRange interaction)
    {
        interactionPopup.SetActive(true);
    }

    private void HidePopup(InteractionRange interaction)
    {
        interactionPopup.SetActive(false);
    }
}
