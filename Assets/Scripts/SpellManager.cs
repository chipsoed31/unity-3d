// SpellManager.cs
// ���� ������ �������� �� ����� ����� � ������ � ������������ ����� ����
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
public class SpellManager : MonoBehaviour
{
    [Header("��������� ������")]
    // ���� ��� ��������
    public string fireballSlotKey = "1"; // ������� ��� ������/������ ��������
    public string rainSlotKey = "2"; // ������� ��� ������/������ ������

    [Header("���������")]
    // ���� ��� ��������
    public float speed = 25;

    [Header("������� ����")]
    public float maxMana = 100f;       // �������� ����
    public float manaRegenRate = 5f;   // �������������� ���� � �������
    public float fireballManaCost = 20f; // ��������� ��������
    public float rainManaCost = 20f; // ��������� ������

    [Header("�����")]
    public AudioClip fireballCastSound;
    public AudioClip rainCastSound;

    [Header("�������")]
    public float fireballCooldown = 1.5f; // ����� ����� ��������
    public float rainCooldown = 0.7f;

    [Header("�������")]
    public GameObject fireballPrefab;   // ������ ��������, ������� ����� ��������
    public Transform firePoint;         // �����, ������ ���������� �������
    public GameObject lightningPrefab;   // ������ ������ ������ (������)
    public GameObject sparklesPrefab;
    public float lightningMaxDistance = 100f;

    [Header("Sliders")]
    public Slider manaSlider;

    private bool fireballSelected = false; // ������� ������ ��� ���
    private bool rainSelected = false;
    private float currentMana;
    private float fireballCooldownTimer = 0f;
    private float rainCooldownTimer = 0f;
    private AudioSource audioSource;

    void Start()
    {
        currentMana = maxMana;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        HandleSlotInput();
        RegenerateMana();
        UpdateCooldown();
        HandleCastInput();
        UpdateUI();
        
    }

    // ��������� ������� ������� ��� ������/������ ��������
    void HandleSlotInput()
    {
        if (Input.GetKeyDown(fireballSlotKey))
        {

            fireballSelected = !fireballSelected; // ����������� ���������
            rainSelected = false;
            Debug.Log(fireballSelected ? "������� ������" : "������� �����");
        }
        if (Input.GetKeyDown(rainSlotKey))
        {
            rainSelected = !rainSelected;
            fireballSelected = false;
            Debug.Log(rainSelected ? "������ �������" : "������ ������");
        }
    }

    // ��������� �������� ��������� ��� ������� ���
    void HandleCastInput()
    {
        if (fireballSelected && Input.GetMouseButtonDown(0) && fireballCooldownTimer <= 0f && currentMana >= fireballManaCost)
            CastFireball();

        if (rainSelected && Input.GetMouseButtonDown(0) && fireballCooldownTimer <= 0f && currentMana >= rainManaCost)
            CastRain();


    }

    // �������� � ������ ��������
    void CastFireball()
    {
        currentMana -= fireballManaCost;
        fireballCooldownTimer = fireballCooldown;

        if (fireballCastSound != null)
            audioSource.PlayOneShot(fireballCastSound);

        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("�� ����� ������ ��� ����� ��������!");
            return;
        }

        // ������� ��������� ��������
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        

        // ����������� ������ �� ����������� ������� ������
        Vector3 direction = Camera.main.transform.forward;

        // ���� � �������� ���� Rigidbody, �������� ��� �������
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            
            rb.linearVelocity = direction * speed;
        }
    }

    void CastRain()
    {
        currentMana -= rainManaCost;
        rainCooldownTimer = rainCooldown;

        currentMana -= rainManaCost;
        rainCooldownTimer = rainCooldown;

        if (rainCastSound != null)
            audioSource.PlayOneShot(rainCastSound);

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, lightningMaxDistance))
        {
            GameObject lightning = Instantiate(lightningPrefab, firePoint.position, Quaternion.identity);

            float t = 100 * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, hit.point, t);
            Destroy(lightning, 0.3f);

            GameObject sparkles = Instantiate(sparklesPrefab, hit.point, Quaternion.identity);
            Destroy(sparkles, 0.5f);
        }
        else
        {
            Vector3 targetPoint = Camera.main.transform.position + Camera.main.transform.forward * lightningMaxDistance;
            GameObject lightning =  Instantiate(lightningPrefab, targetPoint, Quaternion.identity);
            Destroy(lightning, 0.5f);
            GameObject sparkles = Instantiate(sparklesPrefab, targetPoint, Quaternion.identity);
            Destroy(sparkles, 0.5f);
        }

    }

    void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }

    void UpdateCooldown()
    {
        if (fireballCooldownTimer > 0f)
            fireballCooldownTimer -= Time.deltaTime;
        if (rainCooldownTimer > 0f)
            rainCooldownTimer -= Time.deltaTime;
    }

    void UpdateUI()
    {
        manaSlider.value = currentMana;
    }
}