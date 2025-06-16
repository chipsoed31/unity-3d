// SpellManager.cs
// Этот скрипт отвечает за слоты магии и оружия и переключение между ними
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
public class SpellManager : MonoBehaviour
{
    [Header("Настройки слотов")]
    // Слот для фаербола
    public string fireballSlotKey = "1"; // Клавиша для выбора/снятия фаербола
    public string rainSlotKey = "2"; // Клавиша для выбора/снятия молнии

    [Header("Настройки")]
    // Слот для фаербола
    public float speed = 25;

    [Header("Система маны")]
    public float maxMana = 100f;       // Максимум маны
    public float manaRegenRate = 5f;   // Восстановление маны в секунду
    public float fireballManaCost = 20f; // Стоимость фаербола
    public float rainManaCost = 20f; // Стоимость молнии

    [Header("Звуки")]
    public AudioClip fireballCastSound;
    public AudioClip rainCastSound;

    [Header("Кулдаун")]
    public float fireballCooldown = 1.5f; // Время между бросками
    public float rainCooldown = 0.7f;

    [Header("Префабы")]
    public GameObject fireballPrefab;   // Префаб фаербола, который будет вылетать
    public Transform firePoint;         // Точка, откуда появляется фаербол
    public GameObject lightningPrefab;   // Префаб заряда молнии (эффект)
    public GameObject sparklesPrefab;
    public float lightningMaxDistance = 100f;

    [Header("Sliders")]
    public Slider manaSlider;

    private bool fireballSelected = false; // Фаербол выбран или нет
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

    // Обработка нажатия клавиши для выбора/снятия фаербола
    void HandleSlotInput()
    {
        if (Input.GetKeyDown(fireballSlotKey))
        {

            fireballSelected = !fireballSelected; // Переключаем состояние
            rainSelected = false;
            Debug.Log(fireballSelected ? "Фаербол выбран" : "Фаербол убран");
        }
        if (Input.GetKeyDown(rainSlotKey))
        {
            rainSelected = !rainSelected;
            fireballSelected = false;
            Debug.Log(rainSelected ? "Молния выбрана" : "Молния убрана");
        }
    }

    // Обработка выстрела фаерболом при нажатии ЛКМ
    void HandleCastInput()
    {
        if (fireballSelected && Input.GetMouseButtonDown(0) && fireballCooldownTimer <= 0f && currentMana >= fireballManaCost)
            CastFireball();

        if (rainSelected && Input.GetMouseButtonDown(0) && fireballCooldownTimer <= 0f && currentMana >= rainManaCost)
            CastRain();


    }

    // Создание и запуск фаербола
    void CastFireball()
    {
        currentMana -= fireballManaCost;
        fireballCooldownTimer = fireballCooldown;

        if (fireballCastSound != null)
            audioSource.PlayOneShot(fireballCastSound);

        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Не задан префаб или точка выстрела!");
            return;
        }

        // Создаем экземпляр фаербола
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        

        // Направление полета по направлению взгляда камеры
        Vector3 direction = Camera.main.transform.forward;

        // Если у фаербола есть Rigidbody, сообщаем ему импульс
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