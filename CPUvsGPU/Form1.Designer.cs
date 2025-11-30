namespace CPUvsGPU
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBoxVideoOutput = new PictureBox();
            label0 = new Label();
            labelFPS = new Label();
            comboBoxAccelerator = new ComboBox();
            comboBoxCamera = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            checkBoxFilter = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxVideoOutput).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxVideoOutput
            // 
            pictureBoxVideoOutput.Location = new Point(12, 12);
            pictureBoxVideoOutput.Name = "pictureBoxVideoOutput";
            pictureBoxVideoOutput.Size = new Size(760, 448);
            pictureBoxVideoOutput.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxVideoOutput.TabIndex = 0;
            pictureBoxVideoOutput.TabStop = false;
            // 
            // label0
            // 
            label0.AutoSize = true;
            label0.Location = new Point(12, 469);
            label0.Name = "label0";
            label0.Size = new Size(29, 15);
            label0.TabIndex = 1;
            label0.Text = "FPS:";
            // 
            // labelFPS
            // 
            labelFPS.AutoSize = true;
            labelFPS.Location = new Point(47, 469);
            labelFPS.Name = "labelFPS";
            labelFPS.Size = new Size(25, 15);
            labelFPS.TabIndex = 2;
            labelFPS.Text = "000";
            // 
            // comboBoxAccelerator
            // 
            comboBoxAccelerator.FormattingEnabled = true;
            comboBoxAccelerator.Location = new Point(591, 466);
            comboBoxAccelerator.Name = "comboBoxAccelerator";
            comboBoxAccelerator.Size = new Size(121, 23);
            comboBoxAccelerator.TabIndex = 3;
            comboBoxAccelerator.SelectedIndexChanged += comboBoxAccelerator_SelectedIndexChanged;
            // 
            // comboBoxCamera
            // 
            comboBoxCamera.FormattingEnabled = true;
            comboBoxCamera.Location = new Point(391, 466);
            comboBoxCamera.Name = "comboBoxCamera";
            comboBoxCamera.Size = new Size(121, 23);
            comboBoxCamera.TabIndex = 4;
            comboBoxCamera.SelectedIndexChanged += comboBoxCamera_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(334, 470);
            label1.Name = "label1";
            label1.Size = new Size(51, 15);
            label1.TabIndex = 5;
            label1.Text = "Camera:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(518, 470);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 6;
            label2.Text = "Accelerator";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(718, 469);
            label3.Name = "label3";
            label3.Size = new Size(33, 15);
            label3.TabIndex = 7;
            label3.Text = "Filter";
            // 
            // checkBoxFilter
            // 
            checkBoxFilter.AutoSize = true;
            checkBoxFilter.Location = new Point(757, 470);
            checkBoxFilter.Name = "checkBoxFilter";
            checkBoxFilter.Size = new Size(15, 14);
            checkBoxFilter.TabIndex = 8;
            checkBoxFilter.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 495);
            Controls.Add(checkBoxFilter);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(comboBoxCamera);
            Controls.Add(comboBoxAccelerator);
            Controls.Add(labelFPS);
            Controls.Add(label0);
            Controls.Add(pictureBoxVideoOutput);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            Text = "CPU vs GPU";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxVideoOutput).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxVideoOutput;
        private Label label0;
        private Label labelFPS;
        private ComboBox comboBoxAccelerator;
        private ComboBox comboBoxCamera;
        private Label label1;
        private Label label2;
        private Label label3;
        private CheckBox checkBoxFilter;
    }
}
