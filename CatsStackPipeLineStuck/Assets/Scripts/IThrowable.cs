using UnityEngine;

public interface IThrowable : ICarryable
{
    void BeginThrow();                     // Optional: state swap before launch (disable snaps, etc.)
    void Throw(Vector2 direction, float force); // Apply launch (you decide units)
    void OnLanded();                       // Optional: called when it comes to rest/snaps
}
