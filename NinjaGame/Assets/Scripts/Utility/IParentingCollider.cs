using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParentingCollider
{
    void OnChildTriggerEnter(Collider collider);

    void OnChildTriggerExit(Collider collider);
}