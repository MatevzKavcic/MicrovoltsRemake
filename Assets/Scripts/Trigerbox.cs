using UnityEngine;

public class Trigerbox : MonoBehaviour
{
    public MeleeWeapon owner;

    private void OnTriggerEnter(Collider other) // bypass da lahko dostopas do onTriggerEnter
    {
        owner?.HandleHit(other);
    }
}