using UnityEngine;

[ExecuteAlways]  // So it works in Edit mode too
public class ShowBoundingBox : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
