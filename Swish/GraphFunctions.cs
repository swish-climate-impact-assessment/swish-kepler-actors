using System;
using System.Collections.Generic;
using System.Drawing;

namespace Swish
{
	public class GraphFunctions
	{
		public static readonly Color Zone2Colour = Color.LightBlue;
		public static readonly Color Zone1Colour = Color.LightGray;
		public static readonly Color LineColour = Color.Black;

		public static void DrawZones(List<double> values, Bitmap image)
		{
			if (values == null || values.Count < 2)
			{
				throw new ArgumentException("values");
			}

			if (image == null)
			{
				throw new ArgumentException("image");
			}

			int width = image.Width;
			int height = image.Height;

			double minimum = values[0];
			double maximum = values[values.Count - 1];

			using (Graphics graphics = Graphics.FromImage(image))
			{
				if (minimum == maximum)
				{
					graphics.FillRectangle(new SolidBrush(Zone1Colour), 0, 0, width - 1, height - 1);
					return;
				}

				for (int index = 0; index + 1 < values.Count; index++)
				{
					double value0 = values[index];
					double value1 = values[index + 1];

					float x0 = (float)(width - (width - 1) * (value0 - minimum) / (maximum - minimum)) - 1;
					float x1 = (float)(width - (width - 1) * (value1 - minimum) / (maximum - minimum)) - 1;
					float y0 = 0;
					float y1 = height - 1;
					if ((index % 2) == 0)
					{
						graphics.FillRectangle(new SolidBrush(Zone1Colour), x0, y0, x1, y1);
					} else
					{
						graphics.FillRectangle(new SolidBrush(Zone2Colour), x0, y0, x1, y1);
					}
				}
			}
		}

		public static void DrawLines(List<double> values, int offset, int count, Color colour, Bitmap image, double minimum, double maximum)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}

			if (values == null)
			{
				throw new ArgumentNullException("values");
			}

			if (values.Count < 1)
			{
				return;
				throw new ArgumentException("values");
			}

			Pen pen;
			if (colour == null)
			{
				pen = new Pen(LineColour);
			} else
			{
				pen = new Pen(colour);
			}

			int width = image.Width;
			int height = image.Height;

			using (Graphics graphics = Graphics.FromImage(image))
			{
				if (minimum == maximum)
				{
					graphics.DrawLine(pen, 0, height / 2, width - 1, height / 2);
					return;
				}

				int originalCount = count;
				for (int index = offset; count > 1; index++, count--)
				{
					double value0 = values[index];
					if (value0 < minimum)
					{
						value0 = (float)minimum;
					} else if (value0 > maximum)
					{
						value0 = (float)maximum;
					}
					double value1 = values[index + 1];
					if (value1 < minimum)
					{
						value1 = (float)minimum;
					} else if (value1 > maximum)
					{
						value1 = (float)maximum;
					}

					if (value0 == 0 || value1 == 0 || double.IsNaN(value0) || double.IsNaN(value1))
					{
						continue;
					}
					float x0 = (width - 1) * (index - offset) / (originalCount - 1.0f);
					float x1 = (width - 1) * (index - offset + 1.0f) / (originalCount - 1.0f);
					float y0 = (float)(height - (height - 1) * (value0 - minimum) / (maximum - minimum)) - 1;
					float y1 = (float)(height - (height - 1) * (value1 - minimum) / (maximum - minimum)) - 1;
					graphics.DrawLine(pen, x0, y0, x1, y1);
				}

				if (minimum <= 0 && 0 <= maximum)
				{
					float y = (float)(height - (height - 1) * (0.0f - minimum) / (maximum - minimum)) - 1;
					float x0 = 0;
					float x1 = width - 1;
					graphics.DrawLine(pen, x0, y, x1, y);
				}
			}
		}

		public static void ValueRange(out double minimum, out double maximum, List<double> values, int offset, int count)
		{
			minimum = double.MaxValue;
			maximum = double.MinValue;

			for (int index = offset; count > 0; index++, count--)
			{
				double value = values[index];
				if (double.IsNaN(value))
				{
					continue;
				}
				if (value < minimum)
				{
					minimum = value;
				}
				if (value > maximum)
				{
					maximum = value;
				}
			}
		}

		public static void DrawLines(List<double> values, int offset, int count, Color colour, Bitmap image)
		{
			double yMinimum;
			double yMaximum;
			ValueRange(out yMinimum, out yMaximum, values, offset, count);
			DrawLines(values, offset, count, colour, image, yMinimum, yMaximum);
		}

	}
}