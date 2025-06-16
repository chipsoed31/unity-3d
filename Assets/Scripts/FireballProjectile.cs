// FireballProjectile.cs
// ���� ������ ������������ �� ������ �������� � �������� �� ������������ � �����
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public GameObject explosionEffect; // ������ ������� ������

    [Header("���������")]
    public float explosionRadius = 4f;  // ������ ���������
    public float explosionForce = 1500f; // ���� ������
    public float lifeTime = 5f;         // ����� ����� ��������, ���� �� ������ �� �����
    public AudioClip explosionSound;  // ���� ������


    void Start()
    {
        // ������������ ����� ����� lifeTime
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // ������� ���������� ������ ������
        if (explosionEffect != null)
        {
            GameObject explode = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explode, 2.0f);
        }

        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        // ���������� ����������� �� ������� ������
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // ������� �������
        Destroy(gameObject);
    }
}
