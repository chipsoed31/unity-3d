using System.Collections.Generic;
using UnityEngine;

public class OutlineOnHover : MonoBehaviour
{
    [Header("Ќастройки")]
    public Camera cam;                  // камера дл€ Raycast
    public Material outlineMaterial;    // ссылаемс€ на M_Outline
    public string interactableTag = "Interactable";

    // хранит оригинальные материалы дл€ каждого объекта
    private Dictionary<Renderer, Material[]> originalMats = new Dictionary<Renderer, Material[]>();
    private Renderer currentHovered = null;

    void Start()
    {
        // если не задано вручную Ч берЄм MainCamera
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        // пускаем луч из позиции курсора
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();

            // если под курсором объект с нужным тегом
            if (rend != null && hit.collider.CompareTag(interactableTag))
            {
                if (currentHovered != rend)
                {
                    ClearOutline();      // убрать старый контур
                    ApplyOutline(rend);  // добавить новый
                }
                return;
            }
        }
        // ничего не найдено Ч очищаем
        ClearOutline();
    }

    void ApplyOutline(Renderer rend)
    {
        currentHovered = rend;
        // сохран€ем старые материалы
        originalMats[rend] = rend.sharedMaterials;
        // создаЄм новый массив с дополнительным материалом
        var mats = new List<Material>(rend.sharedMaterials);
        mats.Add(outlineMaterial);
        rend.materials = mats.ToArray();
    }

    void ClearOutline()
    {
        if (currentHovered != null)
        {
            // возвращаем оригиналы
            if (originalMats.TryGetValue(currentHovered, out var mats))
                currentHovered.materials = mats;

            originalMats.Remove(currentHovered);
            currentHovered = null;
        }
    }
}
