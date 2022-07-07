using UnityEngine;

namespace Roguelike.Extension
{
    public class Extensions : MonoBehaviour
    {
        public static T GetRandomEnum<T>()
        {
            System.Array A = System.Enum.GetValues(typeof(T));
            T V = (T)A.GetValue(Random.Range(0, A.Length));
            return V;
        }
    }
}

