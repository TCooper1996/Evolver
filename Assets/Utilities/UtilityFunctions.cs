using System;
using UnityEngine;

namespace Utilities
{
    public static class UtilityFunctions
    {
        //Returns angle of vector in radians
        public static float ToAngle(Vector2 vec)
        {
            return (float)Math.Atan2(vec.y, vec.x);
        }

        public static (float, float) UnpackVector(Vector2 vec)
        {
            return (vec.x, vec.y);
        }
        
        public static Func<Vector2, Vector2, Vector2> GetPredictionFunction(float projectileSpeed)
        {
            //This algorithm is credited to James McNeill from https://playtechs.blogspot.com/2007/04/aiming-at-moving-target.html
            Vector2 GetPredictedPosition(Vector2 pos, Vector2 vel)
            {
                //Get the predicted position of a target by solving the quadratic 
                double a = Math.Pow(projectileSpeed, 2) - Math.Pow(vel[0], 2) + Math.Pow(vel[1], 2);
                float b = pos[0] * vel[0] + pos[1] * vel[1];
                float c = pos[0] * pos[0] + pos[1] * pos[1];
                double d = Math.Pow(b, 2) + a * c;
                double t = 0.0;
                if (d >= 0)
                {
                    t = (b + Math.Sqrt(d)) / a;
                    if (t < 0)
                        t = 0;
                }

                return new Vector2((float) (pos.x + vel.x * t), (float) (pos.y + vel.y * t));
            }

            return GetPredictedPosition;
        }
    }
    
}