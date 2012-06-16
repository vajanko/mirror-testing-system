namespace MTS.Setup
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.windowsButton = new System.Windows.Forms.RadioButton();
            this.sqlButton = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server instance:";
            // 
            // serverBox
            // 
            this.serverBox.Location = new System.Drawing.Point(97, 68);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(169, 20);
            this.serverBox.TabIndex = 1;
            this.serverBox.Text = ".\\SQLEXPRESS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "User name:";
            // 
            // loginBox
            // 
            this.loginBox.Location = new System.Drawing.Point(73, 129);
            this.loginBox.Name = "loginBox";
            this.loginBox.Size = new System.Drawing.Size(193, 20);
            this.loginBox.TabIndex = 3;
            this.loginBox.Text = "mts_user";
            // 
            // passwordBox1
            // 
            this.passwordBox1.Location = new System.Drawing.Point(73, 155);
            this.passwordBox1.Name = "passwordBox1";
            this.passwordBox1.PasswordChar = '*';
            this.passwordBox1.Size = new System.Drawing.Size(193, 20);
            this.passwordBox1.TabIndex = 4;
            this.passwordBox1.Text = "mts_user123";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password:";
            // 
            // passwordLabel2
            // 
            this.passwordLabel2.AutoSize = true;
            this.passwordLabel2.Location = new System.Drawing.Point(7, 184);
            this.passwordLabel2.Name = "passwordLabel2";
            this.passwordLabel2.Size = new System.Drawing.Size(45, 13);
            this.passwordLabel2.TabIndex = 7;
            this.passwordLabel2.Text = "Confirm:";
            // 
            // passwordBox2
            // 
            this.passwordBox2.Location = new System.Drawing.Point(73, 181);
            this.passwordBox2.Name = "passwordBox2";
            this.passwordBox2.PasswordChar = '*';
            this.passwordBox2.Size = new System.Drawing.Size(193, 20);
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
            this.cancelButton.Location = new System.Drawing.Point(110, 207);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(191, 207);
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
            this.label6.Location = new System.Drawing.Point(6, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Database name:";
            // 
            // databaseBox
            // 
            this.databaseBox.Location = new System.Drawing.Point(97, 91);
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
            this.panel1.Size = new System.Drawing.Size(276, 52);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.sqlButton);
            this.groupBox1.Controls.Add(this.windowsButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.serverBox);
            this.groupBox1.Controls.Add(this.databaseBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.loginBox);
            this.groupBox1.Controls.Add(this.okButton);
            this.groupBox1.Controls.Add(this.passwordBox1);
            this.groupBox1.Controls.Add(this.cancelButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.passwordBox2);
            this.groupBox1.Controls.Add(this.passwordLabel2);
            this.groupBox1.Location = new System.Drawing.Point(12, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 237);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log on to the server";
            // 
            // windowsButton
            // 
            this.windowsButton.AutoSize = true;
            this.windowsButton.Checked = true;
            this.windowsButton.Location = new System.Drawing.Point(10, 19);
            this.windowsButton.Name = "windowsButton";
            this.windowsButton.Size = new System.Drawing.Size(162, 17);
            this.windowsButton.TabIndex = 13;
            this.windowsButton.TabStop = true;
            this.windowsButton.Text = "Use Windows Authentication";
            this.windowsButton.UseVisualStyleBackColor = true;
            // 
            // sqlButton
            // 
            this.sqlButton.AutoSize = true;
            this.sqlButton.Location = new System.Drawing.Point(10, 42);
            this.sqlButton.Name = "sqlButton";
            this.sqlButton.Size = new System.Drawing.Size(173, 17);
            this.sqlButton.TabIndex = 14;
            this.sqlButton.Text = "Use SQL Server Authentication";
            this.sqlButton.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 207);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Test Connection";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // CreateDatabase
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(299, 340);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.infoLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CreateDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton sqlButton;
        private System.Windows.Forms.RadioButton windowsButton;
        private System.Windows.Forms.Button button1;
    }
}

