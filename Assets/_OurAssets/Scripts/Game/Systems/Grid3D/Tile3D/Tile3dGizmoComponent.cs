using UnityEngine;

namespace CursedOnion
{
    public class Tile3dGizmoComponent : MonoBehaviour
    {
        public Vector3 size = Vector3.one;
        public Color color = Color.green;
        void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawCube(transform.position, size);
        }
    }
}
