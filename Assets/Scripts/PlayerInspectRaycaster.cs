using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInspectRaycaster : MonoBehaviour
{
    [Header("Referencias")]
    public Camera cam;
    public float distancia = 3f;
    public LayerMask capaInteractivos;

    [Header("UI PC")]
    public GameObject TextoInteractuar;

    private ObjetoInteractivo actual;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Start()
    {
        if (TextoInteractuar != null)
            TextoInteractuar.SetActive(false);

        StartCoroutine(FixHUD());
    }

    System.Collections.IEnumerator FixHUD()
    {
        yield return null;

        if (TextoInteractuar != null)
            TextoInteractuar.SetActive(false);
    }

    void Update()
    {
        Detectar();

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            Intentar();
    }

    void Detectar()
    {
        actual = null;

        Ray r = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(r, out RaycastHit hit, distancia, capaInteractivos))
        {
            actual = hit.collider.GetComponent<ObjetoInteractivo>();
        }

        if (TextoInteractuar != null)
            TextoInteractuar.SetActive(actual != null);
    }

    void Intentar()
    {
        if (actual != null)
            actual.IniciarInspeccion();
    }
}
