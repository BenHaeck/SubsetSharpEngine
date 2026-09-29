using System;
using System.Numerics;

namespace Engine {
    public class TileBP {
        
    }

    public class LevelBP {
        public Dictionary<char, List<Vector2>> layout = new Dictionary<char, List<Vector2>>();

        public List<Vector2>? GetTilePositionsByType (char tileType) {
            List<Vector2>? tilePositions = null;
            layout.TryGetValue (tileType, out tilePositions);
            return tilePositions;
        }

        /*public void CreateEmptyWithTiles (char[] tiles) {
            for (int i = 0; i < tiles.Length; i++) {
                if (!layout.ContainsKey(tiles[i]))
                    layout.Add (tiles[i], new List<Vector2> ());
            }
        }*/

        public List<Vector2> GetTilesByType (char t) {
            if (layout.ContainsKey (t)) {
                return layout[t];
            }
            else {
                var tiles = new List<Vector2> ();
                layout.Add (t, tiles);
                return tiles;
            }
        }

        public void CreateTile (char tileType, Vector2 position) {
            if (layout.ContainsKey (tileType)) {
                layout[tileType].Add (position);
            }
            else {
                layout.Add (tileType, new List<Vector2> () { position});
            }
        }

        public void Populate (string level) {
            layout = new Dictionary<char, List<Vector2>> ();
            level = level.Replace ("\r", "");
            string[] tileTypes = level.Split ('\n');
            for (int t = 0; t < tileTypes.Length; t++) {
                if (tileTypes[t].Length <= 2) continue;

                var tileList = new List<Vector2> ();
                layout.Add (tileTypes[t][0], tileList);

                var tiles = tileTypes[t].Split ('|');
                for (int i = 1; i < tiles.Length; i++) {
                    int s = tiles[i].IndexOf (',');
                    if (s == -1) continue;
                    string px = tiles[i].Substring (0, s);
                    string py;
                    int s2 = tiles[i].IndexOf (',', s+1);
                    if (s2 == -1) {
                        py = tiles[i].Substring (s+1);
                    }
                    else {
                        py = tiles[i].Substring (s+1, s2 - s);
                    }
                    tileList.Add (new Vector2(float.Parse (px), float.Parse (py))); 
                }
                
            }
            
        }

        public override string ToString () {
            return $"LevelBP{{\n{Serialize ()}}}";
        }

        public string Serialize () {
            string res = "";
            foreach (KeyValuePair<char, List<Vector2>> tileTypes in layout) {
                List<Vector2> tilePositions = tileTypes.Value;
                if (tilePositions.Count <= 0) continue;
                res += $"{tileTypes.Key}";
                for (int i = 0; i < tileTypes.Value.Count; i++) {
                    res += $"|{tilePositions[i].X},{tilePositions[i].Y}";
                }
                res += "\n";
            }

            return res;
        }

        public string TileCounts () {
            string res = "tile counts {\n";
            uint total = 0;
            foreach (KeyValuePair<char, List<Vector2>> tileDataByType in layout) {
                res += $"  {tileDataByType.Key}: {tileDataByType.Value.Count}\n";
                total += (uint)tileDataByType.Value.Count;
            }
            res += $"}}\ntotal: {total}";
            return res;
        }
    }
}

/*
type|x,y|x,y|x,y

*/