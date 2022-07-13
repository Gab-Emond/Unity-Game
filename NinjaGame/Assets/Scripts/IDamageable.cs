using UnityEngine;
using System.Collections;

/*
An interface contains definitions for a group of related functionalities that a non-abstract class or a struct must implement. 
An interface may define static methods, which must have an implementation
*/
//basically mini inheritance, also c# can both inherit and interface (inherit class before interface)
//<T>
public interface IDamageable
{
    //void Damage(T damageTaken);
    void TakeHit();
}