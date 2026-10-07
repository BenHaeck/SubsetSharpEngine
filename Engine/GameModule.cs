

namespace Engine {
    public class GameStateManager {
        LevelBP bp;

        
    }

    public class GameState {
        
    }

    public abstract class GameStateModule {
        public virtual void OnStateChange (char from, char to) { }
        public abstract void Update (float dt);
        public abstract void Draw ();
    }

    public class GameStateSharedInfo {
        
    }
}