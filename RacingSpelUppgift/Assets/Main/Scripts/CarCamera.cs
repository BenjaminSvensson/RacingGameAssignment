using UnityEngine;

public class CarCamera : MonoBehaviour //CHATGPT!
{
    [SerializeField] Rigidbody targetRb;     // Car's Rigidbody
    [SerializeField] Vector3 offset = new Vector3(0f, 3f, -6f);
    [SerializeField] float followSmooth = 5f;
    [SerializeField] float rotationSmooth = 3f;

    void LateUpdate()
    {
        if (!targetRb) return;

        // Use Rigidbody.position (interpolated) instead of transform.position
        Vector3 desiredPos = targetRb.transform.TransformPoint(offset);

        // Smoothly move camera
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * followSmooth
        );

        // Desired rotation: look at the car
        Quaternion desiredRot = Quaternion.LookRotation(
            targetRb.position - transform.position,
            Vector3.up
        );

        // Smoothly rotate with lag
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            Time.deltaTime * rotationSmooth
        );
    }
}
