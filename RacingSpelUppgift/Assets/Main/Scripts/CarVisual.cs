using UnityEngine;
public class CarVisual : MonoBehaviour
{
    [SerializeField] CarScript carController;
    [SerializeField] GameObject[] carWheels;
    private float spinSpeed;


    private void Update()
    {
        if (carController.moving == true)
        {
            spinWheels();
        }
    }

    private void spinWheels()
    {
        spinSpeed = carController.speed;
        foreach (GameObject wheel in carWheels)
        {
            wheel.transform.Rotate(spinSpeed, 0, 0);
        }
    }
}



/* Använder inte längre

[SerializeField] GameObject playerMain;
[SerializeField] Vector3 carOffset;
private Vector3 targetPosition;

public void Update()
{
    targetPosition = playerMain.transform.position + carOffset;
    transform.position = targetPosition;
}
*/
