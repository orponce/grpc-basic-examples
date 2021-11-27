namespace WFormsEx3ServerStreamingRpc
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
            this.buttonStartClient = new System.Windows.Forms.Button();
            this.buttonSendRequest = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonStartClient
            // 
            this.buttonStartClient.Location = new System.Drawing.Point(43, 46);
            this.buttonStartClient.Name = "buttonStartClient";
            this.buttonStartClient.Size = new System.Drawing.Size(129, 50);
            this.buttonStartClient.TabIndex = 0;
            this.buttonStartClient.Text = "Start client";
            this.buttonStartClient.UseVisualStyleBackColor = true;
            this.buttonStartClient.Click += new System.EventHandler(this.buttonStartClient_Click);
            // 
            // buttonSendRequest
            // 
            this.buttonSendRequest.Location = new System.Drawing.Point(43, 126);
            this.buttonSendRequest.Name = "buttonSendRequest";
            this.buttonSendRequest.Size = new System.Drawing.Size(129, 50);
            this.buttonSendRequest.TabIndex = 1;
            this.buttonSendRequest.Text = "Send request";
            this.buttonSendRequest.UseVisualStyleBackColor = true;
            this.buttonSendRequest.Click += new System.EventHandler(this.buttonSendRequest_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(199, 46);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(598, 160);
            this.textBox1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 256);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonSendRequest);
            this.Controls.Add(this.buttonStartClient);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonStartClient;
        private Button buttonSendRequest;
        private TextBox textBox1;
    }
}