using System;
using System.Collections.Generic;
using System.Linq;
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
        
    }
}
