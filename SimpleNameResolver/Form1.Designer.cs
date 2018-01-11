namespace SimpleNameResolver
{
    partial class ResolverForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && (components != null) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.resolveBtn = new System.Windows.Forms.Button();
            this.domainNameTxtBox = new System.Windows.Forms.TextBox();
            this.ipsListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // resolveBtn
            // 
            this.resolveBtn.Location = new System.Drawing.Point(297, 126);
            this.resolveBtn.Name = "resolveBtn";
            this.resolveBtn.Size = new System.Drawing.Size(75, 36);
            this.resolveBtn.TabIndex = 0;
            this.resolveBtn.Text = "Resolve";
            this.resolveBtn.UseVisualStyleBackColor = true;
            this.resolveBtn.Click += new System.EventHandler(this.resolveBtn_Click);
            // 
            // domainNameTxtBox
            // 
            this.domainNameTxtBox.Location = new System.Drawing.Point(12, 15);
            this.domainNameTxtBox.Name = "domainNameTxtBox";
            this.domainNameTxtBox.Size = new System.Drawing.Size(268, 20);
            this.domainNameTxtBox.TabIndex = 1;
            // 
            // ipsListBox
            // 
            this.ipsListBox.FormattingEnabled = true;
            this.ipsListBox.Location = new System.Drawing.Point(12, 41);
            this.ipsListBox.Name = "ipsListBox";
            this.ipsListBox.Size = new System.Drawing.Size(268, 121);
            this.ipsListBox.TabIndex = 2;
            // 
            // ResolverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 176);
            this.Controls.Add(this.ipsListBox);
            this.Controls.Add(this.domainNameTxtBox);
            this.Controls.Add(this.resolveBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ResolverForm";
            this.Text = "SimpleNameResolver";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button resolveBtn;
        private System.Windows.Forms.TextBox domainNameTxtBox;
        private System.Windows.Forms.ListBox ipsListBox;
    }
}

