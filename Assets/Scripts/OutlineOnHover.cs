using System.Collections.Generic;
using UnityEngine;

public class OutlineOnHover : MonoBehaviour
{
    [Header("���������")]
    public Camera cam;                  // ������ ��� Raycast
    public Material outlineMaterial;    // ��������� �� M_Outline
    public string interactableTag = "Interactable";

    // ������ ������������ ��������� ��� ������� �������
    private Dictionary<Renderer, Material[]> originalMats = new Dictionary<Renderer, Material[]>();
    private Renderer currentHovered = null;

    void Start()
    {
        // ���� �� ������ ������� � ���� MainCamera
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        // ������� ��� �� ������� �������
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();

            // ���� ��� �������� ������ � ������ �����
            if (rend != null && hit.collider.CompareTag(interactableTag))
            {
                if (currentHovered != rend)
                {
                    ClearOutline();      // ������ ������ ������
                    ApplyOutline(rend);  // �������� �����
                }
                return;
            }
        }
        // ������ �� ������� � �������
        ClearOutline();
    }

    void ApplyOutline(Renderer rend)
    {
        currentHovered = rend;
        // ��������� ������ ���������
        originalMats[rend] = rend.sharedMaterials;
        // ������ ����� ������ � �������������� ����������
        var mats = new List<Material>(rend.sharedMaterials);
        mats.Add(outlineMaterial);
        rend.materials = mats.ToArray();
    }

    void ClearOutline()
    {
        if (currentHovered != null)
        {
            // ���������� ���������
            if (originalMats.TryGetValue(currentHovered, out var mats))
                currentHovered.materials = mats;

            originalMats.Remove(currentHovered);
            currentHovered = null;
        }
    }
}
