using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el SoundManager entre escenas
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip audioClip, bool randomPitch = false)
    {
        if (randomPitch)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Rango ajustable según el efecto deseado
        }
        else
        {
            audioSource.pitch = 1f; // Pitch normal
        }

        audioSource.PlayOneShot(audioClip);
    }

    // Reproducir un sonido aleatorio desde una lista de sonidos
    public void PlayRandomSound(AudioClip[] clips)
    {
        if (clips.Length == 0) return;
        
        AudioClip selectedClip = clips[Random.Range(0, clips.Length)];

        audioSource.PlayOneShot(selectedClip);
    }
}
