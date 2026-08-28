// See https://aka.ms/new-console-template for more information
using Raylib_cs;
using Engine;
using System.Numerics;

using SubsetSharpEngine;

public static class Program {
    public static void Main () {
        var entitySystem = new EntitySystem ();

        var physicsEntities = new EntitiesWithComponent<BoxCollider> ();
        var renderers = new Renderer2DCollection ();

        entitySystem.entityCollections = new EntityCollection[]{
            physicsEntities, renderers
        };
        
        var player = new Player ();
        player.collider.position = new Vector2 (64, 64);

        entitySystem.AddEntity (player);

        Raylib.InitWindow (600, 400, "Hello");
        Raylib.SetWindowState (ConfigFlags.VSyncHint);
        while (!Raylib.WindowShouldClose ()) {
            Raylib.BeginDrawing ();
            Raylib.ClearBackground (Color.DarkGray);
            entitySystem.Update (Raylib.GetFrameTime ());

            renderers.DrawAll ();

            Raylib.DrawFPS (6, 6);
            Raylib.EndDrawing ();
        }

        Raylib.CloseWindow ();
    }
}

