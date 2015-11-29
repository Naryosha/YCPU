﻿using Microsoft.Xna.Framework;
using Ypsilon.Core.Graphics;
using Ypsilon.Entities;
using Ypsilon.Modes.Space.Input;
using Ypsilon.Modes.Space.Resources;

namespace Ypsilon.Modes.Space.Entities
{
    class ASpaceComponent : AComponent
    {
        public Position3D Position
        {
            get;
            set;
        }

        public bool IsVisible
        {
            get
            {
                return true;
            }
        }

        public float ViewSize = 1f;
        protected Vector3[] DrawVertices = null;
        protected Color DrawColor = Color.Magenta;
        protected Matrix DrawMatrix = Matrix.Identity;

        public ASpaceComponent()
        {
            Position = Position3D.Zero;
        }

        public virtual void Draw(AEntity entity, VectorRenderer renderer, Position3D worldSpaceCenter, MouseOverList mouseOverList)
        {
            renderer.DrawPolygon(DrawVertices, DrawMatrix, DrawColor, true);
            mouseOverList.AddEntityIfMouseIsOver(entity, DrawMatrix.Translation);
        }

        public void DrawSelection(VectorRenderer renderer)
        {
            Matrix drawMatrixNoRotation = Matrix.CreateScale(ViewSize) * Matrix.Identity;
            drawMatrixNoRotation.Translation = DrawMatrix.Translation;
            renderer.DrawPolygon(Vertices.SelectionLeft, drawMatrixNoRotation, Color.Yellow, false);
            renderer.DrawPolygon(Vertices.SelectionRight, drawMatrixNoRotation, Color.Yellow, false);
        }
    }
}