using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlataformaMovilVertical : MonoBehaviour
{
    [Tooltip("Distancia vertical que se moverá la plataforma (positiva = hacia arriba, negativa = hacia abajo)")]
    public float alturaDesplazamiento = 3f;

    public float velocidad = 2f;

    private Rigidbody rb;
    private Vector3 puntoA;
    private Vector3 puntoB;
    private Vector3 destinoActual;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        puntoA = transform.position;
        puntoB = puntoA + Vector3.up * alturaDesplazamiento;

        destinoActual = puntoB;
    }

    void FixedUpdate()
    {
        Vector3 nuevaPos = Vector3.MoveTowards(transform.position, destinoActual, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(nuevaPos);

        if (Vector3.Distance(transform.position, destinoActual) < 0.01f)
        {
            destinoActual = (destinoActual == puntoA) ? puntoB : puntoA;
        }
    }
}
