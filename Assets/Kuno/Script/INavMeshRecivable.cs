using Unity.AI.Navigation;
using UnityEngine;

public interface INavMeshRecivable
{
    void SetNavMeshSurface(NavMeshSurface navMeshSurface, Transform transform);
}
