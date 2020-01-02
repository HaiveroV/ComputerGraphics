using System;
using System.Drawing;
using Draw.src.Model;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Draw
{
	
	public class DialogProcessor : DisplayProcessor
	{
		private PointF lastLocation;
		private bool isDragging;
		private List <Shape> selection = new List<Shape>();

		public DialogProcessor()
		{
		}

		public List<Shape> Selection {
			get { return selection; }
			set { selection = value; }
		}

		public bool IsDragging {
			get { return isDragging; }
			set { isDragging = value; }
		}
		
		public PointF LastLocation {
			get { return lastLocation; }
			set { lastLocation = value; }
		}
		
		public void AddRandomRectangle()
		{
			Random rnd = new Random();
			int x = rnd.Next(100,1000);
			int y = rnd.Next(100,600);
			
			RectangleShape rect = new RectangleShape(new Rectangle(x,y,100,200));
			rect.FillColor = Color.White;

			ShapeList.Add(rect);
		}

		public void AddRandomSquare ()
		{
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			SquareShape square = new SquareShape(new Rectangle(x, y, 100, 100));
			square.FillColor = Color.White;

			ShapeList.Add(square);
		}

		public void AddRandomEllipse ()
		{
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			EllipseShape ellipse = new EllipseShape(new Rectangle(x, y, 100, 200));
			ellipse.FillColor = Color.White;

			ShapeList.Add(ellipse);
		}
		
		public Shape ContainsPoint(PointF point)
		{
			for(int i = ShapeList.Count - 1; i >= 0; i--)
			{
				if (ShapeList[i].Contains(point))
				{
					return ShapeList[i];
				}	
			}
			return null;
		}
		
		public void TranslateTo(PointF p)
		{
			foreach (var item in Selection)
			{
				item.Location = new PointF(item.Location.X + p.X - lastLocation.X, item.Location.Y + p.Y - lastLocation.Y);
			}

			lastLocation = p;
		}

		public override void Draw(Graphics grfx)
		{
			base.Draw(grfx);
			foreach (var item in Selection)
			{
				grfx.DrawRectangle(Pens.Blue, item.Location.X - 3, item.Location.Y - 3, item.Width + 6, item.Height + 6);
			}
		}

		public void Paste (object clipboardShape)
		{
			ShapeList.Add((Shape)clipboardShape);
		}
		
		public void Delete(List<Shape> shapes)
		{
			foreach (var shape in shapes)
			{
				ShapeList.Remove(shape);
			}
		}

		public void ZoomIn ()
		{
			if (Selection != null)
			{
				foreach (var item in Selection)
				{
					if (item.Height >= 600 && item.Width >=600)
					{
						MessageBox.Show("Достигнахте лимита за уголемяване на фигурата!");
					}
					else
					{
						ShapeList.Remove(item);

						item.Height += 50;
						item.Width += 50;

						ShapeList.Add(item);
					}
				}
			}
		}

		public void ZoomOut ()
		{
			foreach (var item in Selection)
			{
				if (Selection != null)
				{
					if (item.Height > 100 && item.Width > 100)
					{
						ShapeList.Remove(item);

						item.Height -= 50;
						item.Width -= 50;

						ShapeList.Add(item);
					}
					else
					{
						MessageBox.Show("Достигнахте минималният размер на фигурата!");
					}
				}
			}
		}
	}
}
