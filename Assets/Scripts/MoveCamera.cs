using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;    // точка, за которой смотрим (например, голова игрока)
    private Vector3 initialOffset;                   // начальное смещение от cameraPos до камеры
    private float heightOffset;                      // текущий вертикальный оффсет

    private void Start()
    {
        // сохран€ем базовое смещение
        initialOffset = transform.position - cameraPos.position;
        heightOffset = initialOffset.y;
    }

    private void Update()
    {
        // сохран€ем X и Z смещени€ неизменными, а Y Ч из heightOffset
        Vector3 newPos = cameraPos.position + new Vector3(initialOffset.x, heightOffset-2.0f, initialOffset.z);
        transform.position = newPos;
    }

    // этот метод вызываетс€ из PlayerMovement
    public void crouch(float newHeight)
    {
        // newHeight Ч это, например, текуща€ высота капсулы игрока
        heightOffset = newHeight;
    }
}
