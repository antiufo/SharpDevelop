namespace Shaman.AvalonEdit
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.codeEditorWrapper1 = new Shaman.AvalonEdit.CodeEditorWrapper();
            this.SuspendLayout();
            // 
            // codeEditorWrapper1
            // 
            this.codeEditorWrapper1.AutoSkipUsings = false;
            this.codeEditorWrapper1.Code = null;
            this.codeEditorWrapper1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditorWrapper1.Location = new System.Drawing.Point(0, 0);
            this.codeEditorWrapper1.Name = "codeEditorWrapper1";
            this.codeEditorWrapper1.OnDirtyChanged = null;
            this.codeEditorWrapper1.Size = new System.Drawing.Size(603, 301);
            this.codeEditorWrapper1.TabIndex = 0;
            this.codeEditorWrapper1.Text = "codeEditorWrapper1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 301);
            this.Controls.Add(this.codeEditorWrapper1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

		}

		#endregion

		private CodeEditorWrapper codeEditorWrapper1;
	}
}