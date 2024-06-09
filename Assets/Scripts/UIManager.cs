using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button toggleInventoryButton;
    [SerializeField] private Button gachaButton;
    [SerializeField] private GameObject weaponInventoryScrollView;
    [SerializeField] private RectTransform buttonContainer;

    private bool menuOpened = false;

    private void Start()
    {
        weaponInventoryScrollView.gameObject.SetActive(false);
        toggleInventoryButton.gameObject.SetActive(false);
        gachaButton.gameObject.SetActive(false);

        menuButton.onClick.AddListener(ToggleMenu);
        toggleInventoryButton.onClick.AddListener(ToggleWeaponInventory);
    }

    private void ToggleMenu()
    {
        if (menuOpened)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        toggleInventoryButton.gameObject.SetActive(true);
        gachaButton.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(toggleInventoryButton.transform.DOMoveX(buttonContainer.position.x + 150, 0.3f).From(buttonContainer.position));
        sequence.Append(gachaButton.transform.DOMoveX(buttonContainer.position.x + 320, 0.3f).From(buttonContainer.position));

        menuOpened = true;
    }

    private void CloseMenu()
    {
        // Animate buttons to slide left and then deactivate
        Sequence sequence = DOTween.Sequence();
        sequence.Append(gachaButton.transform.DOMoveX(buttonContainer.position.x, 0.3f));
        sequence.Append(toggleInventoryButton.transform.DOMoveX(buttonContainer.position.x, 0.3f)).OnComplete(() =>
        {
            toggleInventoryButton.gameObject.SetActive(false);
            gachaButton.gameObject.SetActive(false);
        });

        menuOpened = false;
    }

    private void ToggleWeaponInventory()
    {
        weaponInventoryScrollView.SetActive(!weaponInventoryScrollView.activeSelf);
    }
}
