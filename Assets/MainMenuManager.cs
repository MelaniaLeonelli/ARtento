using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    // ========================================================================
    // 1. VARIABILI ESISTENTI (Audio, Pannelli, UI) - NON TOCCARE
    // ========================================================================
    [Header("UI Pannelli (Opzionali)")]
    public GameObject pannelloImpostazioni; 
    public GameObject pannelloTutorial2; 

    [Header("Impostazioni Audio")]
    public AudioMixer masterMixer; 
    public AudioSource musicaSottofondo;
    public AudioSource sfxSorgente;

    [Header("Riferimenti Slider UI (Opzionali)")]
    public Slider sliderMusicaUI; 
    public Slider sliderSuoniUI;

    [Header("Riferimenti Tutorial (Opzionali)")]
    public TextMeshProUGUI titoloTutorial;
    public TextMeshProUGUI sottotitoloTutorial;
    public GameObject tutorial2Button; 

    // ========================================================================
    // 2. SETUP INIZIALE
    // ========================================================================
    private void Start()
    {
        // Nascondi i pannelli SOLO SE esistono in questa scena
        if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(false);
        if (pannelloTutorial2 != null) pannelloTutorial2.SetActive(false);

        // Recupera i volumi salvati
        float musicaSalvata = PlayerPrefs.GetFloat("MusicVolumeSave", 0.75f);
        float sfxSalvato = PlayerPrefs.GetFloat("SFXVolumeSave", 0.75f);

        // Applica agli slider SOLO SE esistono
        if (sliderMusicaUI != null) sliderMusicaUI.value = musicaSalvata;
        if (sliderSuoniUI != null) sliderSuoniUI.value = sfxSalvato;

        // Forza l'aggiornamento del Mixer
        SetVolumeMusica(musicaSalvata);
        SetVolumeSFX(sfxSalvato);
    }

    // ========================================================================
    // 3. LOGICA DI NAVIGAZIONE GIOCHI (AGGIUNTA DA MENUMANAGER)
    // ========================================================================
    
    // Funzione per avviare la RICERCA VISIVA (Presa dal codice dei tuoi compagni)
    public void AvviaRicercaVisiva()
    {
        Debug.Log("Avvio Ricerca Visiva...");
        PlayerPrefs.SetString("EsercizioScelto", "Attenzione");
        // ATTENZIONE: Qui ho messo il nome della scena che c'era nel codice dei tuoi compagni.
        // Se la scena si chiama diversamente, cambia la stringa qui sotto.
        SceneManager.LoadScene("EsercizioRicercaVisiva");
    }

    // Funzione per avviare il MEMORY (Presa dal codice dei tuoi compagni)
    public void AvviaMemorySpaziale()
    {
        Debug.Log("Avvio Memory...");
        PlayerPrefs.SetString("EsercizioScelto", "Memoria");
        // Qui ho messo "AR_GameScene" come richiesto nel tuo codice precedente
        SceneManager.LoadScene("AR_GameScene");
    }

    

    // ========================================================================
    // 4. METODI PER IL VOLUME
    // ========================================================================
    public void SetVolumeMusica(float volume)
    {
        if (masterMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20;
            masterMixer.SetFloat("VolumeMusica", dB);
            PlayerPrefs.SetFloat("MusicVolumeSave", volume);
            PlayerPrefs.Save();
        }
    }

    public void SetVolumeSFX(float volume)
    {
        if (masterMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20;
            masterMixer.SetFloat("VolumeSFX", dB);
            PlayerPrefs.SetFloat("SFXVolumeSave", volume);
            PlayerPrefs.Save();
        }
    }

    // ========================================================================
    // 5. METODI PER IL TUTORIAL POP-UP
    // ========================================================================
    public void ApriTutorial2()
    {
        if (pannelloTutorial2 != null)
        {
            pannelloTutorial2.SetActive(true);
            Animator anim = pannelloTutorial2.GetComponent<Animator>();
            if (anim != null) anim.Play("Finestra_PopIn");
            
            if (tutorial2Button != null) tutorial2Button.SetActive(false);
        }
    }

    public void ChiudiTutorial2()
    {
        if (pannelloTutorial2 != null)
        {
            pannelloTutorial2.SetActive(false);
            if (tutorial2Button != null) tutorial2Button.SetActive(true);
        }
    }

    // ========================================================================
    // 6. METODI UTILITY E ALTRA NAVIGAZIONE
    // ========================================================================
    public void SuonoTap(AudioClip clip)
    {
        if (sfxSorgente != null && clip != null) 
            sfxSorgente.PlayOneShot(clip);
    }

    public void ApriImpostazioni() { if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(true); }
    public void ChiudiImpostazioni() { if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(false); }
    
    public void AvviaTutorial() { 
        // Aggiungo anche qui il settaggio della preferenza se serve
        PlayerPrefs.SetString("EsercizioScelto", "Attenzione");
        SceneManager.LoadScene("TutorialScene"); 
    }
    
    public void TornaAlMenu() { SceneManager.LoadScene("MainMenu"); } 
    public void EsciDalGioco() { Application.Quit(); }


    public void ApriClassifica()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("ScenaClassifica");
}
}