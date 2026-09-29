using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
namespace Engine {
    public abstract class LevelEditor {
        public LevelBP bp = new LevelBP();

        public Vector2 tileSize = new Vector2 (1);


        public Vector2 mousePos = Vector2.Zero;
        public Vector2 curserPos;
        public char[] validTiles = new char[0];
        public int currentTile;
        public List<Vector2> currentTileData = new List<Vector2>();

        // controls
        KeyboardKey selectButton = KeyboardKey.Space;

        public void SetSelectedTile (int t) {
            currentTile = t;
            currentTileData = bp.GetTilesByType (validTiles[t]);
        }

        public void AddTile (Vector2 position, bool replaceCurrent = true) {
            RemoveTileAt (position);

            currentTileData.Add (position);
        }

        public void RemoveTileAt (Vector2 position) {
            foreach (KeyValuePair<char, List<Vector2>> tileDataByType in bp.layout) {
                var tileData = tileDataByType.Value;
                for (int i = 0; i < tileDataByType.Value.Count; i++) {
                    if (Collision.CheckIntersectionBox (tileData[i], position, tileSize * 0.3f)) {
                        tileData.RemoveAt (i);
                    }
                }
            }
        }
        

        public void Update (float dt) {
            if (Raylib.IsKeyDown (selectButton)) {
                curserPos = mousePos;
                for (int i = 0; i < validTiles.Length; i++) {
                    if (Collision.CheckIntersectionBox (GetTileSelectPos (i), curserPos, new Vector2 (48) / 2)) {
                        SetSelectedTile (i);
                    }
                }
            }
            else {
                curserPos = mousePos / tileSize;
                curserPos = new Vector2 (MathF.Round (curserPos.X), MathF.Round (curserPos.Y));
                curserPos *= tileSize;

                if (Raylib.IsMouseButtonPressed (MouseButton.Left)) {
                    AddTile (curserPos);
                }
                if (Raylib.IsMouseButtonPressed (MouseButton.Right)) {
                    RemoveTileAt (curserPos);
                }
            }
        }

        public Vector2 GetTileSelectPos (int idx) {
            return new Vector2 (16) + idx * new Vector2 (48, 0);
            
        }

        public void DrawGameWorld () {
            if (Raylib.IsKeyDown (selectButton)) {
                for (int i = 0; i < validTiles.Length; i++) {
                    var pos = GetTileSelectPos (i);
                    DrawTile (validTiles[i], pos);
                }
            }
            else {
                for (int i = 0; i < validTiles.Length; i++) {
                    List<Vector2>? tilePositions = bp.GetTilePositionsByType (validTiles[i]);
                    if (tilePositions == null) continue;

                    for (int j = 0; j < tilePositions.Count; j++) {
                        DrawTile (validTiles[i], tilePositions[j]);
                    }

                }
            }
            Raylib.DrawRectangleV (curserPos - tileSize * 0.5f, tileSize, new Color(255, 255, 255, 128));
            
        }

        public virtual void DrawTile (char tileType, Vector2 pos) {}
    }
}
