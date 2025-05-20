using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Sphere")
        {
            Debug.Log("Trigger Enter");
        }
    }
}
