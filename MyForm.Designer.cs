namespace ChatGPTTrial2
{
    partial class MyForm
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
            this.outputTextBox = new System.Windows.Forms.RichTextBox();
            this.submitButton = new System.Windows.Forms.Button();
            this.startStopListeningButton = new System.Windows.Forms.Button();
            this.inputTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(408, 72);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(380, 366);
            this.outputTextBox.TabIndex = 1;
            this.outputTextBox.Text = "";
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(663, 43);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(125, 23);
            this.submitButton.TabIndex = 2;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // startStopListeningButton
            // 
            this.startStopListeningButton.Location = new System.Drawing.Point(663, 10);
            this.startStopListeningButton.Name = "startStopListeningButton";
            this.startStopListeningButton.Size = new System.Drawing.Size(125, 23);
            this.startStopListeningButton.TabIndex = 3;
            this.startStopListeningButton.Text = "Start Listening";
            this.startStopListeningButton.UseVisualStyleBackColor = true;
            this.startStopListeningButton.Click += new System.EventHandler(this.startStopListeningButton_Click);
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(12, 72);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.inputTextBox.Size = new System.Drawing.Size(380, 366);
            this.inputTextBox.TabIndex = 4;
            this.inputTextBox.Text = "";
            // 
            // MyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.startStopListeningButton);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.outputTextBox);
            this.Name = "MyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MyForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox outputTextBox;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.Button startStopListeningButton;
        private System.Windows.Forms.RichTextBox inputTextBox;
    }
}

