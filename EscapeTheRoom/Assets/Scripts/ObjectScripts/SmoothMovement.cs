using UnityEngine;

public class SmoothMovement : MonoBehaviour
{

    private Vector3 destination;
    private float timeLeft;

    private void Start()
    {
        destination = transform.position;
        timeLeft = 0.0f;
    }

    public void Move(Vector3 destination, float inTime)
    {
        this.destination = destination;
        timeLeft = inTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if (destination != transform.position)
        {
            Vector3 currentPosition = gameObject.transform.position;
            float distance = Vector3.Distance(currentPosition, destination);
            if (timeLeft != 0.0f)
            {
                gameObject.transform.position = Vector3.MoveTowards(currentPosition, destination, distance / timeLeft);
            }
            else
            {
                gameObject.transform.position = destination;
            }
            timeLeft -= Time.deltaTime;
        }
    }
}
