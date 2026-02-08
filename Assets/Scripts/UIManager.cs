using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI in Gioco (Punti e Tempo)")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI timerText; 

    [Header("Risultati Finali (Scena: ScenaRisultati)")]
    public GameObject pannelloStatistiche;
    public TextMeshProUGUI testoRisultato1; 
    public TextMeshProUGUI testoRisultato2; 
    public TextMeshProUGUI testoRisultato3; 
    public TextMeshProUGUI testoValutazione; 
    public TextMeshProUGUI testoPunteggio;

    void Start()
    {
        // IMPORTANTE: Il nome deve essere "ScenaRisultati"
        if (SceneManager.GetActiveScene().name == "ScenaRisultati") 
        {
            // Recuperiamo quale gioco è stato appena completato
            string tipo = PlayerPrefs.GetString("TipoGioco", "Ricerca");
            int punti = PlayerPrefs.GetInt("PunteggioFinale", 0);
            float tempo = PlayerPrefs.GetFloat("TempoFinale", 0f);

            Debug.Log("Caricamento risultati per: " + tipo);

            if (tipo == "Memory")
            {
                int tent = PlayerPrefs.GetInt("TentativiMemory", 1);
                int coppie = PlayerPrefs.GetInt("CoppieTotali", 1);
                
                // Calcolo Valutazione Memory
                float precisione = (float)coppie / (tent > 0 ? tent : 1);
                string profilo = precisione >= 0.7f ? "MEMORIA ALTA" : (precisione >= 0.4f ? "MEMORIA MEDIA" : "MEMORIA DA ALLENARE");

                if(testoRisultato1) testoRisultato1.text = "COPPIE TROVATE: " + coppie;
                if(testoRisultato2) testoRisultato2.text = "ERRORI COMMESSI: " + (tent - coppie);
                if(testoRisultato3) testoRisultato3.text = "TEMPO IMPIEGATO: " + tempo.ToString("F1") + "s";
                if(testoValutazione) testoValutazione.text = profilo;
                if(testoPunteggio) testoPunteggio.text = "Punteggio" +punti.ToString();
            }
            else // Ricerca Visiva
            {
                int rossi = PlayerPrefs.GetInt("CubiTotaliUsciti", 1);
                int presi = punti / 10;
                
                // Calcolo Valutazione Ricerca
                float perc = (rossi > 0) ? (float)presi / rossi : 0;
                string profilo = perc >= 0.8f ? "ATTENZIONE OTTIMA" : (perc >= 0.5f ? "ATTENZIONE STABILE" : "ATTENZIONE BASSA");

                if(testoRisultato1) testoRisultato1.text = "CUBI PRESI: " + presi;
                if(testoRisultato2) testoRisultato2.text = "SU TOTALI USCITI: " + rossi;
                if(testoRisultato3) testoRisultato3.text = "TEMPO IMPIEGATO: " + tempo.ToString("F1") + "s";
                if(testoValutazione) testoValutazione.text = profilo;
                if(testoPunteggio) testoPunteggio.text = "PUNTEGGIO" +punti.ToString();
            }

            if (pannelloStatistiche) pannelloStatistiche.SetActive(true);
            
            // Solo se è una scena AR usiamo il posizionamento davanti alla camera
            if (SceneManager.GetActiveScene().name != "ScenaRisultati") PosizionaDavantiCamera();
        }
    }

    public void UpdateScore(int s) { if (scoreText != null) scoreText.text = "Punti: " + s; }
    public void UpdateTime(float t) { if (timerText != null) timerText.text = "Tempo: " + t.ToString("F1") + "s"; }

    void PosizionaDavantiCamera()
    {
        Camera c = Camera.main;
        if (c != null)
        {
            transform.position = c.transform.position + c.transform.forward * 0.8f;
            transform.LookAt(c.transform);
            transform.Rotate(0, 180, 0);
        }
    }

    public void TornaAlMenu() { SceneManager.LoadScene("MainMenu"); }
}