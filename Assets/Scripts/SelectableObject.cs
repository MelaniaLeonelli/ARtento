using UnityEngine;
using System.Collections;

public class SelectableObject : MonoBehaviour
{

    [Header("Impostazioni Attenzione")]
    public bool isCorrect = false;

    [Header("Impostazioni Memory")]
    public string nomeAnimale;

    private bool isFlipped = false;

    //private AttentionExerciseManager manager;

    //void Start()
    //{
    //    manager = Object.FindFirstObjectByType<AttentionExerciseManager>();
    //}
    //public void SetManager(AttentionExerciseManager m)
    //{
    //    manager = m;
    //}

   public void OnSelected()
{
    // Cerchiamo il manager generico (BaseExerciseManager) 
    // così funziona sia per Memory che per Ricerca Visiva
    var genericManager = Object.FindFirstObjectByType<BaseExerciseManager>();

    if (genericManager != null)
    {
        // Se siamo in Ricerca Visiva
        if (genericManager is VisualSearchManager attentionManager)
        {
            attentionManager.ObjectSelected(this);
        }
        // Se siamo in Memory
        else if (genericManager is MemoryExerciseManager memoryManager)
        {
            if (isFlipped) return;
            memoryManager.ObjectSelected(this);
        }
    }
}


    public void ShakeObject()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        float duration = 0.5f;
        float magnitude = 0.01f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y, originalPos.z + z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    private void OnMouseDown()
    {
        OnSelected();
    }

    public void GiraCarta()
    {
        isFlipped = !isFlipped;

        if (isFlipped)
            transform.Rotate(Vector3.forward, 180f);
        else
            transform.Rotate(Vector3.forward, -180f);
    }

}
