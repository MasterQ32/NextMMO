﻿namespace NextMMO
{
	partial class FormGame
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timerFramerate = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// timerFramerate
			// 
			this.timerFramerate.Enabled = true;
			this.timerFramerate.Interval = 120;
			this.timerFramerate.Tick += new System.EventHandler(this.timerFramerate_Tick);
			// 
			// FormGame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(659, 452);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "FormGame";
			this.Text = "NextMMO";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormGame_Paint);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timerFramerate;

	}
}

