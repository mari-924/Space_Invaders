using UnityEngine;
using UnityEngine.SceneManagement;

public class Bunker : MonoBehaviour
{

    private int health = 3;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            health--;
        }else if (other.gameObject.layer == LayerMask.NameToLayer("Missile"))
        {
            health--;
        }else if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            health--;
        }

        if(health <= 0)
        {
            this.gameObject.SetActive(false);
        }


    }
}
