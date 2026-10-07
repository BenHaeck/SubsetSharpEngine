using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Engine {
    public static class Utils {
        // gets an entity of a certain type
        public static T? GetDerived<T, TSuper> (TSuper[]? vals, bool includeInharited = false)
        where TSuper: class where T : class, TSuper {
            int resIdx = FindObjectOfTypeIdx (vals, typeof (T), includeInharited);
            if (resIdx >= 0) {
                return (T)vals[resIdx];
            }
            return null;
        }

        // gets the index of an entity
        private static int FindObjectOfTypeIdx<TSuper> (TSuper[]? vals, Type type, bool includeInharited) {
            if (vals == null)
                return -1;
            for (int i = 0; i < vals.Length; i++) {
                // if objects type matches T return object casted to T
                if (vals[i].GetType () == type || (vals[i].GetType().IsSubclassOf(type) && includeInharited)) {
                    return i;
                }
            }
            return -1;
        }

        public static void Sort<T> (T[] values, Func<T, double> getPriority) {
            for (int i = 1; i < values.Length; i++) {
                T temp = values[i];
                double priority = getPriority (temp);
                int j;
                for (j = i-1; j >= 0; j--) {
                    if (getPriority (values[j]) < priority) {
                        break;
                    }
                    values[j+1] = values[j];
                }
                values[j+1] = temp;
            }
        }

        

        public static int? BinSearch <T> (T[] values, int key, Func<T, int> GetKey) {
            int start = 0, end = values.Length-1;
            int i = values.Length * 2 + 2;
            while (start <= end) {
                i--;
                if (i <= 0) {
                    Console.WriteLine ("Error, something went wrong with bin search");
                    break;
                }
                int middle = (start + end) / 2;
                int middleKey = GetKey (values[middle]);
                if (middleKey > key) {
                    end = middle - 1;
                }
                else if (middleKey < key) {
                    start = middle + 1;
                }
                else
                    return middle;
            }

            return null;
        }

         public static float FitInside (Vector2 inner, Vector2 outer) {
            if (inner.X / outer.X < inner.Y / outer.Y) {
                return outer.Y/inner.Y;
            }
            else {
                return outer.X / inner.X;
            }
            //return 0;
        }
    }

    // represents a 1D group of regions alternating between active and inactive
    //
    public class Span {
        public bool defaultActive = false;
        //private int defaultActiveAdder => defaultActive? 
        List<float> divisions = new List<float> ();
        public int Count => divisions.Count + 1;

        // gets the starting index for the current region
        public int GetRegionIdx (float pos) {
            int i = 0;
            for ( ; i < divisions.Count; i++) {
                if (pos < divisions[i]) break;
            }
            return i-1;
        }

        public bool IsActive (float pos) {
            return IsRegionActive(GetRegionIdx (pos), defaultActive);
        }

        public void SetRegionActivity (float start, float end, bool active) {
            int startIdx = GetRegionIdx (start)+1;
            int endIdx = GetRegionIdx (end);

            bool shouldClose = IsRegionActive (endIdx, defaultActive) != active;
            for (int i = endIdx; i >= startIdx; i--) {
                divisions.RemoveAt (i);
            }

            int insertIdx = startIdx;
            if (IsActive (start) != active) {
                
                divisions.Insert (insertIdx, start);
                insertIdx++;
            }
            if (shouldClose) {
                divisions.Insert (insertIdx, end);
            }
        }
        public static bool IsRegionActive (int idx, bool flipRegions) {
            // |-1 + 0| % 2 == 1
            return Math.Abs (idx + (flipRegions? 0 : 1)) % 2 == 1;
        }

        public override string ToString () {
            string res = "Span:";
            bool active = defaultActive;
            for (int i = 0; i < divisions.Count; i++) {
                res += $"{(active ? '#' : ' ')}{divisions[i]}";
                active = !active;
            }
            return res + (active ? '#' : ' ');
            
        }
    }
}
