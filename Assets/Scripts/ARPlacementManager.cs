using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARPlacementManager : MonoBehaviour
{
    [Header("Prefab da piazzare")]
    public GameObject visualSearchPrefab;
    public GameObject memoryPrefab;

    [Header("Interfaccia Istruzioni")]
    public TextMeshProUGUI istruzioniText;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;


    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();


    private bool hasPlacedScene = false;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (hasPlacedScene) return;

        // 1. Logica dei messaggi dinamici
        AggiornaIstruzioni();

        // 2. Controllo tocco per piazzare
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            PiazzaEsercizio(hits[0].pose);
        }
    }

    void AggiornaIstruzioni()
    {
        if (istruzioniText == null) return;

        // Conta quanti piani sono stati trovati
        int pianiTrovati = 0;
        foreach (var plane in planeManager.trackables)
        {
            if (plane.trackingState == TrackingState.Tracking) pianiTrovati++;
        }

        if (pianiTrovati == 0)
        {
            istruzioniText.text = "Muovi lentamente il telefono per scansionare le pareti...";
            //Debug.Log("DEBUG: Nessun piano, scrivo Muovi...");
        }
        else
        {
            istruzioniText.text = "Parete individuata! Tocca un punto della parete per iniziare.";
            //Debug.Log("DEBUG: Piano trovato, scrivo Tocca...");
        }
    }

    void PiazzaEsercizio(Pose hitPose)
    {
        string esercizio = PlayerPrefs.GetString("EsercizioScelto", "Memoria");

        if (esercizio == "Memoria")
            Instantiate(memoryPrefab, hitPose.position, hitPose.rotation);
        else
            Instantiate(visualSearchPrefab, hitPose.position, hitPose.rotation);

        DisablePlanes();
        hasPlacedScene = true;

        Debug.Log("Area di gioco piazzata correttamente");
        // Nascondi il testo delle istruzioni
        if (istruzioniText != null) istruzioniText.gameObject.SetActive(false);
    }

    void DisablePlanes()
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        planeManager.enabled = false;
    }


}
