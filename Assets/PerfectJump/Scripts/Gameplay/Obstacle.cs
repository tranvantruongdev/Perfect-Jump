using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int index;
    public bool hit;

    void OnBecameInvisible()
    {
        if (GameManager.S_Instance.Camera != null)
            if (transform.position.x < GameManager.S_Instance.Camera.transform.position.x || transform.position.y < -10)
                Destroy(gameObject);
    }
}
