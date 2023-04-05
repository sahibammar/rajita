using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WasfaProgram
{
    public partial class EditForm : Form
    {
        public string tag;
        public string filePath;
        public EditForm()
        {
            InitializeComponent();
        }

        private void EditForm_Load(object sender, EventArgs e)
        {

            tag = this.Tag.ToString();

            switch (tag)
            {
                case "type":
                    filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\type_data.txt";
                    contentTextBox.Text = File.ReadAllText(@filePath);
                    contentTextBox.RightToLeft = RightToLeft.No;
                break;
                case "quantity":
                    filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\quantity_data.txt";
                    contentTextBox.Text = File.ReadAllText(@filePath);
                    contentTextBox.RightToLeft = RightToLeft.Yes;
                break;
                case "usage":
                    filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\usage_data.txt";
                    contentTextBox.Text = File.ReadAllText(@filePath);
                    contentTextBox.RightToLeft = RightToLeft.Yes;
               break;
               case "duration":
                    filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\duration_data.txt";
                    contentTextBox.Text = File.ReadAllText(@filePath);
                    contentTextBox.RightToLeft = RightToLeft.Yes;
               break;
               case "header":
                    string sAttr0;
                    sAttr0 = ConfigurationManager.AppSettings.Get("Key0");
                    sAttr0 = sAttr0.Replace("_", Environment.NewLine);
                    contentTextBox.SelectAll();
                    contentTextBox.SelectionAlignment = HorizontalAlignment.Center;
                    contentTextBox.Text = sAttr0;
               break;
               case "footer":
                    string sAttr10;
                    sAttr10 = ConfigurationManager.AppSettings.Get("Key10");
                    sAttr10 = sAttr10.Replace("_", Environment.NewLine);
                    contentTextBox.SelectAll();
                    contentTextBox.SelectionAlignment = HorizontalAlignment.Center;
                    contentTextBox.Text = sAttr10;
               break;
            }

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            switch (tag)
            {
                case "type":
                    filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\type_data.txt";
                    File.WriteAllText(@filePath, contentTextBox.Text);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("تم تحديث محتويات قائمة نوع الدواء");
                break;
                case "quantity":
                    filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\quantity_data.txt";
                    File.WriteAllText(@filePath, contentTextBox.Text);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("تم تحديث محتويات قائمة كمية الدواء");
                break;
                case "usage":
                    filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\usage_data.txt";
                    File.WriteAllText(@filePath, contentTextBox.Text);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("تم تحديث محتويات قائمة استخدام الدواء");
                break;
                case "duration":
                    filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\duration_data.txt";
                    File.WriteAllText(@filePath, contentTextBox.Text);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("تم تحديث محتويات قائمة مدة الدواء");
                break;
                case "header":
                    AddUpdateAppSettings("Key0", contentTextBox.Text);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("تم تحديث قالب الراجيتة بشكل دائمي في اعدادات البرنامج");
                break;
                case "footer":
                    AddUpdateAppSettings("Key10", contentTextBox.Text);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("تم تحديث قالب الراجيتة بشكل دائمي في اعدادات البرنامج");
                break;
            }
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

    }
}
