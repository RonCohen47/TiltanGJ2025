using UnityEngine;

public interface ICarryable
{
    AssignmentSO Data { get; }
    void AttachToParent(Transform parent);
    void Detach();
}
