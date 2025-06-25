using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;    // �����, �� ������� ������� (��������, ������ ������)
    private Vector3 initialOffset;                   // ��������� �������� �� cameraPos �� ������
    private float heightOffset;                      // ������� ������������ ������

    private void Start()
    {
        // ��������� ������� ��������
        initialOffset = transform.position - cameraPos.position;
        heightOffset = initialOffset.y;
    }

    private void Update()
    {
        // ��������� X � Z �������� �����������, � Y � �� heightOffset
        Vector3 newPos = cameraPos.position + new Vector3(initialOffset.x, heightOffset-2.0f, initialOffset.z);
        transform.position = newPos;
    }

    // ���� ����� ���������� �� PlayerMovement
    public void crouch(float newHeight)
    {
        // newHeight � ���, ��������, ������� ������ ������� ������
        heightOffset = newHeight;
    }
}
