using Raylib_cs;
using System.Numerics;
namespace Engine {
    public class WindowManager {
        public RenderTexture2D renderTexture;
        Vector2 RenderTextureSize => renderTexture.Texture.Dimensions;
        public Vector2 WindowSize => WindowSize;
        private Vector2 windowSize = Vector2.One;
        public Vector2 RenderTextureDrawSize => renderTextureDrawSize;
        private Vector2 renderTextureDrawSize;
        
        public Vector2 InnerMousePos => innerMousePos;
        private Vector2 innerMousePos = Vector2.Zero;


        public WindowManager (int width, int height) {
            renderTexture = Raylib.LoadRenderTexture (width, height);
            //RenderTextureSize = new Vector2 (width, height);
        }

        public void Update () {
            windowSize.X = Raylib.GetRenderWidth ();
            windowSize.Y = Raylib.GetRenderHeight ();

            float renderTextureScale = Utils.FitInside (RenderTextureSize, windowSize);
            renderTextureDrawSize = RenderTextureSize * renderTextureScale;
            innerMousePos = (Raylib.GetMousePosition() + (renderTextureDrawSize - windowSize) / 2) / renderTextureScale;
        }

        public void Draw () {
            Raylib.DrawTexturePro (renderTexture.Texture,
                new Rectangle (0, 0, RenderTextureSize.X, -RenderTextureSize.Y),
                new Rectangle (windowSize/2, renderTextureDrawSize),
                renderTextureDrawSize / 2, 0, Color.White
            );
        }

        ~WindowManager () {
            Raylib.UnloadRenderTexture (renderTexture);
        }

        public override string ToString () {
            return $"windowSize {windowSize}, renderTextureSize {RenderTextureSize} -> {RenderTextureDrawSize}";
        }
    }
}