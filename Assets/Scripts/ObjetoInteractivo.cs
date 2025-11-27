using UnityEngine;

public class ObjetoInteractivo : MonoBehaviour
{
    [TextArea(4,8)]
    public string descripcion;

    [Header("Referencia al objeto visual real")]
    public GameObject objetoReal;

    private InspectManager inspector;

    void Awake()
    {
        inspector = FindObjectOfType<InspectManager>();
    }

    public void IniciarInspeccion()
    {
        if (objetoReal == null)
        {
            Debug.LogError("[ObjetoInteractivo] Falta objetoReal asignado.");
            return;
        }

        inspector.Iniciar(objetoReal, descripcion);
    }
}
