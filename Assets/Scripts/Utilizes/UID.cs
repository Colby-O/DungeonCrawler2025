using UnityEngine;

namespace DC2025
{
    public class UID : MonoBehaviour
    {
        public static int GetID(Transform obj)
        {
            int a = 0;
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.Append(obj.name);
            Transform parent = obj.parent;
            a += obj.GetSiblingIndex();
            while (parent != null)
            {
                s.Append(parent.name);
                a += parent.GetSiblingIndex();
                parent = parent.parent;
            }

            string name = s.ToString();
            char[] c = name.ToCharArray();
            foreach (var item in c)
            {
                a += (int)item;
            }

            a += obj.childCount;

            return a;
        }
    }
}
