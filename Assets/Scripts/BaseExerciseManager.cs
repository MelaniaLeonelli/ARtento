using UnityEngine;

public enum Difficulty { Facile, Medio, Difficile }

public class BaseExerciseManager : MonoBehaviour
{
    [Header("Dati Comuni")]
    public Difficulty currentDifficulty = Difficulty.Facile;
    public int score = 0;
    public float tempoRimanente;
    protected bool gameActive = false;
    protected UIManager uiManager;

    protected virtual void Start()
    {
        uiManager = Object.FindFirstObjectByType<UIManager>();
        score = 0;
        gameActive = true;
    }

  protected virtual void Update()
{
    if (!gameActive) return;

    // Invece di togliere tempo, lo aggiungiamo
    tempoRimanente += Time.deltaTime; 
    
    if (uiManager != null) 
        uiManager.UpdateTime(tempoRimanente);
}

    public virtual void AddScore(int amount)
    {
        score = Mathf.Max(0, score + amount);
        if (uiManager != null) uiManager.UpdateScore(score);
    }

    protected virtual void GameOver()
    {
        gameActive = false;
        Debug.Log("Game Over Base");
    }
}