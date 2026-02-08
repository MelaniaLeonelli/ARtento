using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    public TextMeshProUGUI[] testiMemory; // Trascina qui i 3 testi del Memory
    public TextMeshProUGUI[] testiRicerca; // Trascina qui i 3 testi della Ricerca

    void Start()
    {
        MostraPunteggi("Memory", testiMemory);
        MostraPunteggi("VisualSearch", testiRicerca);
    }

    void MostraPunteggi(string gioco, TextMeshProUGUI[] testi)
{
    List<int> scores = HighScoreManager.GetHighScores(gioco);
    for (int i = 0; i < testi.Length; i++)
    {
        if (i < scores.Count && testi[i] != null)
        {
            // Modifica questa riga qui sotto:
            string etichetta = gioco == "Memory" ? "MEMORY" : "RICERCA";
            testi[i].text = "TOP " + (i + 1) + " PUNTEGGIO " + etichetta + ": " + scores[i].ToString();
        }
    }
}

    // Collega questa funzione al pulsante "Indietro"
    public void TornaAlMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); 
    }
}