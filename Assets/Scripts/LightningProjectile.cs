// LightningProjectile.cs
// Скрипт для префаба молнии: мгновенный взрыв, прозрачность и авто-удаление
using UnityEngine;

public class LightningProjectile : MonoBehaviour
{
    public GameObject smallExplosionEffect; // Префаб небольшого взрыва
    public float explosionRadius = 2f;
    public float explosionForce = 300f;
    public float lifeTime = 1f;
    public AudioClip explosionSound;  // Звук взрыва

    private float timer;
    //private Renderer rend;
    //private Color initialColor;

    void Start()
    {
        // Создаем эффект взрыва сразу при появлении
        //if (smallExplosionEffect != null)
        //    Instantiate(smallExplosionEffect, transform.position, Quaternion.identity);
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        // Физический взрыв
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            AudioSource.PlayClipAtPoint(explosionSound, hit.transform.position);
        }

        // Инициализация прозрачности
        //rend = GetComponent<Renderer>();
        //if (rend != null)
        //    initialColor = rend.material.color;

        timer = lifeTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        //if (rend != null && timer > 0f)
        //{
        //    float alpha = timer / lifeTime;
        //    Color c = initialColor;
        //    c.a = alpha;
        //    rend.material.color = c;
        //}

        if (timer <= 0f)
            Destroy(gameObject);
    }
}