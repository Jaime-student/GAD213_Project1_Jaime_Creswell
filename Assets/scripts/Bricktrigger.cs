using UnityEngine;

public class Bricktrigger : MonoBehaviour
{
    private PlayerMovement playerMoveSpeed;
    public GameObject player;

    void Start()
    {
        playerMoveSpeed = player.GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerMoveSpeed.moveSpeed = 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerMoveSpeed.moveSpeed = 40f;
        }
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);
    }
}
