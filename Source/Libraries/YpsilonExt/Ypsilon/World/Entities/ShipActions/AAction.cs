﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ypsilon.World.Entities.ShipActions
{
    class AAction
    {
        public Ship Parent;

        public AAction(Ship parent)
        {
            Parent = parent;
        }

        public virtual void Update(float frameSeconds)
        {
            
        }
    }
}
