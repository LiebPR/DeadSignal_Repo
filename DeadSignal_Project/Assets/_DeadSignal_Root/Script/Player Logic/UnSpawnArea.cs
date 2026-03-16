using UnityEngine;

public class UnSpawnArea : MonoBehaviour
{
    [SerializeField] float radius = 3f;
    public float Radius => radius;

    public bool Contains(Vector3 point)
    {
        return Vector3.Distance(transform.position, point) <= radius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
