using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.Math
{//https://docs.unity3d.com/Manual/Namespaces.html

//https://en.wikibooks.org/wiki/C_Sharp_Programming/Namespaces
    public static class MathUtility
    {
    
        public static void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart){
            float a = radius * Mathf.Cos(elevation);
            outCart.x = a * Mathf.Cos(polar);
            outCart.y = radius * Mathf.Sin(elevation);
            outCart.z = a * Mathf.Sin(polar);
        }
        
        public static void CartesianToSpherical(Vector3 cartCoords, out float outRadius, out float outPolar, out float outElevation){
            if (cartCoords.x == 0)
                cartCoords.x = Mathf.Epsilon;
            outRadius = Mathf.Sqrt((cartCoords.x * cartCoords.x)
                            + (cartCoords.y * cartCoords.y)
                            + (cartCoords.z * cartCoords.z));
            outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
            if (cartCoords.x < 0)
                outPolar += Mathf.PI;
            outElevation = Mathf.Asin(cartCoords.y / outRadius);
        }
        //out: has to give values to outcart (resulting pos)

        public static int dotProdIntArr(int[] A, int[] B){
            int sum = 0;
            
            for(int i = 0; i<A.Length;i++){
                sum += A[i]*B[i];
            }
            
            return sum;
        }


        /////////////////////////////////////////////////Sebastian Extra Methods for pathfinder
        



    }
}
