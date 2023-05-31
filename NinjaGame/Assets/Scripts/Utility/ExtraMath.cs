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

        public static int DotProdInArray(int[] A, int[] B){//needs to be same length
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

        //https://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
        public static float DistancePointLine2d(Vector2 l_origin, Vector2 dir, Vector2 point){//freya implementation 
            return Mathf.Abs(Determinant2D( dir.normalized, point - l_origin ));//absolute distance
        }

        public static float Determinant2D ( Vector2 a, Vector2 b ) => a.x * b.y - a.y * b.x; // or Cross, 2D "cross product"
        //////////////////////////////Circle Intersection

        public static Vector2[] CirclesIntersect(Vector2 p_0, float r_0, Vector2 p_1, float r_1){//return intersection points
            
            if(Vector2.SqrMagnitude(p_0-p_1)>(r_0+r_1)*(r_0+r_1)){
                return null;//if distance between circles greater than radius, no intersections, return null
            }
            else if(Vector2.SqrMagnitude(p_0-p_1)==(r_0+r_1)*(r_0+r_1)){
                return new Vector2[]{p_0+r_0*((p_1-p_0).normalized)};//one intersection
            }

            float d = (p_1-p_0).magnitude;

            float a = (r_0*r_0-r_1*r_1 + d*d)/(2*d);
            float h = Mathf.Sqrt(r_0*r_0-a*a);

            Vector2 p_2 = p_0 + a*(p_1-p_0)/d;

            Vector2[] p_3 = new Vector2[2];

            p_3[0] =new Vector2(p_2[0]+h*(p_1[1]-p_0[1])/d, p_2[1]-h*(p_1[0]-p_0[0])/d) ;
            p_3[1] =new Vector2(p_2[0]-h*(p_1[1]-p_0[1])/d, p_2[1]+h*(p_1[0]-p_0[0])/d) ;

            return p_3;
        }

        public static Vector2[] LineBetweenCircles(Vector2 p_0, float r_0, Vector2 p_1, float r_1, int dir=1){

            if((p_1-p_0).sqrMagnitude==(r_0+r_1)*(r_0+r_1)){
                Vector2 intersect = p_1+r_1*(p_0-p_1).normalized;
                return new Vector2[]{intersect};
            }

            //dir = -1; clockwise, dir = 1; counterclockwise
            Vector2 p_mid = p_1-p_0;
            float p_mag = p_mid.magnitude;

            float angle_0 = Mathf.Acos((r_0+r_1)/p_mag);
            //float angle_1 = Mathf.Asin((r_0+r_1)/p_mag);

            Vector2 midRotated_0 =r_0*RotationMatrix2d(p_mid,-dir*angle_0).normalized;
            Vector2 midRotated_1 = r_1*RotationMatrix2d(-p_mid,-dir*angle_0).normalized;
            
            return new Vector2[]{p_0+midRotated_0,p_1+midRotated_1};
        }

        public static Vector2[] LineCirclePoint(Vector2 p_0, float r_0, Vector2 p_1, int dir = 1){
            Vector2 p_mid = p_1-p_0;
            float p_mag = p_mid.magnitude;

            float angle_0 = Mathf.Acos(r_0/p_mag);
            Vector2 midRotated_0 =r_0*RotationMatrix2d(p_mid,-dir*angle_0).normalized;

            return new Vector2[]{p_0+midRotated_0, p_1};

        }

        public static Vector2 RotationMatrix2d(Vector2 v_init, float angle){//pos = counterclock angle
            return new Vector2(v_init.x*Mathf.Cos(angle)-v_init.y*Mathf.Sin(angle),v_init.x*Mathf.Sin(angle)+v_init.y*Mathf.Cos(angle)); 
        }


        public static float ArcLength2D(Vector2 p_A, Vector2 p_B, float r){
            float c_Len = (p_A-p_B).magnitude;

            if(c_Len==r){
                return Mathf.PI*r;
            }

            float arc_Angle = 2*Mathf.Atan(c_Len/(2*r));//angle in radian
            
            return arc_Angle*r;

        }

        public static Vector2[] GetPointsInArc(Vector2 p_0, float r_0,Vector2 p_A, Vector2 p_B, bool clockWiseDir){
            float minDist = 2f;
            float minAngle = 2*Mathf.Asin(minDist/(2*r_0))*Mathf.Rad2Deg;
            minAngle = Mathf.Max(30,minAngle);
            int dir = 1;
            if(clockWiseDir){
                dir = -1;
            }

            //dir = 1: counterclock, dir = -1 clock
            float c_Len = (p_A-p_B).magnitude;
            float arc_Angle = Vector2.Angle(p_A-p_0,p_B-p_0);//always between -180 and 180

            //clockwise ab = negative, counterclock ab = positive

            if(Ccw(p_A,p_B,p_0)==clockWiseDir){
                arc_Angle = 360-arc_Angle; 
            }

            int numPoints = (int)(arc_Angle/minAngle);//rounds down
            //ex: 60, 3 points (20,40,60),last point removed,2 points between (20,40)
            //10, 0 points
            //20, 1 point(20)
            

            float angleBetweenPoints = dir*arc_Angle/numPoints;

            Vector2[] arcPoints = new Vector2[Mathf.Max(0,numPoints-1)];
            
            float startAngle = Vector2.SignedAngle(Vector2.right,p_A-p_0);
            if(startAngle<0){
                startAngle = 360+startAngle;
            }
            for (int i = 1; i < numPoints; i++)
            {
                arcPoints[i-1] = GetUnitOnCircle(startAngle+i*angleBetweenPoints,r_0)+p_0;
            }

            return arcPoints;
        }

        public static Vector2 GetUnitOnCircle(float angleDeg, float radius){//returns -center
        
            // initialize calculation variables
            float _x = 0;
            float _y = 0;
            Vector2 _returnVector;
            
            float angleRadians = angleDeg*Mathf.Deg2Rad;

            // get the 2D dimensional coordinates
            _x = radius * Mathf.Cos(angleRadians);
            _y = radius * Mathf.Sin(angleRadians);
        
            // derive the 2D vector
            _returnVector = new Vector2(_x, _y);
        
            // return the vector info
            return _returnVector;
        }

        public static Vector3 AverageVect(Vector3 v_1, Vector3 v_2){//aka midpoint of line
            return (v_1+v_2)/2;
        }

        public static Vector2 GradientOfLine3D(Vector3 p1, Vector3 p2){//y vertical, xz horizontal
            Debug.Log("verify use");
            return new Vector2( (p2.y - p1.y) / (p2.x - p1.x), (p2.y - p1.y) / (p2.z - p1.z));
        }//multiply by displacement x/z to know how much y moved?
        //project on 2d plane of horizontal normal to do 2d gradient?
        
        public static float GradientOfLine2D(Vector2 p1, Vector2 p2){
            return (p2.y - p1.y) / (p2.x - p1.x);
        }

    }
}
