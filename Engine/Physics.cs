using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Engine {
    public class BoxCollider {
        public Vector2 position;
        public Vector2 size;

        public BoxCollider (Vector2 position, Vector2 size) {
            this.position = position;
            this.size = size;
        }
    }
}
