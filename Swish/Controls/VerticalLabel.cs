using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Swish.Controls
{
	public class VerticalLabel: Label
	{
		public enum AlignmentCode
		{
			Top,
			Bottom,
		}

		public enum OrientationCode
		{
			Right,
			Left,
		}

		public AlignmentCode Alignment
		{
			get;
			set;
		}

		public OrientationCode Orientation
		{
			get;
			set;
		}

		private void Draw(PaintEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			ApplyTransform(e);

			Brush brush = new SolidBrush(ForeColor);
			e.Graphics.DrawString(Text, Font, brush, 0, 0);
		}

		private void ApplyTransform(PaintEventArgs e)
		{
			Matrix matrix = e.Graphics.Transform;
			SizeF textSize = e.Graphics.MeasureString(Text, Font);
			if (Orientation == OrientationCode.Left)
			{
				if (Alignment == AlignmentCode.Bottom)
				{
					matrix.Rotate(90);
					float x = this.Height - textSize.Width;
					matrix.Translate(x, -textSize.Height);
				} else
				{
					matrix.Rotate(90);
					matrix.Translate(0, -textSize.Height);
				}
			} else
			{
				if (Alignment == AlignmentCode.Bottom)
				{
					matrix.Rotate(270);
					matrix.Translate(-this.Height, 0);
				} else
				{
					matrix.Rotate(270);
					matrix.Translate(-textSize.Width, 0);
				}
			}
			e.Graphics.Transform = matrix;
		}

		//
		//
		// Parameters:
		//   e:
		//     A System.Windows.Forms.PaintEventArgs that contains the event data.
		protected override void OnPaint(PaintEventArgs e)
		{
			ApplyTransform(e);
			base.OnPaint(e);
		}

		//
		//
		// Returns:
		//     The default System.Drawing.Size of the control.
		protected override Size DefaultSize
		{
			get
			{
				Size size = base.DefaultSize;
				size = new Size(size.Width, size.Width);//size = new Size(size.Height, size.Width);
				return size;
			}
		}

		//
		// Summary:
		//     Gets the preferred height of the control.
		//
		// Returns:
		//     The height of the control (in pixels), assuming a single line of text is
		//     displayed.
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override int PreferredHeight { get { return base.PreferredWidth; } }
		//
		// Summary:
		//     Gets the preferred width of the control.
		//
		// Returns:
		//     The width of the control (in pixels), assuming a single line of text is displayed.
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override int PreferredWidth { get { return base.PreferredHeight; } }

		//
		// Summary:
		//     Retrieves the size of a rectangular area into which a control can be fitted.
		//
		// Parameters:
		//   proposedSize:
		//     The custom-sized area for a control.
		//
		// Returns:
		//     An ordered pair of type System.Drawing.Size representing the width and height
		//     of a rectangle.
		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = proposedSize;
			size = new Size(size.Width, size.Width);//size = new Size(size.Height, size.Width);
			size = base.GetPreferredSize(size);
			size = new Size(size.Width, size.Width);//size = new Size(size.Height, size.Width);
			return size;
		}

		//
		//
		// Returns:
		//     The text associated with this control.
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[SettingsBindable(true)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }



	}


}
