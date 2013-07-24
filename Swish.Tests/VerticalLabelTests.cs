using System;
using Swish.Controls;

namespace Swish.Tests
{
	class VerticalLabelTests
	{
		internal void ManualView()
		{
			using (VerticalLabel text = new VerticalLabel())
			using (System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel())
			{
				panel.Size = new System.Drawing.Size(300, 300);
				panel.Controls.Add(text);

				text.Location = new System.Drawing.Point(150, 150);
				text.Text = "this is some text";
				text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

				text.Alignment = VerticalLabel.AlignmentCode.Top;
				text.Orientation = VerticalLabel.OrientationCode.Right;
				DisplayForm.Display(panel, "test", true, false);
			}

			using (VerticalLabel text = new VerticalLabel())
			using (System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel())
			{
				panel.Size = new System.Drawing.Size(300, 300);
				panel.Controls.Add(text);

				text.Location = new System.Drawing.Point(150, 150);
				text.Text = "this is some text";
				text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

				text.Alignment = VerticalLabel.AlignmentCode.Bottom;
				text.Orientation = VerticalLabel.OrientationCode.Right;
				DisplayForm.Display(panel, "test", true, false);
			}

			using (VerticalLabel text = new VerticalLabel())
			using (System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel())
			{
				panel.Size = new System.Drawing.Size(300, 300);
				panel.Controls.Add(text);

				text.Location = new System.Drawing.Point(150, 150);
				text.Text = "this is some text";
				text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

				text.Alignment = VerticalLabel.AlignmentCode.Top;
				text.Orientation = VerticalLabel.OrientationCode.Left;
				DisplayForm.Display(panel, "test", true, false);
			}

			using (VerticalLabel text = new VerticalLabel())
			using (System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel())
			{
				panel.Size = new System.Drawing.Size(300, 300);
				panel.Controls.Add(text);
				text.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

				text.Location = new System.Drawing.Point(150, 150);
				text.Text = "this is some text";
				text.Alignment = VerticalLabel.AlignmentCode.Bottom;
				text.Orientation = VerticalLabel.OrientationCode.Left;
				DisplayForm.Display(panel, "test", true, false);
			}

		}
	}
}
