using UnityEngine;

public class PreviewCameraOrbit : MonoBehaviour
{
    public Transform target; // El coche a mirar
    public float sensitivity = 3f;
    public float distance = 5f;  // Distancia a la que se coloca la cámara

    private float yaw = 0f;
    private float pitch = 15f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Girar con ratón
        if (Input.GetMouseButton(0))  // click izquierdo
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity;
            pitch = Mathf.Clamp(pitch, 5f, 60f);
        }

        // Calculamos la nueva posición de cámara
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        transform.position = target.position + rotation * direction;
        transform.LookAt(target);
    }
}
