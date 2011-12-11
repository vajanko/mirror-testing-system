namespace MTS.Simulator
{
    partial class Simulator
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
            this.startButton = new System.Windows.Forms.Button();
            this.errorButton = new System.Windows.Forms.Button();
            this.insertMirrorButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.unfoldingTime = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.foldingTime = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.powerfoldMax = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.powerfoldMin = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.spiralCurrentMax = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.spiralCurrentMin = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.directionLightCurrentMax = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.directionLightCurrentMin = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.isNewMirror = new System.Windows.Forms.RadioButton();
            this.isOldMirror = new System.Windows.Forms.RadioButton();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.isRightMirror = new System.Windows.Forms.RadioButton();
            this.isLeftMirror = new System.Windows.Forms.RadioButton();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.powerOff = new System.Windows.Forms.RadioButton();
            this.powerOn = new System.Windows.Forms.RadioButton();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.unlockButton = new System.Windows.Forms.Button();
            this.lockButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.unlockTime = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lockTime = new System.Windows.Forms.NumericUpDown();
            this.removeMirrorButton = new System.Windows.Forms.Button();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.rightRubber = new System.Windows.Forms.CheckBox();
            this.leftRubber = new System.Windows.Forms.CheckBox();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.tester1 = new MTS.Simulator.Tester();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unfoldingTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.foldingTime)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.powerfoldMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.powerfoldMin)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spiralCurrentMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spiralCurrentMin)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.directionLightCurrentMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionLightCurrentMin)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unlockTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lockTime)).BeginInit();
            this.groupBox15.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(9, 19);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(63, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.startButton_MouseDown);
            this.startButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.startButton_MouseUp);
            // 
            // errorButton
            // 
            this.errorButton.Location = new System.Drawing.Point(9, 48);
            this.errorButton.Name = "errorButton";
            this.errorButton.Size = new System.Drawing.Size(63, 23);
            this.errorButton.TabIndex = 1;
            this.errorButton.Text = "ErrorAck";
            this.errorButton.UseVisualStyleBackColor = true;
            // 
            // insertMirrorButton
            // 
            this.insertMirrorButton.Location = new System.Drawing.Point(9, 19);
            this.insertMirrorButton.Name = "insertMirrorButton";
            this.insertMirrorButton.Size = new System.Drawing.Size(63, 23);
            this.insertMirrorButton.TabIndex = 3;
            this.insertMirrorButton.Text = "Insert";
            this.insertMirrorButton.UseVisualStyleBackColor = true;
            this.insertMirrorButton.Click += new System.EventHandler(this.insertMirrorButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Location = new System.Drawing.Point(373, 204);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 100);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Powerfold";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.unfoldingTime);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.foldingTime);
            this.groupBox5.Location = new System.Drawing.Point(131, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(127, 75);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Time";
            // 
            // unfoldingTime
            // 
            this.unfoldingTime.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "UnfoldingTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.unfoldingTime.Location = new System.Drawing.Point(57, 48);
            this.unfoldingTime.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.unfoldingTime.Name = "unfoldingTime";
            this.unfoldingTime.Size = new System.Drawing.Size(64, 20);
            this.unfoldingTime.TabIndex = 1;
            this.unfoldingTime.Value = global::MTS.Simulator.Properties.Settings.Default.UnfoldingTime;
            this.unfoldingTime.ValueChanged += new System.EventHandler(this.unfoldingTime_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Unfold:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Fold:";
            // 
            // foldingTime
            // 
            this.foldingTime.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "FoldingTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.foldingTime.Location = new System.Drawing.Point(57, 23);
            this.foldingTime.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.foldingTime.Name = "foldingTime";
            this.foldingTime.Size = new System.Drawing.Size(64, 20);
            this.foldingTime.TabIndex = 0;
            this.foldingTime.Value = global::MTS.Simulator.Properties.Settings.Default.FoldingTime;
            this.foldingTime.ValueChanged += new System.EventHandler(this.foldingTime_ValueChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.powerfoldMax);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.powerfoldMin);
            this.groupBox4.Location = new System.Drawing.Point(6, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(119, 75);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Current";
            // 
            // powerfoldMax
            // 
            this.powerfoldMax.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "PowerfoldMaxCurrent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.powerfoldMax.Location = new System.Drawing.Point(39, 48);
            this.powerfoldMax.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.powerfoldMax.Name = "powerfoldMax";
            this.powerfoldMax.Size = new System.Drawing.Size(74, 20);
            this.powerfoldMax.TabIndex = 1;
            this.powerfoldMax.Value = global::MTS.Simulator.Properties.Settings.Default.PowerfoldMaxCurrent;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Max:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Min:";
            // 
            // powerfoldMin
            // 
            this.powerfoldMin.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "PowerfoldMinCurrent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.powerfoldMin.Location = new System.Drawing.Point(39, 23);
            this.powerfoldMin.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.powerfoldMin.Name = "powerfoldMin";
            this.powerfoldMin.Size = new System.Drawing.Size(74, 20);
            this.powerfoldMin.TabIndex = 0;
            this.powerfoldMin.Value = global::MTS.Simulator.Properties.Settings.Default.PowerfoldMinCurrent;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Location = new System.Drawing.Point(373, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(264, 100);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Spiral";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.spiralCurrentMax);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.spiralCurrentMin);
            this.groupBox6.Location = new System.Drawing.Point(6, 19);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(119, 75);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Current";
            // 
            // spiralCurrentMax
            // 
            this.spiralCurrentMax.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "SpiralMaxCurrent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.spiralCurrentMax.Location = new System.Drawing.Point(39, 48);
            this.spiralCurrentMax.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.spiralCurrentMax.Name = "spiralCurrentMax";
            this.spiralCurrentMax.Size = new System.Drawing.Size(74, 20);
            this.spiralCurrentMax.TabIndex = 1;
            this.spiralCurrentMax.Value = global::MTS.Simulator.Properties.Settings.Default.SpiralMaxCurrent;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Max:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Min:";
            // 
            // spiralCurrentMin
            // 
            this.spiralCurrentMin.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "SpiralMinCurrent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.spiralCurrentMin.Location = new System.Drawing.Point(39, 23);
            this.spiralCurrentMin.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.spiralCurrentMin.Name = "spiralCurrentMin";
            this.spiralCurrentMin.Size = new System.Drawing.Size(74, 20);
            this.spiralCurrentMin.TabIndex = 0;
            this.spiralCurrentMin.Value = global::MTS.Simulator.Properties.Settings.Default.SpiralMinCurrent;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox7);
            this.groupBox3.Location = new System.Drawing.Point(373, 310);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(264, 100);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Direction light";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.directionLightCurrentMax);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.label9);
            this.groupBox7.Controls.Add(this.directionLightCurrentMin);
            this.groupBox7.Location = new System.Drawing.Point(6, 18);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(119, 75);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Current";
            // 
            // directionLightCurrentMax
            // 
            this.directionLightCurrentMax.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "BlinkerMaxCurrent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.directionLightCurrentMax.Location = new System.Drawing.Point(39, 48);
            this.directionLightCurrentMax.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.directionLightCurrentMax.Name = "directionLightCurrentMax";
            this.directionLightCurrentMax.Size = new System.Drawing.Size(74, 20);
            this.directionLightCurrentMax.TabIndex = 1;
            this.directionLightCurrentMax.Value = global::MTS.Simulator.Properties.Settings.Default.BlinkerMaxCurrent;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Max:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Min:";
            // 
            // directionLightCurrentMin
            // 
            this.directionLightCurrentMin.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "BlinkerMinCurrent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.directionLightCurrentMin.Location = new System.Drawing.Point(39, 23);
            this.directionLightCurrentMin.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.directionLightCurrentMin.Name = "directionLightCurrentMin";
            this.directionLightCurrentMin.Size = new System.Drawing.Size(74, 20);
            this.directionLightCurrentMin.TabIndex = 0;
            this.directionLightCurrentMin.Value = global::MTS.Simulator.Properties.Settings.Default.BlinkerMinCurrent;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(542, 523);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Listen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.listenButton_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(623, 523);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Disconnect";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.isNewMirror);
            this.groupBox8.Controls.Add(this.isOldMirror);
            this.groupBox8.Location = new System.Drawing.Point(78, 374);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(84, 66);
            this.groupBox8.TabIndex = 13;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Mirror Type";
            // 
            // isNewMirror
            // 
            this.isNewMirror.AutoSize = true;
            this.isNewMirror.Location = new System.Drawing.Point(6, 42);
            this.isNewMirror.Name = "isNewMirror";
            this.isNewMirror.Size = new System.Drawing.Size(47, 17);
            this.isNewMirror.TabIndex = 1;
            this.isNewMirror.TabStop = true;
            this.isNewMirror.Text = "New";
            this.isNewMirror.UseVisualStyleBackColor = true;
            // 
            // isOldMirror
            // 
            this.isOldMirror.AutoSize = true;
            this.isOldMirror.Checked = true;
            this.isOldMirror.Location = new System.Drawing.Point(6, 19);
            this.isOldMirror.Name = "isOldMirror";
            this.isOldMirror.Size = new System.Drawing.Size(41, 17);
            this.isOldMirror.TabIndex = 0;
            this.isOldMirror.TabStop = true;
            this.isOldMirror.Text = "Old";
            this.isOldMirror.UseVisualStyleBackColor = true;
            this.isOldMirror.CheckedChanged += new System.EventHandler(this.isOldMirror_CheckedChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.isRightMirror);
            this.groupBox9.Controls.Add(this.isLeftMirror);
            this.groupBox9.Location = new System.Drawing.Point(168, 374);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(80, 66);
            this.groupBox9.TabIndex = 14;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Orientation";
            // 
            // isRightMirror
            // 
            this.isRightMirror.AutoSize = true;
            this.isRightMirror.Location = new System.Drawing.Point(6, 42);
            this.isRightMirror.Name = "isRightMirror";
            this.isRightMirror.Size = new System.Drawing.Size(50, 17);
            this.isRightMirror.TabIndex = 1;
            this.isRightMirror.TabStop = true;
            this.isRightMirror.Text = "Right";
            this.isRightMirror.UseVisualStyleBackColor = true;
            // 
            // isLeftMirror
            // 
            this.isLeftMirror.AutoSize = true;
            this.isLeftMirror.Checked = true;
            this.isLeftMirror.Location = new System.Drawing.Point(6, 19);
            this.isLeftMirror.Name = "isLeftMirror";
            this.isLeftMirror.Size = new System.Drawing.Size(43, 17);
            this.isLeftMirror.TabIndex = 0;
            this.isLeftMirror.TabStop = true;
            this.isLeftMirror.Text = "Left";
            this.isLeftMirror.UseVisualStyleBackColor = true;
            this.isLeftMirror.CheckedChanged += new System.EventHandler(this.isLeftMirror_CheckedChanged);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.startButton);
            this.groupBox10.Controls.Add(this.errorButton);
            this.groupBox10.Location = new System.Drawing.Point(97, 447);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(84, 80);
            this.groupBox10.TabIndex = 15;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Buttons";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.powerOff);
            this.groupBox11.Controls.Add(this.powerOn);
            this.groupBox11.Location = new System.Drawing.Point(15, 374);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(57, 66);
            this.groupBox11.TabIndex = 16;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Power";
            // 
            // powerOff
            // 
            this.powerOff.AutoSize = true;
            this.powerOff.Checked = true;
            this.powerOff.Location = new System.Drawing.Point(6, 41);
            this.powerOff.Name = "powerOff";
            this.powerOff.Size = new System.Drawing.Size(39, 17);
            this.powerOff.TabIndex = 8;
            this.powerOff.TabStop = true;
            this.powerOff.Text = "Off";
            this.powerOff.UseVisualStyleBackColor = true;
            this.powerOff.CheckedChanged += new System.EventHandler(this.powerOff_CheckedChanged);
            // 
            // powerOn
            // 
            this.powerOn.AutoSize = true;
            this.powerOn.Location = new System.Drawing.Point(6, 18);
            this.powerOn.Name = "powerOn";
            this.powerOn.Size = new System.Drawing.Size(39, 17);
            this.powerOn.TabIndex = 7;
            this.powerOn.Text = "On";
            this.powerOn.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.unlockButton);
            this.groupBox13.Controls.Add(this.lockButton);
            this.groupBox13.Controls.Add(this.label10);
            this.groupBox13.Controls.Add(this.unlockTime);
            this.groupBox13.Controls.Add(this.label1);
            this.groupBox13.Controls.Add(this.lockTime);
            this.groupBox13.Location = new System.Drawing.Point(373, 12);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(264, 77);
            this.groupBox13.TabIndex = 19;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Lock/Unlock";
            // 
            // unlockButton
            // 
            this.unlockButton.Location = new System.Drawing.Point(156, 43);
            this.unlockButton.Name = "unlockButton";
            this.unlockButton.Size = new System.Drawing.Size(72, 22);
            this.unlockButton.TabIndex = 8;
            this.unlockButton.Text = "Unlock";
            this.unlockButton.UseVisualStyleBackColor = true;
            this.unlockButton.Click += new System.EventHandler(this.unlockButton_Click);
            // 
            // lockButton
            // 
            this.lockButton.Location = new System.Drawing.Point(156, 17);
            this.lockButton.Name = "lockButton";
            this.lockButton.Size = new System.Drawing.Size(72, 22);
            this.lockButton.TabIndex = 7;
            this.lockButton.Text = "Lock";
            this.lockButton.UseVisualStyleBackColor = true;
            this.lockButton.Click += new System.EventHandler(this.lockButton_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Unlock time:";
            // 
            // unlockTime
            // 
            this.unlockTime.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "UnlockTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.unlockTime.Location = new System.Drawing.Point(86, 46);
            this.unlockTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.unlockTime.Name = "unlockTime";
            this.unlockTime.Size = new System.Drawing.Size(64, 20);
            this.unlockTime.TabIndex = 5;
            this.unlockTime.Value = global::MTS.Simulator.Properties.Settings.Default.UnlockTime;
            this.unlockTime.ValueChanged += new System.EventHandler(this.unlockTime_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Lock time:";
            // 
            // lockTime
            // 
            this.lockTime.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::MTS.Simulator.Properties.Settings.Default, "LockTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lockTime.Location = new System.Drawing.Point(86, 20);
            this.lockTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.lockTime.Name = "lockTime";
            this.lockTime.Size = new System.Drawing.Size(64, 20);
            this.lockTime.TabIndex = 3;
            this.lockTime.Value = global::MTS.Simulator.Properties.Settings.Default.LockTime;
            this.lockTime.ValueChanged += new System.EventHandler(this.lockTime_ValueChanged);
            // 
            // removeMirrorButton
            // 
            this.removeMirrorButton.Location = new System.Drawing.Point(9, 48);
            this.removeMirrorButton.Name = "removeMirrorButton";
            this.removeMirrorButton.Size = new System.Drawing.Size(63, 23);
            this.removeMirrorButton.TabIndex = 21;
            this.removeMirrorButton.Text = "Remove";
            this.removeMirrorButton.UseVisualStyleBackColor = true;
            this.removeMirrorButton.Click += new System.EventHandler(this.removeMirrorButton_Click);
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.rightRubber);
            this.groupBox15.Controls.Add(this.leftRubber);
            this.groupBox15.Location = new System.Drawing.Point(254, 374);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(74, 66);
            this.groupBox15.TabIndex = 22;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Rubber";
            // 
            // rightRubber
            // 
            this.rightRubber.AutoSize = true;
            this.rightRubber.Checked = global::MTS.Simulator.Properties.Settings.Default.RightRubber;
            this.rightRubber.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MTS.Simulator.Properties.Settings.Default, "RightRubber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rightRubber.Location = new System.Drawing.Point(9, 42);
            this.rightRubber.Name = "rightRubber";
            this.rightRubber.Size = new System.Drawing.Size(51, 17);
            this.rightRubber.TabIndex = 1;
            this.rightRubber.Text = "Right";
            this.rightRubber.UseVisualStyleBackColor = true;
            this.rightRubber.CheckedChanged += new System.EventHandler(this.rightRubber_CheckedChanged);
            // 
            // leftRubber
            // 
            this.leftRubber.AutoSize = true;
            this.leftRubber.Checked = global::MTS.Simulator.Properties.Settings.Default.LeftRubber;
            this.leftRubber.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MTS.Simulator.Properties.Settings.Default, "LeftRubber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.leftRubber.Location = new System.Drawing.Point(9, 19);
            this.leftRubber.Name = "leftRubber";
            this.leftRubber.Size = new System.Drawing.Size(44, 17);
            this.leftRubber.TabIndex = 0;
            this.leftRubber.Text = "Left";
            this.leftRubber.UseVisualStyleBackColor = true;
            this.leftRubber.CheckedChanged += new System.EventHandler(this.leftRubber_CheckedChanged);
            // 
            // elementHost1
            // 
            this.elementHost1.BackColor = System.Drawing.Color.White;
            this.elementHost1.BackColorTransparent = true;
            this.elementHost1.Location = new System.Drawing.Point(15, 12);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(346, 356);
            this.elementHost1.TabIndex = 18;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.tester1;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.insertMirrorButton);
            this.groupBox12.Controls.Add(this.removeMirrorButton);
            this.groupBox12.Location = new System.Drawing.Point(12, 446);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(79, 81);
            this.groupBox12.TabIndex = 23;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Mirror";
            // 
            // Simulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 568);
            this.Controls.Add(this.groupBox12);
            this.Controls.Add(this.groupBox15);
            this.Controls.Add(this.groupBox13);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Simulator";
            this.Text = "MTS Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Simulator_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unfoldingTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.foldingTime)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.powerfoldMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.powerfoldMin)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spiralCurrentMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spiralCurrentMin)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.directionLightCurrentMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionLightCurrentMin)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.unlockTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lockTime)).EndInit();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button errorButton;
        private System.Windows.Forms.Button insertMirrorButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown powerfoldMax;
        private System.Windows.Forms.NumericUpDown powerfoldMin;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.NumericUpDown unfoldingTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown foldingTime;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.NumericUpDown spiralCurrentMax;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown spiralCurrentMin;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.NumericUpDown directionLightCurrentMax;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown directionLightCurrentMin;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton isNewMirror;
        private System.Windows.Forms.RadioButton isOldMirror;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RadioButton isRightMirror;
        private System.Windows.Forms.RadioButton isLeftMirror;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown unlockTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown lockTime;
        private System.Windows.Forms.Button unlockButton;
        private System.Windows.Forms.Button lockButton;
        private Tester tester1;
        private System.Windows.Forms.Button removeMirrorButton;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.CheckBox rightRubber;
        private System.Windows.Forms.CheckBox leftRubber;
        private System.Windows.Forms.RadioButton powerOff;
        private System.Windows.Forms.RadioButton powerOn;
        private System.Windows.Forms.GroupBox groupBox12;
    }
}

