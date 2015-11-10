﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ypsilon.Entities.Defines
{
    class ShipVertices
    {
        public static Vector3[] SimpleArrow = new Vector3[4] {
            new Vector3(0, 1, 0),
            new Vector3(1, -1, 0f),
            new Vector3(0, -0.7f, 0.5f),
            new Vector3(-1, -1, 0f) };
    }
}
