
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Engine {
    public abstract class LevelEditorTool {
        public readonly char[] validTiles;
        public int currentTileIdx = 0;
        public char CurrentTile => validTiles[currentTileIdx];

        public virtual void OnSelect () { }
        public virtual void OnDeselect () { }

        public abstract void Update (LevelEditor editor, Vector2 cursorPos, float dt);

        public virtual void Draw () { }
    }

    public class TilePlaceTool: LevelEditorTool {
        public override void Update (LevelEditor editor, Vector2 cursorPos, float dt) {
            if (Raylib.IsMouseButtonDown (MouseButton.Left)) {
                editor.RemoveTileAt (cursorPos);

                editor.info.bp.CreateTile (CurrentTile, cursorPos);
            }
            if (Raylib.IsMouseButtonDown (MouseButton.Right)) {
                editor.RemoveTileAt (cursorPos);
            }
        }
    }
}
