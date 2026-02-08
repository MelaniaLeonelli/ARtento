using UnityEngine;
using System.Collections.Generic;

public static class HighScoreManager
{
    public static void SalvaPunteggio(string nomeGioco, int punteggio)
    {
        // 1. Carica i vecchi punteggi
        List<int> scores = GetHighScores(nomeGioco);

        // 2. Aggiunge il nuovo
        scores.Add(punteggio);

        // 3. Ordina dal più grande al più piccolo
        scores.Sort((a, b) => b.CompareTo(a));

        // 4. Tiene solo i migliori 3
        if (scores.Count > 3)
        {
            scores.RemoveRange(3, scores.Count - 3);
        }

        // 5. Salva su disco
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetInt(nomeGioco + "_" + i, scores[i]);
        }
        PlayerPrefs.Save();
    }

    // Questa serve per leggere i dati nella classifica
    public static List<int> GetHighScores(string nomeGioco)
    {
        List<int> scores = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            scores.Add(PlayerPrefs.GetInt(nomeGioco + "_" + i, 0));
        }
        return scores;
    }
}