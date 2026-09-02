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
        TransparentWall = 2,
        Enemy = 1<<2,
        Item = 1<<3,

        all = -1,
    }
}
