

using Raylib_cs;

namespace Engine {
    public class GameStateManager {
        public readonly GameStateSharedInfo info;

        public Dictionary<char, GameState> gameStates = new Dictionary<char, GameState>();

        public string updateOrder = "";
        public string drawOrder = "";

        private bool loadNext = false;
        public void QueueLevelChange () {
            loadNext = true;
        }

        public GameStateManager (GameStateSharedInfo info, Dictionary<char, GameState> gameStates) {
            foreach (KeyValuePair<char, GameState> state in gameStates) {
                state.Value.info = info;
            }
            this.gameStates = gameStates;
            this.info = info;
        }

        public void Update () {
            float dt = Raylib.GetFrameTime ();
            for (int i = 0; i < updateOrder.Length; i++) {
                gameStates[updateOrder[i]].Update (this, dt);
            }

            if (loadNext) {
                foreach (KeyValuePair<char, GameState> state in gameStates) {
                    state.Value.LoadLevel (info.bp);
                }
                loadNext = false;
            }
        }

        public void Draw () {
            for (int i = 0; i < drawOrder.Length; i++) {
                gameStates[drawOrder[i]].Draw (this);
            }
        }
    }

    public abstract class GameState {
        public GameStateSharedInfo info;
        public virtual void LoadLevel (LevelBP bp) {}
        public abstract void Update (GameStateManager manager, float dt);
        public abstract void Draw (GameStateManager manager);
    }

    public abstract class GameStateModule {
        public virtual void OnStateChange (char from, char to) { }
        public abstract void Update (float dt);
        public abstract void Draw ();
    }

    public class GameStateSharedInfo {
        public WindowManager display;
        public LevelBP bp;
    }
}