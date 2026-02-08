using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Funzione pubblica da collegare al bottone
    public void GoToMainMenu()
    {
        // Sostituisci "MainMenu" con il nome esatto della tua scena del menu
        SceneManager.LoadScene("MainMenu");
    }
}