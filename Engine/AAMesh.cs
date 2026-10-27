using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine {
    public class AAMesh : Entity {
        // region start division idx = region idx - 1
        // region end division idx = region idx
        List<float> divisions = new List<float>();
        List<RegionSet1D> regions = new List<RegionSet1D> () {new RegionSet1D()};

        public static Color[] debugColors = {Color.Red, Color.Magenta, Color.Blue, Color.Yellow, Color.Green, new Color(255, 255, 0), Color.White};

        public int RegionCount => regions.Count;

        public float minimumRegionSize = 0.01f;

        public int SubRegionCount (int regionIdx) {
            return regions[regionIdx].Count;
        }

        public int GetRegionIdx (float pos) {
            if (divisions.Count <= 0) return 0;
            if (pos < divisions[0]) return 0;
            if (pos >= divisions[divisions.Count - 1]) {
                return regions.Count - 1;
            }

            int startIdx = 1, endIdx = divisions.Count - 1;
            int loopBudget = (regions.Count + 1) * 2;
            while (startIdx <= endIdx && loopBudget > 0) {
                loopBudget++;
                int middleIdx = (startIdx + endIdx) / 2;
                float startPos = divisions[middleIdx-1], endPos = divisions[middleIdx];
                //Console.WriteLine ($"{startIdx}\t{middleIdx}\t{endIdx}\t{startPos}\t{pos}\t{endPos}");
                if (pos < startPos) {
                    endIdx = middleIdx - 1;
                    continue;
                }
                if (endPos <= pos) {
                    //Console.WriteLine ("a");
                    startIdx = middleIdx + 1;
                    continue;
                }
                return middleIdx;
            }

            Console.WriteLine ($"Could not find region for {pos}");
            return -1;
        }
        
        public int GetOrMakeDivisionAt (float pos) {
            int i = GetRegionIdx(pos);
            /*for (i = 0; i < divisions.Count; i++) {
                // if a division is at roughly the same position as the one were trying to make return its index,
                // and don't make a new one
                if (divisions[i]-minimumRegionSize < pos && divisions[i]+minimumRegionSize > pos)
                    return i;

                // the divisions position is greater than the position were looking for then
                // we've looped passed it so break.
                if (divisions[i] > pos) break;
            }*/

            if (i != 0)
                if (divisions[i-1] - minimumRegionSize < pos && divisions[i-1] + minimumRegionSize > pos)
                    return i-1;
            

            // creates a duplicate of the region. That way the regions on each side of the division should match
            regions.Insert (i, (RegionSet1D)regions[i].Clone());
            
            // inserts the new division
            divisions.Insert (i, pos);
            return i;
        }

        public void AppendRectangle (Vector2 upperLeft, Vector2 lowerRight, bool active) {
            int start = GetOrMakeDivisionAt (upperLeft.X); // makes the left divisions
            int end = GetOrMakeDivisionAt (lowerRight.X); // makes the right division

            for (int i = start+1; i <= end; i++) {
                regions[i].SetRegionActivity (upperLeft.Y, lowerRight.Y, active);
            }
            Console.WriteLine ($"{start} {end}, region Count {regions.Count}, divisions {divisions.Count}");
            for (int j = 0; j < divisions.Count; j++) {
                Console.WriteLine (divisions[j]);
            }
        }

        public void CullRedundentRegions () {
            for (int i = divisions.Count - 2; i >= 0; i--) {
                // if the regions have different lengths than they are different, so move on to the next loop
                if (regions[i].divisions.Count != regions[i+1].divisions.Count) continue;

                bool regionsMatch = true;
                for (int j = 0; j < regions[i].divisions.Count; j++) {
                    //Console.WriteLine ($"inner Idx {j}, {regions[i].divisions.Count} {regions[i+1].divisions.Count}");
                    float d1 = regions[i].divisions[j], d2 = regions[i+1].divisions[j];
                    // compares subdivisions in each region to see if the two regions are the same
                    if (d1 + minimumRegionSize < d2 || d1 - minimumRegionSize > d2) {
                        regionsMatch = false;
                        break;
                    }
                }

                // if they are the same than cull one and delete the division to merge them
                if (regionsMatch) {
                    divisions.RemoveAt (i);
                    regions.RemoveAt (i+1);
                }
            }
        }

        public (float start, float end, RegionSet1D region) GetRegionByIdx (int i) {
            if (i <= 0) {
                return (float.NegativeInfinity, divisions[0], regions[0]);
            }

            if (i >= regions.Count - 1) {
                return (divisions[divisions.Count - 1], float.PositiveInfinity, regions[regions.Count - 1]);
            }

            return (divisions[i-1], divisions[i], regions[i]);
        }

        public bool SubRegionBox (BoxCollider outputBox, int regionIdx, int subregionIdx) {
            (float leftBound, float rightBound, RegionSet1D region) = GetRegionByIdx (regionIdx);
            (float upper, float lower) = region.GetRegionBounds (subregionIdx);
            outputBox.position = new Vector2 (leftBound + rightBound, upper + lower) / 2;
            outputBox.size = new Vector2 (rightBound - leftBound, lower - upper);
            return RegionSet1D.IsRegionActive (subregionIdx, region.defaultActive);
        }

        public void DrawTextured (Texture2D texture, float v, Color color) {
            for (int i = 1; i < regions.Count-1; i++) {
                float left = divisions[i-1];
                float right = divisions[i];
                for (int j = 0; j < regions[i].Count - 2; j+=2) {
                    float upper = regions[i].divisions[j];
                    float lower = regions[i].divisions[j+1];
                    Vector2 position = new Vector2 (left, upper);
                    Vector2 size = new Vector2 (right - left, lower - upper);
                    //Raylib.DrawRectangleV (position, size, color);
                    Raylib.DrawTexturePro (texture, new Rectangle (position * v, size * v), new Rectangle (position, size), Vector2.Zero, 0, color);
                }
            }
        }

        public void DrawColoredDebug () {
            for (int i = 1; i < regions.Count-1; i++) {
                float left = divisions[i-1];
                float right = divisions[i];
                for (int j = 0; j < regions[i].Count - 2; j+=2) {
                    float upper = regions[i].divisions[j];
                    float lower = regions[i].divisions[j+1];
                    Vector2 position = new Vector2 (left, upper);
                    Vector2 size = new Vector2 (right - left, lower - upper);
                    Color color = debugColors[(j + i - 1) % debugColors.Length];
                    
                    Raylib.DrawRectangleV (position, size, color);

                }
            }
        }

        public void DrawColored (Color color) {
            for (int i = 1; i < regions.Count-1; i++) {
                float left = divisions[i-1];
                float right = divisions[i];
                for (int j = 0; j < regions[i].Count - 2; j+=2) {
                    float upper = regions[i].divisions[j];
                    float lower = regions[i].divisions[j+1];
                    Vector2 position = new Vector2 (left, upper);
                    Vector2 size = new Vector2 (right - left, lower - upper);
                    Raylib.DrawRectangleV (position, size, color);
                    
                }
            }
        }
    }


    // represents a 1D group of regions alternating between active and inactive
    //
    public class RegionSet1D {
        public bool defaultActive = false;
        //private int defaultActiveAdder => defaultActive? 
        public List<float> divisions = new List<float> ();
        public int Count => divisions.Count + 1;

        // gets the starting index for the current region
        public int GetRegionIdx (float pos) {
            
            if (divisions.Count == 0) {
                return 0;
            }
            if (pos < divisions[0]) {
                return 0;
            }

            if (pos >= divisions[divisions.Count - 1]) {
                return divisions.Count;
            }

            int start = 1, end = divisions.Count - 1;
            int loopBudget = Count * 2;
            while (start <= end) {
                if (loopBudget <= 0) break;
                loopBudget -= 1;

                int middle = (start + end) / 2;
                //(float startPos, float endPos) = GetRegionBounds (middle);
                float startPos = divisions[middle - 1];
                float endPos = divisions[middle];
                //Console.WriteLine ($"Key {pos}, {start} {middle} {end}, {startPos} {endPos}");
                if (startPos <= pos && pos < endPos) {
                    return middle;
                }
                if (pos < startPos) end = middle - 1;
                else if (end < pos) start = middle + 1;
            }
            Console.WriteLine ("Error Could not find region at " + pos);
            return -1;
        }

        float GetDivisionPosition (int idx) {
            if (idx < 0) {
                return float.NegativeInfinity;
            }
            if (idx >= divisions.Count) {
                return float.PositiveInfinity;
            }

            return divisions[idx];
        }

        public (float, float) GetRegionBounds (int idx) {
            return (GetDivisionPosition (idx - 1), GetDivisionPosition (idx));
        }

        public bool IsActive (float pos) {
            return IsRegionActive (GetRegionIdx (pos), defaultActive);
        }

        public void SetRegionActivity (float start, float end, bool active) {
            int startIdx = GetRegionIdx (start);
            int endIdx = GetRegionIdx (end);
            //Console.WriteLine ($"{startIdx} {endIdx}");
            bool shouldClose = IsRegionActive (endIdx, defaultActive) != active;
            for (int i = endIdx-1; i >= startIdx; i--) {
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

        public bool IsRegionActive (int idx) {
            return IsRegionActive (idx, defaultActive);
        }

        public static bool IsRegionActive (int idx, bool flipRegions) {
            // |-1 + 0| % 2 == 1
            return Math.Abs (idx + (flipRegions ? 1 : 0)) % 2 == 1;
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

        public object Clone () {
            var clone = new RegionSet1D ();
            clone.divisions = new List<float> (divisions.Count);
            for (int i = 0; i < divisions.Count; i++) {
                clone.divisions.Add (divisions[i]);
            }
            return clone;
        }
    }

    public struct AAMeshIterator {
        public readonly float bottom, top, left, right;
        int regionIdx = 0;
        int subregionIdx;
        AAMesh aamesh;
        public AAMeshIterator (AAMesh aamesh, float left, float right, float bottom, float top) {
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;
            this.aamesh = aamesh;
            regionIdx = aamesh.GetRegionIdx (left);
            subregionIdx = aamesh.GetRegionByIdx (regionIdx).region.GetRegionIdx(top);
        }

        public AAMeshIterator (AAMesh aamesh, BoxCollider collider) {
            left = collider.position.X - collider.size.X / 2;
            bottom = collider.position.Y - collider.size.Y / 2;
            right = collider.position.X + collider.size.X / 2;
            top = collider.position.Y + collider.size.Y / 2;
            this.aamesh = aamesh;
            regionIdx = aamesh.GetRegionIdx (left);
            subregionIdx = aamesh.GetRegionByIdx (regionIdx).region.GetRegionIdx (top);
        }
    }

}
