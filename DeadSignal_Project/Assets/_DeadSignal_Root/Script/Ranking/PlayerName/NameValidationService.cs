using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NameValidationService : MonoBehaviour
{
    private HashSet<string> bannedWords = new HashSet<string>();
    private readonly Regex allowedCharacters = new Regex("^[a-zA-Z0-9]+$");

    private void Awake()
    {
        LoadBannedWords();
    }

    private void LoadBannedWords()
    {
        TextAsset file = Resources.Load<TextAsset>("banned_words");

        if (file == null)
        {
            Debug.LogError("No se encontró banned_words.txt en Resources.");
            return;
        }

        string[] words = file.text.Split('\n');

        foreach (string word in words)
        {
            string clean = word.Trim().ToLower();
            if (!string.IsNullOrEmpty(clean))
            {
                bannedWords.Add(clean);
            }
        }

        Debug.Log("Palabras cargadas: " + bannedWords.Count);
    }

    public bool IsNameValid(string name, int minLength, int maxLength, out string errorMessage)
    {
        errorMessage = "";

        if (string.IsNullOrWhiteSpace(name))
        {
            errorMessage = "Nombre vacío.";
            return false;
        }

        name = name.Trim();

        if (name.Length < minLength)
        {
            errorMessage = "Nombre demasiado corto.";
            return false;
        }

        if (name.Length > maxLength)
        {
            errorMessage = "Nombre demasiado largo.";
            return false;
        }

        if (!allowedCharacters.IsMatch(name))
        {
            errorMessage = "Solo letras y números permitidos.";
            return false;
        }

        string lowerName = name.ToLower();

        foreach (string banned in bannedWords)
        {
            if (lowerName.Contains(banned))
            {
                errorMessage = "Nombre no permitido.";
                return false;
            }
        }

        return true;
    }
}