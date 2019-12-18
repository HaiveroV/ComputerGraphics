using System;
using System.Drawing;

namespace Draw.src.Model
{
    [Serializable]
    public class EllipseShape : Shape
    {
        public EllipseShape(RectangleF rect) : base (rect)
        {

        }

        public override bool Contains(PointF point)
        {
            if (base.Contains(point))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public override void DrawSelf(Graphics grfx)
        {
            base.DrawSelf(grfx);

            grfx.FillEllipse(new SolidBrush(FillColor), Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
            grfx.DrawEllipse(Pens.Black, Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }
    }
}
