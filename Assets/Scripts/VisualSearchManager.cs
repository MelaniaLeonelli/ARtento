using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class VisualSearchManager : BaseExerciseManager
{
    [Header("Collegamenti UI")]
    public UIManager uiManagerScript;
    [Header("I nostri stampini (Prefab)")]
    public GameObject targetPrefab; // Cubo Blu
    public GameObject distractorPrefab; // Cubo Rosso

    [Header("Impostazioni Sessione")]
    public float spawnRadius = 0.5f;
    public int ondateTotali = 5; 

    [Header("Statistiche Studio")]
    private int totaleCubiRossiUsciti = 0; 

    private ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    private int successiConsecutivi = 0;
    private int erroriConsecutivi = 0;
    private int ondataAttuale = 0;

    protected override void Start()
    {
        base.Start();
        arRaycastManager = FindFirstObjectByType<ARRaycastManager>();

        uiManagerScript = Object.FindFirstObjectByType<UIManager>();
        if (uiManagerScript != null) uiManager = uiManagerScript;

        tempoRimanente = 0f; 
        ConfiguraLivello();
        StartCoroutine(GeneratoreDiOndate());
    }

    protected override void Update()
    {
        base.Update(); 
    }

    IEnumerator GeneratoreDiOndate()
    {
        ondataAttuale = 0;
        yield return new WaitForSeconds(1.0f); 

        while (gameActive && ondataAttuale < ondateTotali)
        {
            ondataAttuale++;
            int quantitaOggetti = (currentDifficulty == Difficulty.Facile) ? 5 : (currentDifficulty == Difficulty.Medio ? 8 : 12);

            for (int i = 0; i < quantitaOggetti; i++)
            {
                CreaOggettoCentroSchermo();
                yield return new WaitForSeconds(0.15f);
            }

            float pausa = (currentDifficulty == Difficulty.Facile) ? 3.0f : (currentDifficulty == Difficulty.Medio ? 2.0f : 1.0f);
            yield return new WaitForSeconds(pausa);
        }

        GameOver();
    }

    void CreaOggettoCentroSchermo()
    {
        if (arRaycastManager == null) return;
        Vector2 centroSchermo = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // AllTypes permette lo spawn anche dove non c'è un piano perfetto
        if (arRaycastManager.Raycast(centroSchermo, hits, TrackableType.AllTypes))
        {
            Pose hitPose = hits[0].pose;
            bool isRosso = (Random.value < 0.4f);
            GameObject prefabScelto = isRosso ? distractorPrefab : targetPrefab;

            if (isRosso) totaleCubiRossiUsciti++; 

            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = hitPose.position + (hitPose.right * randomOffset.x) + (hitPose.up * randomOffset.y);
            spawnPos += hitPose.forward * 0.05f;

            GameObject nuovo = Instantiate(prefabScelto, spawnPos, hitPose.rotation);
            SelectableObject so = nuovo.GetComponent<SelectableObject>();
            if (so != null) so.isCorrect = isRosso; 

            float durata = (currentDifficulty == Difficulty.Facile) ? 7f : (currentDifficulty == Difficulty.Medio ? 5f : 3f);
            Destroy(nuovo, durata);
        }
    }

    public void ObjectSelected(SelectableObject obj)
    {
        if (!gameActive) return;

        if (obj.isCorrect)
        {
            AddScore(10); 
            successiConsecutivi++;
            erroriConsecutivi = 0;

            if (successiConsecutivi >= 3)
            {
                AumentaDifficolta();
                successiConsecutivi = 0;
            }
        }
        else
        {
            AddScore(-5); 
            successiConsecutivi = 0;
            erroriConsecutivi++;
            obj.ShakeObject();

            if (erroriConsecutivi >= 3)
            {
                DiminuisceDifficolta();
                erroriConsecutivi = 0;
            }
        }
        Destroy(obj.gameObject);
    }

    void ConfiguraLivello()
    {
        if (uiManager != null) uiManager.UpdateScore(score);
    }

    void AumentaDifficolta()
    {
        if (currentDifficulty == Difficulty.Facile) currentDifficulty = Difficulty.Medio;
        else if (currentDifficulty == Difficulty.Medio) currentDifficulty = Difficulty.Difficile;
        ConfiguraLivello();
    }

    void DiminuisceDifficolta()
    {
        if (currentDifficulty == Difficulty.Difficile) currentDifficulty = Difficulty.Medio;
        else if (currentDifficulty == Difficulty.Medio) currentDifficulty = Difficulty.Facile;
        ConfiguraLivello();
    }

   protected override void GameOver()
    {
        gameActive = false;
        StopAllCoroutines();

        HighScoreManager.SalvaPunteggio("VisualSearch", score);
        

        
        PlayerPrefs.SetInt("PunteggioFinale", score);
        PlayerPrefs.SetFloat("TempoFinale", tempoRimanente);
        PlayerPrefs.SetInt("CubiTotaliUsciti", totaleCubiRossiUsciti); 
        PlayerPrefs.SetString("TipoGioco", "Ricerca"); 
        PlayerPrefs.Save();

        // Disattiva AR prima del cambio scena per fermare gli errori nei log
        ARSession session = Object.FindFirstObjectByType<ARSession>();
        if (session != null) session.enabled = false;

        SceneManager.LoadScene("ScenaRisultati");
    }
}