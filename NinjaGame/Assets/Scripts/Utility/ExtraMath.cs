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

        //////////////////////////////////////////////////Probability Functions (wiki+stackoverflow)

        /// <summary>
        /// generate a random number where the likelihood of a large number is greater than the likelihood of a small number
        /// </summary>
        /// <param name="rnd">the random number generator used to spawn the number</param>
        /// <returns>the random number</returns>
        public static double InverseBellCurve(System.Random rnd)
        {
            return 1 - BellCurve(rnd);
        }
        /// <summary>
        /// generate a random number where the likelihood of a small number is greater than the likelihood of a Large number
        /// </summary>
        /// <param name="rnd">the random number generator used to spawn the number</param>
        /// <returns>the random number</returns>
        public static double BellCurve(System.Random rnd)
        {
            return  System.Math.Pow(2 * rnd.NextDouble() - 1, 2);
        }
        /// <summary>
        /// generate a random number where the likelihood of a mid range number is greater than the likelihood of a Large or small number
        /// </summary>
        /// <param name="rnd">the random number generator used to spawn the number</param>
        /// <returns>the random number</returns>
        public static double HorizontalBellCurve(System.Random rnd)
        {
            //This is not a real bell curve as using the cube of the value but approximates the result set
            return  (System.Math.Pow(2 * rnd.NextDouble() - 1, 3)/2)+.5;
        }

        //Marsaglia polar method gaussian double (box muller transform)
        public static double RandGaussianDouble(System.Random r)
        {
            double u, v, S;

            do
            {
                u = 2.0 * r.NextDouble() - 1.0;
                v = 2.0 * r.NextDouble() - 1.0;
                S = u * u + v * v;
            }
            while (S >= 1.0);

            double fac = System.Math.Sqrt(-2.0 * System.Math.Log(S) / S);
            return u * fac;
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


        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value){
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        }


        /////////////////////////////////LineInstersection 2d/////////////////

        public static bool Ccw(Vector2 A,Vector2 B,Vector2 C){//counterclockwise triangle
            return (C.y-A.y)*(B.x-A.x) > (B.y-A.y)*(C.x-A.x);
        }

        public static bool IntersectFlat(Vector2 A,Vector2 B,Vector2 C,Vector2 D){//returns if AB intersects CD
            return Ccw(A,C,D) != Ccw(B,C,D) && Ccw(A,B,C) != Ccw(A,B,D);
        }



    }
}
