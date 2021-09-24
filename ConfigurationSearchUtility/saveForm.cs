using System;
using System.Windows.Forms;

namespace ConfigurationSearchUtility
{
    public partial class saveForm : Form
    {

        public saveForm()
        {
            InitializeComponent();
        }

        private void saveForm_Load(object sender, EventArgs e)
        {

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            /*string filepath = mainWindow.instance.filepath;
            string orgName = mainWindow.instance.orgName;
            string deviceName = mainWindow.instance.deviceName;
            string deviceType = mainWindow.instance.deviceType;
*/
            mainWindow.instance.SaveButton.PerformClick();
            this.Close();
        }
    }
}
