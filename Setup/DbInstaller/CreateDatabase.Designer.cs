namespace Praetor.Setup
{
    partial class CreateDatabase
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
            this.label1 = new System.Windows.Forms.Label();
            this.serverBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.loginBox = new System.Windows.Forms.TextBox();
            this.passwordBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.passwordLabel2 = new System.Windows.Forms.Label();
            this.passwordBox2 = new System.Windows.Forms.TextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.databaseBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.existingDbButton = new System.Windows.Forms.RadioButton();
            this.newDbButton = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server instance:";
            // 
            // serverBox
            // 
            this.serverBox.Location = new System.Drawing.Point(109, 94);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(169, 20);
            this.serverBox.TabIndex = 1;
            this.serverBox.Text = ".\\SQLEXPRESS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 158);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Login:";
            // 
            // loginBox
            // 
            this.loginBox.Location = new System.Drawing.Point(80, 155);
            this.loginBox.Name = "loginBox";
            this.loginBox.Size = new System.Drawing.Size(198, 20);
            this.loginBox.TabIndex = 3;
            this.loginBox.Text = "mts_user";
            // 
            // passwordBox1
            // 
            this.passwordBox1.Location = new System.Drawing.Point(80, 181);
            this.passwordBox1.Name = "passwordBox1";
            this.passwordBox1.PasswordChar = '*';
            this.passwordBox1.Size = new System.Drawing.Size(198, 20);
            this.passwordBox1.TabIndex = 4;
            this.passwordBox1.Text = "mts_user123";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password:";
            // 
            // passwordLabel2
            // 
            this.passwordLabel2.AutoSize = true;
            this.passwordLabel2.Location = new System.Drawing.Point(19, 210);
            this.passwordLabel2.Name = "passwordLabel2";
            this.passwordLabel2.Size = new System.Drawing.Size(45, 13);
            this.passwordLabel2.TabIndex = 7;
            this.passwordLabel2.Text = "Confirm:";
            // 
            // passwordBox2
            // 
            this.passwordBox2.Location = new System.Drawing.Point(80, 207);
            this.passwordBox2.Name = "passwordBox2";
            this.passwordBox2.PasswordChar = '*';
            this.passwordBox2.Size = new System.Drawing.Size(198, 20);
            this.passwordBox2.TabIndex = 6;
            this.passwordBox2.Text = "mts_user123";
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.Location = new System.Drawing.Point(11, 9);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(166, 20);
            this.infoLabel.TabIndex = 8;
            this.infoLabel.Text = "Database installation";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(122, 233);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(203, 233);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Database name:";
            // 
            // databaseBox
            // 
            this.databaseBox.Location = new System.Drawing.Point(109, 117);
            this.databaseBox.Name = "databaseBox";
            this.databaseBox.Size = new System.Drawing.Size(169, 20);
            this.databaseBox.TabIndex = 12;
            this.databaseBox.Text = "mts";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.existingDbButton);
            this.panel1.Controls.Add(this.newDbButton);
            this.panel1.Location = new System.Drawing.Point(12, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(266, 52);
            this.panel1.TabIndex = 13;
            // 
            // existingDbButton
            // 
            this.existingDbButton.AutoSize = true;
            this.existingDbButton.Location = new System.Drawing.Point(3, 29);
            this.existingDbButton.Name = "existingDbButton";
            this.existingDbButton.Size = new System.Drawing.Size(162, 17);
            this.existingDbButton.TabIndex = 1;
            this.existingDbButton.Text = "Connect to existing database";
            this.existingDbButton.UseVisualStyleBackColor = true;
            this.existingDbButton.CheckedChanged += new System.EventHandler(this.existingDbButton_CheckedChanged);
            // 
            // newDbButton
            // 
            this.newDbButton.AutoSize = true;
            this.newDbButton.Checked = true;
            this.newDbButton.Location = new System.Drawing.Point(3, 6);
            this.newDbButton.Name = "newDbButton";
            this.newDbButton.Size = new System.Drawing.Size(126, 17);
            this.newDbButton.TabIndex = 0;
            this.newDbButton.TabStop = true;
            this.newDbButton.Text = "Create new database";
            this.newDbButton.UseVisualStyleBackColor = true;
            // 
            // CreateDatabase
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(290, 265);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.databaseBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.passwordLabel2);
            this.Controls.Add(this.passwordBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.passwordBox1);
            this.Controls.Add(this.loginBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CreateDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox loginBox;
        private System.Windows.Forms.TextBox passwordBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label passwordLabel2;
        private System.Windows.Forms.TextBox passwordBox2;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox databaseBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton existingDbButton;
        private System.Windows.Forms.RadioButton newDbButton;
    }
}

