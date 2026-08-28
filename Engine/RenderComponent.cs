
using SubsetSharpEngine;
using Raylib_cs;
using System.Numerics;
using System.Linq.Expressions;

namespace Engine {
    // this exists in case I want this engine to support 3D rendering. Right now I'm mostly focused on 2D
    public abstract class ObjectRenderer {

        public abstract void Draw (Vector2 position);
    }

    // base class for any 2D renderer. You can use this to define custom 2D renderers for more complex objects
    // made up of multiple sprites
    public abstract class Renderer2D : ObjectRenderer {
        public readonly int layer;

        public Renderer2D (int layer) {
            this.layer = layer;
        }
    }

    // a collection consisting of an entity, its renderer, and its collider
    public struct EntityRenderInfo : ISingleEntityInfo {
        public Renderer2D renderer;
        public BoxCollider collider;
        public Entity entity;
        public Entity GetEntity () {
            return entity;
        }

        
        public bool PopulateFrom (Entity entity) {
            this.entity = entity;
            collider = entity.GetComponent<BoxCollider> (false)!;
            renderer = entity.GetComponent<Renderer2D> (true)!;
            return collider != null && renderer != null;
        }
    }

    // A collection of drawable entities sorted into layers
    public class Renderer2DCollection: EntityCollection  {
        private readonly List<EntityRenderInfo>[] layers;

        // getters
        public int LayerCount => layers.Length;
        public List<EntityRenderInfo> GetLayer (int i) {
            return layers[i];
        }
        
        public Renderer2DCollection (int numLayers = 1) {
            layers = new List<EntityRenderInfo>[numLayers];
            for (int i = 0; i < layers.Length; i++) {
                layers[i] = new List<EntityRenderInfo> ();
            }
        }

        // Methods
        public void DrawAll () {
            for (int i = layers.Length - 1; i >= 0; i--) {
                var currentLayer = layers[i];
                for (int j = currentLayer.Count - 1; j >= 0; j--) {
                    currentLayer[j].renderer.Draw (currentLayer[j].collider.position);
                }
            }
        }

        // callbacks
        public override bool TryAddEntity (Entity entity) {
            var info = new EntityRenderInfo ();
            bool res = info.PopulateFrom (entity);
            if (res) {
                layers[info.renderer!.layer].Add(info);
            }
            return res;
        }

        public override void Clear () {
            for (int i = 0; i < layers.Length; i++) {
                layers[i].Clear ();
            }
        }

        public override void Clean () {
            for (int i = 0; i < layers.Length; i++) {
                Entity.CleanList (layers[i]);
            }
        }
    }

    // renderer implementations
    public class RectangleRenderer: Renderer2D {
        private BoxCollider box;
        public Color color = Color.White;
        public RectangleRenderer (BoxCollider box, int layer) : base(layer) {
            this.box = box;
        }

        public override void Draw (Vector2 position) {
            Raylib.DrawRectangleV (position - box.size / 2, box.size, color);
        }
    }

    public class SpriteRenderer: Renderer2D{
        public Texture2D texture;
        public (Vector2 position, Vector2 dimensions)? src = null;
        public Vector2 drawDimensions = Vector2.One;
        public SpriteRenderer (int layer) : base (layer) {
            
        }

        

        public override void Draw (Vector2 position) {
            var srcRect = new Rectangle (Vector2.Zero, new Vector2 (texture.Width, texture.Height));
            if (src.HasValue) {
                (var srcPos, var srcDimensions) = src.Value;
                srcRect = new Rectangle (srcPos, srcDimensions);
            }
            Raylib.DrawTexturePro (texture, srcRect, new Rectangle (position, drawDimensions), drawDimensions * 0.5f, 0, Color.White);
        }
    }
}