
namespace ConfigurationSearchUtility
{
    partial class mainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainWindow));
            this.openButton = new System.Windows.Forms.Button();
            this.fileSelectionGroupBox = new System.Windows.Forms.GroupBox();
            this.filePathBox = new System.Windows.Forms.TextBox();
            this.informationGroupBox = new System.Windows.Forms.GroupBox();
            this.organizationTextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.routerRadio = new System.Windows.Forms.RadioButton();
            this.switchRadio = new System.Windows.Forms.RadioButton();
            this.infoSubmitButton = new System.Windows.Forms.Button();
            this.deviceNameTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.openFD = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.analyzeButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dateLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.fileSelectionGroupBox.SuspendLayout();
            this.informationGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // openButton
            // 
            this.openButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(188)))), ((int)(((byte)(37)))));
            this.openButton.FlatAppearance.BorderSize = 0;
            this.openButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openButton.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.openButton.Location = new System.Drawing.Point(335, 33);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(323, 47);
            this.openButton.TabIndex = 2;
            this.openButton.Text = "Select Configuration File";
            this.openButton.UseVisualStyleBackColor = false;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // fileSelectionGroupBox
            // 
            this.fileSelectionGroupBox.Controls.Add(this.filePathBox);
            this.fileSelectionGroupBox.Controls.Add(this.openButton);
            this.fileSelectionGroupBox.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileSelectionGroupBox.ForeColor = System.Drawing.Color.White;
            this.fileSelectionGroupBox.Location = new System.Drawing.Point(24, 416);
            this.fileSelectionGroupBox.Name = "fileSelectionGroupBox";
            this.fileSelectionGroupBox.Size = new System.Drawing.Size(1009, 169);
            this.fileSelectionGroupBox.TabIndex = 3;
            this.fileSelectionGroupBox.TabStop = false;
            this.fileSelectionGroupBox.Text = "File Selection";
            this.fileSelectionGroupBox.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // filePathBox
            // 
            this.filePathBox.BackColor = System.Drawing.Color.White;
            this.filePathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filePathBox.Enabled = false;
            this.filePathBox.Font = new System.Drawing.Font("Segoe UI Symbol", 12F);
            this.filePathBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.filePathBox.Location = new System.Drawing.Point(95, 103);
            this.filePathBox.Multiline = true;
            this.filePathBox.Name = "filePathBox";
            this.filePathBox.Size = new System.Drawing.Size(825, 47);
            this.filePathBox.TabIndex = 1;
            this.filePathBox.Visible = false;
            // 
            // informationGroupBox
            // 
            this.informationGroupBox.Controls.Add(this.organizationTextbox);
            this.informationGroupBox.Controls.Add(this.label5);
            this.informationGroupBox.Controls.Add(this.routerRadio);
            this.informationGroupBox.Controls.Add(this.switchRadio);
            this.informationGroupBox.Controls.Add(this.infoSubmitButton);
            this.informationGroupBox.Controls.Add(this.deviceNameTextbox);
            this.informationGroupBox.Controls.Add(this.label4);
            this.informationGroupBox.Controls.Add(this.label3);
            this.informationGroupBox.Controls.Add(this.label2);
            this.informationGroupBox.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold);
            this.informationGroupBox.ForeColor = System.Drawing.Color.White;
            this.informationGroupBox.Location = new System.Drawing.Point(24, 100);
            this.informationGroupBox.Name = "informationGroupBox";
            this.informationGroupBox.Size = new System.Drawing.Size(1009, 302);
            this.informationGroupBox.TabIndex = 4;
            this.informationGroupBox.TabStop = false;
            this.informationGroupBox.Text = "Provide Organization Information";
            this.informationGroupBox.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // organizationTextbox
            // 
            this.organizationTextbox.Font = new System.Drawing.Font("Segoe UI Symbol", 12F);
            this.organizationTextbox.Location = new System.Drawing.Point(335, 181);
            this.organizationTextbox.Name = "organizationTextbox";
            this.organizationTextbox.Size = new System.Drawing.Size(660, 34);
            this.organizationTextbox.TabIndex = 14;
            this.organizationTextbox.TextChanged += new System.EventHandler(this.organizationTextbox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(19, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(200, 28);
            this.label5.TabIndex = 13;
            this.label5.Text = "Organization Name";
            // 
            // routerRadio
            // 
            this.routerRadio.AutoSize = true;
            this.routerRadio.ForeColor = System.Drawing.Color.White;
            this.routerRadio.Location = new System.Drawing.Point(589, 116);
            this.routerRadio.Name = "routerRadio";
            this.routerRadio.Size = new System.Drawing.Size(93, 32);
            this.routerRadio.TabIndex = 12;
            this.routerRadio.TabStop = true;
            this.routerRadio.Text = "router";
            this.routerRadio.UseVisualStyleBackColor = true;
            // 
            // switchRadio
            // 
            this.switchRadio.AutoSize = true;
            this.switchRadio.ForeColor = System.Drawing.Color.White;
            this.switchRadio.Location = new System.Drawing.Point(335, 116);
            this.switchRadio.Name = "switchRadio";
            this.switchRadio.Size = new System.Drawing.Size(93, 32);
            this.switchRadio.TabIndex = 11;
            this.switchRadio.TabStop = true;
            this.switchRadio.Text = "switch";
            this.switchRadio.UseVisualStyleBackColor = true;
            this.switchRadio.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // infoSubmitButton
            // 
            this.infoSubmitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(188)))), ((int)(((byte)(37)))));
            this.infoSubmitButton.FlatAppearance.BorderSize = 0;
            this.infoSubmitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.infoSubmitButton.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoSubmitButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.infoSubmitButton.Location = new System.Drawing.Point(424, 236);
            this.infoSubmitButton.Name = "infoSubmitButton";
            this.infoSubmitButton.Size = new System.Drawing.Size(132, 47);
            this.infoSubmitButton.TabIndex = 3;
            this.infoSubmitButton.Text = "Submit";
            this.infoSubmitButton.UseVisualStyleBackColor = false;
            this.infoSubmitButton.Click += new System.EventHandler(this.deviceButton_Click);
            // 
            // deviceNameTextbox
            // 
            this.deviceNameTextbox.Font = new System.Drawing.Font("Segoe UI Symbol", 12F);
            this.deviceNameTextbox.Location = new System.Drawing.Point(335, 46);
            this.deviceNameTextbox.Name = "deviceNameTextbox";
            this.deviceNameTextbox.Size = new System.Drawing.Size(660, 34);
            this.deviceNameTextbox.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(19, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 28);
            this.label4.TabIndex = 5;
            this.label4.Text = "Device Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(79)))), ((int)(((byte)(125)))));
            this.label3.Location = new System.Drawing.Point(297, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 28);
            this.label3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(19, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 28);
            this.label2.TabIndex = 1;
            this.label2.Text = "Device Name";
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(188)))), ((int)(((byte)(37)))));
            this.SaveButton.FlatAppearance.BorderSize = 0;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.SaveButton.Location = new System.Drawing.Point(544, 602);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(247, 47);
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Text = "Save PDF Report";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // openFD
            // 
            this.openFD.FileName = "openFileDialog1";
            // 
            // analyzeButton
            // 
            this.analyzeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(188)))), ((int)(((byte)(37)))));
            this.analyzeButton.FlatAppearance.BorderSize = 0;
            this.analyzeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.analyzeButton.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.analyzeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.analyzeButton.Location = new System.Drawing.Point(260, 602);
            this.analyzeButton.Name = "analyzeButton";
            this.analyzeButton.Size = new System.Drawing.Size(192, 47);
            this.analyzeButton.TabIndex = 5;
            this.analyzeButton.Text = "Analyze File";
            this.analyzeButton.UseVisualStyleBackColor = false;
            this.analyzeButton.Click += new System.EventHandler(this.analyzeButton_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dateLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F);
            this.dateLabel.ForeColor = System.Drawing.Color.White;
            this.dateLabel.Location = new System.Drawing.Point(883, 45);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(73, 26);
            this.dateLabel.TabIndex = 7;
            this.dateLabel.Text = "label5";
            this.dateLabel.Click += new System.EventHandler(this.dateLabel_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(773, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 28);
            this.label6.TabIndex = 8;
            this.label6.Text = "Date:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Century", 34.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(188)))), ((int)(((byte)(37)))));
            this.label7.Location = new System.Drawing.Point(12, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(341, 70);
            this.label7.TabIndex = 9;
            this.label7.Text = "NetHarden";
            // 
            // mainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(65)))), ((int)(((byte)(66)))));
            this.ClientSize = new System.Drawing.Size(1058, 670);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateLabel);
            this.Controls.Add(this.analyzeButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.informationGroupBox);
            this.Controls.Add(this.fileSelectionGroupBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NetHarden By Digital Arrays";
            this.Load += new System.EventHandler(this.mainWindow_Load);
            this.fileSelectionGroupBox.ResumeLayout(false);
            this.fileSelectionGroupBox.PerformLayout();
            this.informationGroupBox.ResumeLayout(false);
            this.informationGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.GroupBox fileSelectionGroupBox;
        private System.Windows.Forms.GroupBox informationGroupBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFD;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button analyzeButton;
        private System.Windows.Forms.TextBox deviceNameTextbox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.Button infoSubmitButton;
        private System.Windows.Forms.RadioButton switchRadio;
        private System.Windows.Forms.TextBox organizationTextbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton routerRadio;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox filePathBox;
        public System.Windows.Forms.Button SaveButton;
    }
}