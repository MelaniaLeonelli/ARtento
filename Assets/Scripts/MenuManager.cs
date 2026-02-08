using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void AvviaRicercaVisiva()
    {
        PlayerPrefs.SetString("EsercizioScelto", "Attenzione");
        SceneManager.LoadScene("EsercizioRicercaVisiva");
    }
    public void AvviaTutorial()
    {
        PlayerPrefs.SetString("EsercizioScelto", "Attenzione");
        SceneManager.LoadScene("TutorialScene");
    }

    public void AvviaMemorySpaziale()
    {
        Debug.Log("Pulsante Memory cliccato!");
        PlayerPrefs.SetString("EsercizioScelto", "Memoria");
        //SceneManager.LoadScene(1);
        SceneManager.LoadScene("AR_GameScene");
    }
}