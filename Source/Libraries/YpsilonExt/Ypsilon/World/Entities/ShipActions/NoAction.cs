﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.World.Entities.ShipActions
{
    /// <summary>
    /// Just flying along.
    /// </summary>
    class NoAction : AAction
    {
        public NoAction(Ship parent)
            : base(parent)
        {

        }
    }
}
