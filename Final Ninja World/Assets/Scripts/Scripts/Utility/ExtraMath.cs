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

        public static int DotProdInArray(int[] A, int[] B){
            int sum = 0;
            
            for(int i = 0; i<A.Length;i++){
                sum += A[i]*B[i];
            }
            
            return sum;
        }

        public static float SigmoidFunct(float x, float maxVal = 1, float center = 0, float slope = -1){
            return maxVal/(1.0f + Mathf.Exp(slope*(x-center)));
        }


        /////////////////////////////////////////////////Sebastian Extra Methods for pathfinder
        
        //smoothstep for vectors, from unity tips
        public static Vector3 SmoothStep(Vector3 posInit, Vector3 posFinal, float time){
            
            Vector3 position = Vector3.Lerp(posInit, posFinal, Mathf.SmoothStep(0f,1f,time));

            return position;

        }
        
        //note smoothstep = 3x**2 - 2x**3
        
        public static Vector3 QuadFriction(Vector3 velocity, float coef = 0.5f){////cst = density diff* volume
            Vector3 frictVel = Vector3.zero;
            float frict = coef * Vector3.SqrMagnitude(velocity) * Time.deltaTime/2; 
            //F_r = c* v**2  
            frictVel = -frict*velocity/Vector3.Magnitude(velocity);

            return frictVel;
        }

        public static Vector3 LinFriction(Vector3 velocity, float coef = 0.5f){
            Vector3 frictVel = Vector3.zero;
            //F_r = c* v   
            frictVel = -coef*velocity* Time.deltaTime;
            return frictVel;
        }

    }
}
