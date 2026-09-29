using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsetSharpEngine {
    [Flags]
    public enum Tags {
        None = 0,
        Wall = 1,
        //TransparentWall = 2,
        //Enemy = 1<<2,
        Hitbox = 1<<3,
        Character = 1<<4,

        PlayerAligned = 1<<10,
        EnemyAligned = 1<<11,
        //Item = 1<<4,
        All = -1,
    }

    public enum Layer {
        Ground,
        Characters,
        Bullets,
    }
}
