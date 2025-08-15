using UnityEngine;


public class SpawnMarker : MonoBehaviour
{

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0));
    }
}
