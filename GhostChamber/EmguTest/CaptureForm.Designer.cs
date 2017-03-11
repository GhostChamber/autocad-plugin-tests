namespace EmguTest
{
    partial class CaptureForm
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
            this.components = new System.ComponentModel.Container();
            this.FrameImageBox = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.FrameImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // FrameImageBox
            // 
            this.FrameImageBox.Location = new System.Drawing.Point(12, 12);
            this.FrameImageBox.Name = "FrameImageBox";
            this.FrameImageBox.Size = new System.Drawing.Size(856, 567);
            this.FrameImageBox.TabIndex = 2;
            this.FrameImageBox.TabStop = false;
            // 
            // CaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 591);
            this.Controls.Add(this.FrameImageBox);
            this.Name = "CaptureForm";
            this.Text = "CaptureForm";
            ((System.ComponentModel.ISupportInitialize)(this.FrameImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox FrameImageBox;
    }
}