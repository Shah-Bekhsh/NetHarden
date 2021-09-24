using System;
using System.IO;
using System.Windows.Forms;

namespace ConfigurationSearchUtility
{
    public partial class mainWindow : Form
    {
        public static string DEST = "";
        public static mainWindow instance;
        public Button SB;

        public string filepath;
        public string deviceName = "";
        public string deviceType = "";
        public string orgName = "";

        Fortinet newFortinetFile = new Fortinet();

        public mainWindow()
        {
            InitializeComponent();
            timer1.Start();
            instance = this;
        }

        private void label5_Click(object sender, EventArgs e)
        {
        
        }

        private void mainWindow_Load(object sender, EventArgs e)
        {
            analyzeButton.Visible = false;
            fileSelectionGroupBox.Visible = false;
            SaveButton.Visible = false;
            SaveButton.Enabled = false;
        }

        //Open Button
        //Allows us to provide a configuration file through the system dialog box.
        private void openButton_Click(object sender, EventArgs e)
        {
            openFD.InitialDirectory = "C:";
            openFD.Title = "Provide Configuration File";
            openFD.FileName = "";
            openFD.Filter = "Configuration File|*.conf";

            if (openFD.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("Please specify a configuration file!");
            }
            else
            {
                filepath = openFD.FileName;
                filePathBox.Text = openFD.FileName;
                //SaveButton.Enabled = true;
                //analyzeButton.Enabled = true;
                deviceNameTextbox.Enabled = true;
                infoSubmitButton.Enabled = true;
                filePathBox.Visible = true;
                analyzeButton.Visible = true;
            }
        }

        //Analyze Button
        //Checks all the commands throughout the configuration file
        private void analyzeButton_Click(object sender, EventArgs e)
        {
            newFortinetFile.analyzeFortiConfig(filepath);
            SaveButton.Enabled = true;
            SaveButton.Visible = true;
            MessageBox.Show("Report Analyzed Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            /*
                        saveForm save = new saveForm();
                        save.Show();*/
        }

        //Save Button
        //Opens system dialog box to ask for save path
        //And then creates the pdf report with all the findings
        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF File|*.pdf", ValidateNames = true })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //Creating the pdfReport Document
                        DEST = sfd.FileName;
                        newFortinetFile.createFortiReport(DEST, orgName, deviceName, deviceType);                        //Adding Page numbers
                        FileInfo file = new FileInfo(DEST);

                        string SRC = "Numbered"+file.Name;
                        SRC = file.DirectoryName + @"\" + SRC;
                        file.Directory.Create();
                        MyPDFmaker maker = new MyPDFmaker();
                        maker.AddPageNumbers(SRC, DEST);
                        if (System.IO.File.Exists(DEST))
                        {
                            try
                            {
                                System.IO.File.Delete(DEST);
                            }
                            catch (System.IO.IOException ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }

                        FileInfo numberedFile = new FileInfo(SRC);
                        string SRC2 = "Watermarked" + numberedFile.Name;
                        SRC2 = file.DirectoryName + @"\" + SRC2;
                        file.Directory.Create();
                        maker = new MyPDFmaker();
                        maker.addWatermark(DEST, SRC);
                        if (System.IO.File.Exists(SRC))
                        {
                            try
                            {
                                System.IO.File.Delete(SRC);
                            }
                            catch (System.IO.IOException ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }

                        //Clear Form
                        filePathBox.Text = "";
                        SaveButton.Visible = false;
                        analyzeButton.Visible = false;
                        deviceNameTextbox.Text = "";
                        organizationTextbox.Text = "";
                        routerRadio.Checked = false;
                        switchRadio.Checked = false;
                        fileSelectionGroupBox.Visible = false;
                        
                        MessageBox.Show("Report Successfully Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime presentDate = DateTime.Now;
            dateLabel.Text = presentDate.ToShortDateString();

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        //changed the name of this button but visual studio doesnt seem to agree so lets leave it at this.
        //Submit button
        private void deviceButton_Click(object sender, EventArgs e)
        {
            if (deviceNameTextbox.Text != "" && organizationTextbox.Text != "")
            {
                if (switchRadio.Checked == true || routerRadio.Checked == true)
                {
                    deviceName = deviceNameTextbox.Text;
                    orgName = organizationTextbox.Text;
                    analyzeButton.Enabled = true;
                    if (switchRadio.Checked == true)
                    {
                        deviceType = "Switch";
                    }
                    if (routerRadio.Checked == true)
                    {
                        deviceType = "Router";
                    }
                    MessageBox.Show("Select Configuration File for Analysis", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fileSelectionGroupBox.Visible = true;
                }
                else
                {
                    MessageBox.Show("Please specify device type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                if (deviceNameTextbox.Text == "")
                {
                    MessageBox.Show("Invalid or no device name entered!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                if (organizationTextbox.Text == "")
                {
                    MessageBox.Show("No organization name entered!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                if (switchRadio.Checked == false && routerRadio.Checked == false)
                {
                    MessageBox.Show("Device type not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void organizationTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateLabel_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
