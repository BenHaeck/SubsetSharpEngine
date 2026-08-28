using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine {
    public static class Utils {
        public static T? GetDerived<T, TSuper> (TSuper[]? vals, bool includeInharited = false)
        where TSuper: class where T : class, TSuper {
            int resIdx = FindObjectOfTypeIdx (vals, typeof (T), includeInharited);
            if (resIdx >= 0) {
                return (T)vals[resIdx];
            }
            return null;
        }

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

    }
}
