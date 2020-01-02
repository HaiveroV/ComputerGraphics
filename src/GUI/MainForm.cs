using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Draw
{
	/// <summary>
	/// Върху главната форма е поставен потребителски контрол,
	/// в който се осъществява визуализацията
	/// </summary>
	public partial class MainForm : Form
	{
		
		private DialogProcessor dialogProcessor = new DialogProcessor();
		private const string form = "Shape";

		public MainForm()
		{
			InitializeComponent();
		}

		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void ViewPortPaint(object sender, PaintEventArgs e)
		{
			dialogProcessor.ReDraw(sender, e);
		}
		
		void DrawRectangleSpeedButtonClick(object sender, EventArgs e)
		{
			dialogProcessor.AddRandomRectangle();
			
			statusBar.Items[0].Text = "Последно действие: Рисуване на правоъгълник";
			
			viewPort.Invalidate();
		}

		void ViewPortMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (pickUpSpeedButton.Checked) 
			{
				pickColorButton.Enabled = false;
				zoomInButton.Enabled = false;
				zoomOutButton.Enabled = false;

				var selection = dialogProcessor.ContainsPoint(e.Location);
				if (selection != null)
				{
					if (dialogProcessor.Selection.Contains(selection))
					{
						dialogProcessor.Selection.Remove(selection);
					}
					else
					{
						dialogProcessor.Selection.Add((Shape)selection);
					}
				}
				statusBar.Items[0].Text = "Последно действие: Селекция на примитив";
				pickColorButton.Enabled = true;
				zoomInButton.Enabled = true;
				zoomOutButton.Enabled = true;
				dialogProcessor.IsDragging = true;
				dialogProcessor.LastLocation = e.Location;
				viewPort.Invalidate();				
			}
		}

		void ViewPortKeyPress(object sender, KeyEventArgs e)
		{

			if (e.Control && e.KeyValue == (int)'C' )
			{
				Clipboard.SetData(form, dialogProcessor.Selection);
				statusBar.Items[0].Text = "Последно действие: Копиране на фигура";
			}
			if (e.Control && e.KeyValue == (int)'V' )
			{
				if (Clipboard.ContainsData(form))
				{
					List<Shape> clipShapes = (List<Shape>)Clipboard.GetData(form);
					foreach (var clipShape in clipShapes)
					{
						dialogProcessor.Paste(clipShape);
						viewPort.Invalidate();
					}
					viewPort.Invalidate();
					statusBar.Items[0].Text = "Последно действие: Поставяне на фигура";
				}
			}
			if (e.KeyValue == 46)
			{
				dialogProcessor.Delete(dialogProcessor.Selection);
				viewPort.Invalidate();
				statusBar.Items[0].Text = "Последно действие: Изтриване на фигура";
			}
		}

		void ViewPortMouseMove(object sender, MouseEventArgs e)
		{
			if (dialogProcessor.IsDragging)
			{
				if (dialogProcessor.Selection != null) statusBar.Items[0].Text = "Последно действие: Влачене";
				dialogProcessor.TranslateTo(e.Location);
				viewPort.Invalidate();
			}
		}

		void ViewPortMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			dialogProcessor.IsDragging = false;
		}

		private void squareButton_Click(object sender, EventArgs e)
		{
			dialogProcessor.AddRandomSquare();

			statusBar.Items[0].Text = "Последно действие: Рисуване на квадрат";

			viewPort.Invalidate();
		}

		private void ellipseButton_Click(object sender, EventArgs e)
		{
			dialogProcessor.AddRandomEllipse();

			statusBar.Items[0].Text = "Последно действие: Рисуване на елипса";

			viewPort.Invalidate();
			
		}

		private void pickColorButton_Click(object sender, EventArgs e)
		{
			if (dialogProcessor.Selection !=null)
			{
				ColorDialog colorDialog = new ColorDialog();
				colorDialog.AllowFullOpen = false;
				colorDialog.ShowHelp = true;

				if (colorDialog.ShowDialog() == DialogResult.OK)
				{
					foreach (var item in dialogProcessor.Selection)
					{
						item.FillColor = colorDialog.Color;
					}

					statusBar.Items[0].Text = "Последно действие: Смяна цвета на фигурата";
					viewPort.Invalidate();
				}
			}
		}

		private void zoomInButton_Click(object sender, EventArgs e)
		{
			dialogProcessor.ZoomIn();
			viewPort.Invalidate();
			statusBar.Items[0].Text = "Последно действие: Увеличаване размера на фигурата";
		}

		private void zoomOutButton_Click(object sender, EventArgs e)
		{
			dialogProcessor.ZoomOut();
			viewPort.Invalidate();
			statusBar.Items[0].Text = "Последно действие: Намаляване размера на фигурата";
		}

		private void exportAsPictureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Images|*.png;*.bmp;*.jpg";

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = dialog.FileName;
				string format = filePath.Substring(filePath.Length - 3);
				ImageFormat imageFormat = ImageFormat.Bmp;

				if (format == "jpg")
				{
					imageFormat = ImageFormat.Jpeg;
				}
				else if (format == "png")
				{
					imageFormat = ImageFormat.Png;
				}
				Bitmap bmp = new Bitmap((int)viewPort.Width, (int)viewPort.Height);
				viewPort.DrawToBitmap(bmp, new Rectangle(0, 0, (int)viewPort.Width, (int)viewPort.Height));
				bmp.Save(filePath, imageFormat);
				MessageBox.Show("Снимката е запазена успешно!");
			}
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult message = MessageBox.Show("Искате ли да запазите прогреса в сегашният проект?");
			if (message == DialogResult.Yes)
			{
				this.saveProjectToolStripMenuItem_Click(sender, e);
				dialogProcessor.ShapeList = new List<Shape>();
				viewPort.Invalidate();
				MessageBox.Show("Успешно запазихте вашият проект!");
			}
			else
			{
				dialogProcessor.ShapeList = new List<Shape>();
				viewPort.Invalidate();
				MessageBox.Show("Успешно започнахте нов проект");
			}
		}

		private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "All files|*.dat";

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = openFileDialog.FileName;

				BinaryFormatter binaryFormatter = new BinaryFormatter();
				using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
				{
					dialogProcessor.ShapeList = (List<Shape>)binaryFormatter.Deserialize(fileStream);
					viewPort.Invalidate();
					MessageBox.Show("Проектът се отвори успешно");
				}
			}
		}

		private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "All files|*.dat";

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;

				BinaryFormatter formatter = new BinaryFormatter();
				using (FileStream fileStream = new FileStream(@fileName, FileMode.OpenOrCreate))
				{
					formatter.Serialize(fileStream, dialogProcessor.ShapeList);
				}
				MessageBox.Show("Проектът е запазен успешно!");
			}
		}
		private void MainForm_Load(object sender, EventArgs e)
		{

		}
	}
}
