using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Engine {
    public static class Collision {
        public static Vector2 GetOverlapBox (Vector2 pos1, Vector2 pos2, Vector2 combSize) {
            Vector2 dist = pos1 - pos2;
            for (int i = 0; i < 2; i++) {
                dist[i] = MathF.Abs (dist[i]);
            }

            return combSize - dist;
        }

        public static bool CheckIntersectionBox (Vector2 pos1, Vector2 pos2, Vector2 combSize) {
            Vector2 overlap = GetOverlapBox (pos1, pos2, combSize);
            return overlap.X > 0 && overlap.Y > 0;
        }

        public static bool CheckIntersection (BoxCollider box1, BoxCollider box2) {
            return CheckIntersectionBox (box1.position, box2.position, (box1.size + box2.size)/2);
        }
    }

    public class PointCollider {
        public Vector2 position;
    }

    public class BoxCollider : PointCollider {
        public Vector2 size;

        public BoxCollider (Vector2 position, Vector2 size) {
            this.position = position;
            this.size = size;
        }

        public void AlignEdge (BoxCollider other, int axis) {
            float dir = MathF.Sign(position[axis] - other.position[axis]);
            position[axis] = other.position[axis] + (other.size[axis] + size[axis]) * 0.5f * dir;
        }
    }
}
