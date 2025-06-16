// FireballProjectile.cs
// Этот скрипт навешивается на префаб фаербола и отвечает за столкновения и взрыв
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public GameObject explosionEffect; // Префаб эффекта взрыва

    [Header("Настройки")]
    public float explosionRadius = 4f;  // Радиус поражения
    public float explosionForce = 1500f; // Сила взрыва
    public float lifeTime = 5f;         // Время жизни фаербола, если он никуда не попал
    public AudioClip explosionSound;  // Звук взрыва


    void Start()
    {
        // Автоудаление через время lifeTime
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // Создаем визуальный эффект взрыва
        if (explosionEffect != null)
        {
            GameObject explode = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explode, 2.0f);
        }

        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        // Физическое воздействие на объекты вокруг
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Удаляем фаербол
        Destroy(gameObject);
    }
}
