// LightningProjectile.cs
// ������ ��� ������� ������: ���������� �����, ������������ � ����-��������
using UnityEngine;

public class LightningProjectile : MonoBehaviour
{
    public GameObject smallExplosionEffect; // ������ ���������� ������
    public float explosionRadius = 2f;
    public float explosionForce = 300f;
    public float lifeTime = 1f;
    public AudioClip explosionSound;  // ���� ������

    private float timer;
    //private Renderer rend;
    //private Color initialColor;

    void Start()
    {
        // ������� ������ ������ ����� ��� ���������
        //if (smallExplosionEffect != null)
        //    Instantiate(smallExplosionEffect, transform.position, Quaternion.identity);
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        // ���������� �����
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            AudioSource.PlayClipAtPoint(explosionSound, hit.transform.position);
        }

        // ������������� ������������
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