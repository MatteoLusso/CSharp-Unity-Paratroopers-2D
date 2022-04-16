using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public class Algebra
    {
        public static float Determinant(Vector3 CoeffLine1, Vector3 CoeffLine2)
        {
            return (CoeffLine1.x * CoeffLine2.y) - (CoeffLine2.x * CoeffLine1.y);
        }
        public static Vector2 IntersectionPointBetweenTwoLines(Vector3 CoeffLine1, Vector3 CoeffLine2, float Determinant)
        {
            float XCoord = ((CoeffLine2.y * CoeffLine1.z) - (CoeffLine1.y * CoeffLine2.z)) / Determinant;
            float YCoord = ((CoeffLine1.x * CoeffLine2.z) - (CoeffLine2.x * CoeffLine1.z)) / Determinant;

            return new Vector2(XCoord, YCoord);
        }
        public static Vector3 LinePassingThroughTwoPoints(Vector2 PointA, Vector2 PointB) // A1*x + B1*y = C1 -> Returns [A1, B1, C1] <-> [x, y, z]
        {
            float A1 = PointB.y - PointA.y;
            float B1 = PointA.x - PointB.x;
            float C1 = (A1 * PointA.x) + (B1 * PointA.y);

            return new Vector3(A1, B1, C1);
        }
    }
    public class Angles
    {
        public static float SignedAngleTo360Angle(float input_SignedAngle)
        {
            if(input_SignedAngle < 0.0f)
            {
                input_SignedAngle = 360.0f + input_SignedAngle;
            }
            return input_SignedAngle;
        }

        public static Utilities.Screen.Side IconSide(float angle_World, float angle_UI)
        {
            if(angle_World >= 360.0f - angle_UI || angle_World < angle_UI)
            {
                ////Debug.Log("Destra");
                return Utilities.Screen.Side.Right;
            }
            else if(angle_World >= angle_UI && angle_World < 180 - angle_UI)
            {
                ////Debug.Log("Sopra");
                return Utilities.Screen.Side.Up;
            }
            else if(angle_World >= 180 - angle_UI && angle_World < 180 + angle_UI)
            {
                ////Debug.Log("Sinistra");
                return Utilities.Screen.Side.Left;
            }
            else if(angle_World >= 180 + angle_UI && angle_World < 360 - angle_UI)
            {
                ////Debug.Log("Sotto");
                return Utilities.Screen.Side.Down;
            }
            else
            {
                return Utilities.Screen.Side.None;
            }
        }
    }

    public static class Screen
    {
        public enum Side {None, Right, Down, Left, Up};
    }
}
