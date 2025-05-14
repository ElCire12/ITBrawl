using UnityEngine;

public class GorraScript : MonoBehaviour
{
    public float distancia = 1f;       // Distancia que se moverá hacia arriba
    public float duracion = 0.5f;      // Duración del movimiento

    private Vector3 posicionInicial;

    private void OnEnable()
    {
        posicionInicial = transform.position;
        StopAllCoroutines(); // Por si se vuelve a activar rápidamente
        StartCoroutine(MoverHaciaArriba());
    }

    private System.Collections.IEnumerator MoverHaciaArriba()
    {
        Vector3 objetivo = posicionInicial + Vector3.up * distancia;
        float t = 0f;

        while (t < duracion)
        {
            t += Time.deltaTime;
            float progreso = t / duracion;
            transform.position = Vector3.Lerp(posicionInicial, objetivo, progreso);
            yield return null;
        }

        transform.position = objetivo;
        Destroy(this.gameObject);
    }
}
