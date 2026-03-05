using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private NameValidationService validationService;

    [Header("Restricciones")]
    [SerializeField] private int minLength = 2;
    [SerializeField] private int maxLength = 12;

    private void Start()
    {
        inputField.characterLimit = maxLength; // BLOQUEA físicamente escritura extra
        inputField.onValueChanged.AddListener(ValidateInput);
    }

    private void ValidateInput(string text)
    {
        if (validationService == null)
        {
            confirmButton.interactable = false;
            return;
        }

        bool isValid = validationService.IsNameValid(text, minLength, maxLength, out _);
        confirmButton.interactable = isValid;
    }

    public void ConfirmName()
    {
        string playerName = inputField.text.Trim();

        if (!validationService.IsNameValid(playerName, minLength, maxLength, out string error))
        {
            Debug.Log(error);
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        Debug.Log("Nombre guardado: " + playerName);
    }
}