using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Servicio encargado de validar nombres de jugador.
/// Carga una lista de palabras prohibidas desde un archivo txt
/// y comprueba si el nombre introducido contiene alguna.
/// </summary>
public class NameValidationService : MonoBehaviour
{
    [Header("Archivo con palabras prohibidas")]
    [SerializeField] private TextAsset bannedWordsFile;

    // Conjunto de palabras prohibidas cargadas en memoria
    private HashSet<string> bannedWords = new HashSet<string>();

    /// <summary>
    /// Se ejecuta al iniciar el objeto y carga las palabras prohibidas.
    /// </summary>
    private void Awake()
    {
        LoadBannedWords();
    }

    /// <summary>
    /// Lee el archivo txt línea por línea y guarda cada palabra en un HashSet.
    /// </summary>
    void LoadBannedWords()
    {
        if (bannedWordsFile == null)
        {
            Debug.LogWarning("No banned words file assigned.");
            return;
        }

        using (StringReader reader = new StringReader(bannedWordsFile.text))
        {
            while (reader.Peek() != -1)
            {
                string word = reader.ReadLine().Trim().ToLower();

                if (!string.IsNullOrEmpty(word))
                    bannedWords.Add(word);
            }
        }

        Debug.Log("Banned words loaded: " + bannedWords.Count);
    }

    /// <summary>
    /// Comprueba si un nombre es válido según:
    /// - longitud mínima
    /// - longitud máxima
    /// - palabras prohibidas
    /// </summary>
    public bool IsNameValid(string name, int minLength, int maxLength, out string error)
    {
        error = "";

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Nombre vacío";
            return false;
        }

        if (name.Length < minLength)
        {
            error = "Nombre demasiado corto";
            return false;
        }

        if (name.Length > maxLength)
        {
            error = "Nombre demasiado largo";
            return false;
        }

        // Normalizamos el nombre antes de comprobarlo
        string normalized = NormalizeName(name);

        // Comprobamos si contiene alguna palabra prohibida
        foreach (var banned in bannedWords)
        {
            if (normalized.Contains(banned))
            {
                error = "Nombre contiene palabra prohibida";
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Normaliza el nombre eliminando caracteres especiales
    /// y dejando solo letras en minúsculas.
    /// Esto evita que los jugadores evadan el filtro con
    /// cosas como "p.u.t.a" o "p u t a".
    /// </summary>
    string NormalizeName(string input)
    {
        input = input.ToLower();

        StringBuilder sb = new StringBuilder();

        foreach (char c in input)
        {
            if (char.IsLetter(c))
                sb.Append(c);
        }

        return sb.ToString();
    }
}