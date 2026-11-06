using UnityEngine;

public class MobExistanceController : MonoBehaviour
{
    public Collider2D ExistanceCollider;

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.GetComponent<EnemyClass>() != null)
        {
            collider.GetComponent<EnemyClass>().KillObject();
        }
    }
}
