using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class MemoryExerciseManager : BaseExerciseManager
{
    [Header("Specifici Memory")]
    public GameObject[] carteAsset;
    
    [Header("Statistiche Sessione")]
    private int tentativiTotali = 0; 
    private int coppieTotaliSessione = 0; 

    private List<SelectableObject> carteInScena = new List<SelectableObject>();
    private SelectableObject primaScelta;
    private SelectableObject secondaScelta;
    private bool bloccato = false;
    private bool giocoIniziato = false;

    protected override void Start()
    {
        // Cerca automaticamente l'UI per evitare errori di trascinamento
        uiManager = Object.FindFirstObjectByType<UIManager>();
        
        base.Start();
        tempoRimanente = 0f; 
        ConfiguraLivello();
        
        // Piccolo ritardo per permettere ad AR Foundation di inizializzare i piani
        Invoke("AvviaSpawnRitardato", 0.8f);
    }

    protected override void Update()
    {
        // Il timer e il gioco procedono solo se siamo attivi
        if (!giocoIniziato || !gameActive) return;
        base.Update(); 
    }

    void AvviaSpawnRitardato()
    {
        ARPlaneManager planeManager = FindFirstObjectByType<ARPlaneManager>();
        if (planeManager != null)
        {
            giocoIniziato = true;
            SpawnSullePareti(planeManager);
        }
    }

    void ConfiguraLivello()
    {
        if (uiManager != null) uiManager.UpdateScore(score);
    }

    void SpawnSullePareti(ARPlaneManager planeManager)
    {
        List<ARPlane> pareti = new List<ARPlane>();
        foreach (var plane in planeManager.trackables)
        {
            // Cerchiamo superfici verticali (pareti)
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                pareti.Add(plane);
        }

        // Se non trova pareti, usa qualsiasi piano disponibile
        if (pareti.Count == 0) foreach (var plane in planeManager.trackables) pareti.Add(plane);
        if (pareti.Count == 0) return;

        // Imposta numero coppie in base alla difficoltà
        int numeroCoppie = (currentDifficulty == Difficulty.Facile) ? 3 : (currentDifficulty == Difficulty.Medio ? 4 : 5);
        coppieTotaliSessione += numeroCoppie; 
        
        List<GameObject> listaDaMescolare = new List<GameObject>();
        for (int i = 0; i < numeroCoppie; i++)
        {
            GameObject prefab = carteAsset[i % carteAsset.Length];
            listaDaMescolare.Add(prefab);
            listaDaMescolare.Add(prefab);
        }

        // Algoritmo di mescolamento
        for (int i = 0; i < listaDaMescolare.Count; i++)
        {
            GameObject temp = listaDaMescolare[i];
            int randomIndex = Random.Range(i, listaDaMescolare.Count);
            listaDaMescolare[i] = listaDaMescolare[randomIndex];
            listaDaMescolare[randomIndex] = temp;
        }

        // Spawn a griglia davanti al piano AR
        int colonne = 4;
        float spaziatura = 0.25f;

        for (int i = 0; i < listaDaMescolare.Count; i++)
        {
            ARPlane pareteScelta = pareti[i % pareti.Count];
            int riga = i / colonne;
            int col = i % colonne;
            
            float xPos = (col - (colonne / 2f) + 0.5f) * spaziatura;
            float yPos = (riga - (Mathf.CeilToInt((float)listaDaMescolare.Count / colonne) / 2f) + 0.5f) * spaziatura;

            Vector3 posMondo = pareteScelta.transform.TransformPoint(new Vector3(xPos, 0, yPos));
            posMondo += pareteScelta.normal * 0.1f;

            GameObject obj = Instantiate(listaDaMescolare[i], posMondo, Quaternion.LookRotation(-pareteScelta.normal));
            obj.transform.Rotate(90, 0, 0); // Corregge l'orientamento
            obj.transform.Rotate(0, 180, 0);

            SelectableObject so = obj.GetComponent<SelectableObject>();
            if (so != null) carteInScena.Add(so);
        }
    }

    public void ObjectSelected(SelectableObject carta)
    {
        if (bloccato || carta == primaScelta || !gameActive) return;
        
        carta.GiraCarta(); // Metodo sullo script della carta

        if (primaScelta == null)
        {
            primaScelta = carta;
        }
        else
        {
            secondaScelta = carta;
            tentativiTotali++; 
            StartCoroutine(ControllaCoppia());
        }
    }

    IEnumerator ControllaCoppia()
    {
        bloccato = true;
        yield return new WaitForSeconds(1f);

        if (primaScelta.nomeAnimale == secondaScelta.nomeAnimale)
        {
            AddScore(20);
            carteInScena.Remove(primaScelta);
            carteInScena.Remove(secondaScelta);
            Destroy(primaScelta.gameObject);
            Destroy(secondaScelta.gameObject);

            if (carteInScena.Count == 0) GestisciFineLivello();
        }
        else
        {
            AddScore(-2);
            primaScelta.GiraCarta(); // Rigira le carte se diverse
            secondaScelta.GiraCarta();
        }

        primaScelta = null;
        secondaScelta = null;
        bloccato = false;
    }

    void GestisciFineLivello()
    {
        // Se abbiamo finito l'ultima difficoltà (Difficile), andiamo ai risultati
        if (currentDifficulty == Difficulty.Difficile)
        {
            GameOver(); 
        }
        else
        {
            // Aumenta difficoltà e resetta
            currentDifficulty = (currentDifficulty == Difficulty.Facile) ? Difficulty.Medio : Difficulty.Difficile;
            Debug.Log("Memory: Passo al livello successivo.");
            Invoke("ResetEsercizio", 1.5f); 
        }
    }

    void ResetEsercizio()
    {
        if (!gameActive) return;
        carteInScena.Clear();
        primaScelta = null;
        secondaScelta = null;
        bloccato = false;
        giocoIniziato = false;
        AvviaSpawnRitardato();
    }

    protected override void GameOver()
    {
        gameActive = false;
        StopAllCoroutines();

        // --- NUOVO: Salvataggio Classifica ---
        // Salva il record in modo invisibile
        HighScoreManager.SalvaPunteggio("Memory", score);
        // ------------------------------------

        // Salviamo i dati per la nuova scena dei risultati (Stats partita corrente)
        PlayerPrefs.SetInt("PunteggioFinale", score);
        PlayerPrefs.SetFloat("TempoFinale", tempoRimanente);
        PlayerPrefs.SetInt("TentativiMemory", tentativiTotali);
        PlayerPrefs.SetInt("CoppieTotali", coppieTotaliSessione);
        PlayerPrefs.SetString("TipoGioco", "Memory");
        PlayerPrefs.Save();

        // Disattiva AR prima del cambio scena per pulizia (come nel Visual Search)
        ARSession session = Object.FindFirstObjectByType<ARSession>();
        if (session != null) session.enabled = false;

        // Carichiamo la nuova scena dei risultati
        UnityEngine.SceneManagement.SceneManager.LoadScene("ScenaRisultati");
    }
}