using UnityEngine;
using TMPro;

public class MobileTourManager : MonoBehaviour
{
    [Header("Cámara")]
    [SerializeField] private Camera camara;
    [SerializeField] private Transform[] puntosCamara;
    [SerializeField] private float duracionTransicion = 1f;
    [SerializeField] private AnimationCurve curvaTransicion;

    [Header("UI Descripción")]
    [SerializeField] private GameObject panelDescripcion;
    [SerializeField] private TextMeshProUGUI textoTitulo;
    [SerializeField] private TextMeshProUGUI textoDescripcion;

    [System.Serializable]
    public class DatosCuadro
    {
        public string titulo;
        [TextArea(3, 6)]
        public string descripcion;
    }

    [Header("Datos de los 8 cuadros")]
    [SerializeField] private DatosCuadro[] cuadros;

    private int indiceActual = 0;

    // Transición
    private bool enTransicion = false;
    private float tiempoTransicion = 0f;
    private Vector3 posOrigen;
    private Quaternion rotOrigen;
    private Vector3 posDestino;
    private Quaternion rotDestino;

    private void Start()
    {
        if (camara == null)
            camara = Camera.main;

        if (puntosCamara == null || puntosCamara.Length == 0)
        {
            Debug.LogError("MobileTourManager: No hay puntos de cámara asignados.");
            enabled = false;
            return;
        }

        if (cuadros == null || cuadros.Length < puntosCamara.Length)
        {
            Debug.LogWarning("MobileTourManager: Hay menos datos de cuadros que puntos de cámara. Revisa el array 'cuadros'.");
        }

        // Posiciona cámara en el primer punto
        ActualizarCamaraInstantaneo();
        ActualizarDescripcionUI();
        OcultarDescripcion();
    }

    private void Update()
    {
        if (!enTransicion) return;

        tiempoTransicion += Time.deltaTime;
        float t = Mathf.Clamp01(tiempoTransicion / duracionTransicion);

        if (curvaTransicion != null && curvaTransicion.keys.Length > 0)
        {
            t = curvaTransicion.Evaluate(t);
        }
        else
        {
            // Suavizado básico si no asignas curva
            t = Mathf.SmoothStep(0f, 1f, t);
        }

        camara.transform.position = Vector3.Lerp(posOrigen, posDestino, t);
        camara.transform.rotation = Quaternion.Slerp(rotOrigen, rotDestino, t);

        if (t >= 1f)
        {
            enTransicion = false;
        }
    }

    // ------------------ BOTONES --------------------

    public void SiguienteCuadro()
    {
        int nuevo = indiceActual + 1;
        if (nuevo >= puntosCamara.Length)
            nuevo = 0; // vuelve al primero

        CambiarIndice(nuevo);
    }

    public void AnteriorCuadro()
    {
        int nuevo = indiceActual - 1;
        if (nuevo < 0)
            nuevo = puntosCamara.Length - 1; // va al último

        CambiarIndice(nuevo);
    }

    public void ToggleDescripcion()
    {
        if (panelDescripcion == null) return;
        panelDescripcion.SetActive(!panelDescripcion.activeSelf);
    }

    // ------------------ LÓGICA INTERNA --------------------

    private void CambiarIndice(int nuevoIndice)
    {
        if (nuevoIndice == indiceActual) return;

        indiceActual = nuevoIndice;
        IniciarTransicion();
        OcultarDescripcion();
        ActualizarDescripcionUI();
    }

    private void IniciarTransicion()
    {
        if (camara == null) return;

        enTransicion = true;
        tiempoTransicion = 0f;

        posOrigen = camara.transform.position;
        rotOrigen = camara.transform.rotation;

        posDestino = puntosCamara[indiceActual].position;
        rotDestino = puntosCamara[indiceActual].rotation;
    }

    private void ActualizarCamaraInstantaneo()
    {
        if (camara == null) return;

        camara.transform.position = puntosCamara[indiceActual].position;
        camara.transform.rotation = puntosCamara[indiceActual].rotation;
    }

    private void ActualizarDescripcionUI()
    {
        if (cuadros == null || cuadros.Length == 0) return;
        if (indiceActual < 0 || indiceActual >= cuadros.Length) return;

        if (textoTitulo != null)
            textoTitulo.text = cuadros[indiceActual].titulo;

        if (textoDescripcion != null)
            textoDescripcion.text = cuadros[indiceActual].descripcion;
    }

    private void OcultarDescripcion()
    {
        if (panelDescripcion != null)
            panelDescripcion.SetActive(false);
    }
}
