using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class InspectManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject canvasHUD;
    public GameObject canvasInspeccion;
    public RectTransform panelRotacion;
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoConsejo;
    public Button botonSalir;

    [Header("Crosshair")]
    public GameObject crosshair;

    [Header("Cámaras")]
    public Camera mainCamera;
    public Camera inspectCamera;

    [Header("Rotación")]
    public Transform pivotInspeccion;
    public float rotacionVelocidad = 70f;

    [Header("Zoom por distancia")]
    public float zoomStep = 0.5f;
    public float distMin = 1.5f;
    public float distMax = 7f;

    [Header("Posición inicial del cuadro")]
    public Vector3 offsetLocal = new Vector3(0.82f, -0.18f, 2.5f);

    private GameObject clon;
    private bool inspeccionActiva;
    private bool arrastrando;
    private Vector2 ultimoPointer;
    private PlayerFPSController player;

    void Start()
    {
        canvasInspeccion.SetActive(false);
        inspectCamera.gameObject.SetActive(false);

        player = FindObjectOfType<PlayerFPSController>();
        botonSalir.onClick.AddListener(Salir);
    }

    // ==============================================================
    public void Iniciar(GameObject objeto, string descripcion)
    {
        if (inspeccionActiva) return;

        inspeccionActiva = true;
        player.SetControlsEnabled(false);
        player.ForceUnlockCursor();

        if (crosshair != null)
            crosshair.SetActive(false);

        // activar cámara inspección
        mainCamera.gameObject.SetActive(false);
        inspectCamera.gameObject.SetActive(true);

        // Reset pivot
        pivotInspeccion.localPosition = offsetLocal;
        pivotInspeccion.localRotation = Quaternion.identity;

        if (clon != null)
            Destroy(clon);

        // Crear clon
        clon = Instantiate(objeto, pivotInspeccion);
        clon.transform.localPosition = Vector3.zero;
        clon.transform.localRotation = Quaternion.Euler(0, 0, 180);
        clon.transform.localScale = objeto.transform.lossyScale * 0.90f;

        foreach (var c in clon.GetComponentsInChildren<Collider>())
            c.enabled = false;

        textoDescripcion.text = descripcion;
        canvasInspeccion.SetActive(true);
        canvasHUD.SetActive(false);

        arrastrando = false;
    }

    void Update()
    {
        if (!inspeccionActiva) return;

        ProcesarPC();
    }

    // ================= PC ====================
    private void ProcesarPC()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 pos = mouse.position.ReadValue();

        bool dentro = RectTransformUtility.RectangleContainsScreenPoint(
            panelRotacion, pos, inspectCamera);

        // ROTACIÓN
        if (mouse.leftButton.isPressed && dentro)
        {
            if (!arrastrando)
            {
                arrastrando = true;
                ultimoPointer = pos;
            }
            else
            {
                Vector2 delta = pos - ultimoPointer;
                ultimoPointer = pos;

                pivotInspeccion.Rotate(Vector3.up, -delta.x * rotacionVelocidad * Time.deltaTime, Space.World);
                pivotInspeccion.Rotate(inspectCamera.transform.right, delta.y * rotacionVelocidad * Time.deltaTime, Space.World);
            }
        }
        else arrastrando = false;

        // ZOOM
        float scroll = mouse.scroll.ReadValue().y;

        if (dentro && Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 dir = (pivotInspeccion.position - inspectCamera.transform.position).normalized;

            float dist = Vector3.Distance(inspectCamera.transform.position, pivotInspeccion.position);

            dist -= scroll * zoomStep;
            dist = Mathf.Clamp(dist, distMin, distMax);

            Vector3 nuevaPos = pivotInspeccion.position - dir * dist;

            inspectCamera.transform.position =
                Vector3.Lerp(inspectCamera.transform.position, nuevaPos, Time.deltaTime * 10f);
        }
    }

    // ================= SALIR ====================
    public void Salir()
    {
        inspeccionActiva = false;

        if (clon != null) Destroy(clon);

        inspectCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        canvasInspeccion.SetActive(false);
        canvasHUD.SetActive(true);

        player.SetControlsEnabled(true);

        if (crosshair != null)
            crosshair.SetActive(true);
    }
}
