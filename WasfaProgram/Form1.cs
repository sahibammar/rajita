using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading.Tasks;
using Squirrel;
using System.Globalization;
using System.Linq;

namespace WasfaProgram
{
    public partial class Form1 : Form
    {

        public string myHtml;
        public string sAttr0, sAttr10;
        public bool isSecureMode=true;

        public List<DrugRecord> drugRecordList = new List<DrugRecord>();
        public DateTime dateTime = DateTime.UtcNow.Date;

        public class DrugRecord
        {
            public string name { get; set; }
            public int count { get; set; }
            public string type { get; set; }
            public string quantity { get; set; }
            public string usage { get; set; }
            public string duration { get; set; }
            public DrugRecord(string name, int count, string type, string quantity, string usage, string duration)
            {
                this.name = name;
                this.count = count;
                this.type = type;
                this.quantity = quantity;
                this.usage = usage;
                this.duration = duration;
            }
        }

        public Form1()
        {
            InitializeComponent();
            //CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            using (var manager = new UpdateManager(@"C:\Temp\Releases"))
            {
                await manager.UpdateApp();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createWorkDirectory();
            printWebBrowser.Visible = false;

            dateValueLabel.Text = dateTime.ToString("dd/MM/yyyy");
            toolTip1.SetToolTip(button2, "انقر هنا لأضافة الدواء الى الراجيتة");
            toolTip1.SetToolTip(headerEditButton, "انقر هنا لتعديل قالب الراجيتة");
            toolTip1.SetToolTip(footerEditButton, "انقر هنا لتعديل قالب الراجيتة");
            toolTip1.SetToolTip(Printbutton, "انقر هنا لفتح نافذة الطباعة");
            toolTip1.SetToolTip(reviewButton, "انقر هنا لفتح نافذة المعاينة");
            toolTip1.SetToolTip(button1, "انقر هنا لمسج الراجيتة");
            toolTip1.SetToolTip(typeEditButton, "انقر هنا لفتح نافذة تعديل قائمة النوع");
            toolTip1.SetToolTip(typeComboBox, "انقر هنا لأختيار نوع الدواء");
            toolTip1.SetToolTip(quantityEditButton, "انقر هنا لفتح نافذة تعديل قائمة الكمية");
            toolTip1.SetToolTip(quantityComboBox, "انقر هنا لأختيار كمية الدواء");
            toolTip1.SetToolTip(usageEditButton, "انقر هنا لفتح نافذة تعديل قائمة الاستخدام");
            toolTip1.SetToolTip(usageComboBox, "انقر هنا لأختيار طريقة الاستخدام");
            toolTip1.SetToolTip(durationEditButton, "انقر هنا لفتح نافذة تعديل قائمة المدة");
            toolTip1.SetToolTip(durationComboBox, "انقر هنا لأختيار مدة الاستخدام");
            toolTip1.SetToolTip(saveChangesButton, "انقر هنا لحفظ الاعدادات");
            toolTip1.SetToolTip(addDrugButton, "انقر هنا لفتح نافذة اضافة دواء");
            toolTip1.SetToolTip(deleteDrugButton, "انقر هنا لفتح نافذة حذف دواء");
            toolTip1.SetToolTip(searchBox, "اكتب هنا للبحث عن دواء معين");
            toolTip1.SetToolTip(drugsListBox, "انقر مرتين على اسم الدواء لاضافته للأدوية المفضلة");

            if (!(File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" +"\\drugs_data.txt")))
            {
                initiateDrugsDataFile();
            }

            if (!(File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\duration_data.txt")))
                initiateDurationDataFile();

            if (!(File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\type_data.txt")))
                initiateTypeDataFile();

            if (!(File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\quantity_data.txt")))
                initiateQuantityDataFile();

            if (!(File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\usage_data.txt")))
                initiateUsageDataFile();


            sAttr0 = ConfigurationManager.AppSettings.Get("Key0");
            sAttr0 = sAttr0.Replace("_", Environment.NewLine);

            sAttr10 = ConfigurationManager.AppSettings.Get("Key10");
            sAttr10 = sAttr10.Replace("_", Environment.NewLine);

            headerTextBox.SelectAll();
            headerTextBox.SelectionAlignment = HorizontalAlignment.Center;

            footerTextBox.SelectAll();
            footerTextBox.SelectionAlignment = HorizontalAlignment.Center;

            headerTextBox.Text = sAttr0;
            footerTextBox.Text = sAttr10;

            searchBox.Enter += new EventHandler(searchBox_Enter);
            nameTextBox.Enter += new EventHandler(nameTextBox_Enter);
            

            loadFromTypeFileIntoTypeComboBox();
            loadFromQuantityFileIntoQuantityComboBox();
            loadFromUsageFileIntoUsageComboBox();
            loadFromDurationFileIntoDurationComboBox();
            loadFromDrugsDataFileIntoDrugRecordList();
            resortDrugRecordList();
            copyFromDrugRecordListIntoDrugsListBox();

            this.Activated += AfterLoading;
        }

        private void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            switchLangToArabic();
        }

        public void loadFromTypeFileIntoTypeComboBox()
        {
            List<string> typeList = new List<string>();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\type_data.txt";

            var fileStream = new FileStream(@filePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                    typeList.Add(line);                
            }
            typeComboBox.DataSource = typeList;
        }

        public void loadFromQuantityFileIntoQuantityComboBox()
        {
            List<string> quantityList = new List<string>();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\quantity_data.txt";

            var fileStream = new FileStream(@filePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                    quantityList.Add(line);
            }
            quantityComboBox.DataSource = quantityList;
        }

        public void loadFromUsageFileIntoUsageComboBox()
        {
            List<string> usageList = new List<string>();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\usage_data.txt";

            var fileStream = new FileStream(@filePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                    usageList.Add(line);
            }
            usageComboBox.DataSource = usageList;
        }

        public void loadFromDurationFileIntoDurationComboBox()
        {
            List<string> durationList = new List<string>();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\duration_data.txt";

            var fileStream = new FileStream(@filePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                    durationList.Add(line);
            }
            durationComboBox.DataSource = durationList;
        }

        public void loadFromDrugsDataFileIntoDrugRecordList()
        {
            SimpleSecurity ss = new SimpleSecurity();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\drugs_data.txt";

            var fileStream = new FileStream(@filePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    line = ss.Decrypt(line);
                    DrugRecord drugRecord = JsonConvert.DeserializeObject<DrugRecord>(line);
                    drugRecordList.Add(drugRecord);
                }
            }
        }

        public void resortDrugRecordList()
        {
            drugRecordList = drugRecordList.OrderByDescending(d => d.count).ToList();
        }

        public void writeFromDrugUsageListIntoDrugsDataFile()
        {
            SimpleSecurity ss = new SimpleSecurity();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\drugs_data.txt";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filePath))
            {
                foreach (DrugRecord drugRecord in drugRecordList)
                {
                    file.WriteLine(ss.Encrypt(JsonConvert.SerializeObject(drugRecord)));
                }
            }
        }

        public void copyFromDrugRecordListIntoDrugsListBox()
        {
            //copy content of drugUsageList into local drugList and assign it to drugsListBox
            List<string> drugList = new List<string>();
            foreach (DrugRecord drugRecord in drugRecordList)
            {
                drugList.Add(drugRecord.name);
            }
            drugsListBox.DataSource = drugList;
        }

        public void filterFromDrugUsageListDrugsListBox(string prefix)
        {
            //filter content of drugUsageList into drugList and assign it to drugsListBox
            List<string> drugList = new List<string>();
            foreach (DrugRecord drugRecord in drugRecordList)
            {
                if (drugRecord.name.StartsWith(prefix.ToUpper()))
                    drugList.Add(drugRecord.name);
            }
            drugsListBox.DataSource = drugList;
        }

        public void createWorkDirectory()
        {
            string path;

            // creating Rajita directory
            string RajitaDirectoryPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita";
            path = RajitaDirectoryPath;

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine("Rajita: Rajita directory exists already.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("Rajita: Rajita directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }
            catch (Exception e)
            {
                Console.WriteLine("Rajita: The process of creating Rajita directory failed: {0}", e.ToString());
            }
            finally { }

            // creating database directory
            string databaseDirectoryPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database";
            path = databaseDirectoryPath;

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine("Rajita: database directory exists already.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("Rajita: database directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }
            catch (Exception e)
            {
                Console.WriteLine("Rajita: The process of creating database directory failed: {0}", e.ToString());
            }
            finally { }

            // creating documents directory
            string documentsDirectoryPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\documents";
            path = documentsDirectoryPath;

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine("Rajita: documents directory exists already.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("Rajita: documents directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }
            catch (Exception e)
            {
                Console.WriteLine("Rajita: The process of creating documents directory failed: {0}", e.ToString());
            }
            finally { }
        }

        public void initiateDurationDataFile()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\duration_data.txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@filePath))
            {
                using (var streamReader = new StreamReader(asm.GetManifestResourceStream("WasfaProgram.files.duration.txt"), Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        file.WriteLine(line);
                    }
                }
            }
        }

        public void initiateTypeDataFile()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\type_data.txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@filePath))
            {
                using (var streamReader = new StreamReader(asm.GetManifestResourceStream("WasfaProgram.files.type.txt"), Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        file.WriteLine(line);
                    }
                }
            }
        }

        public void initiateQuantityDataFile()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\quantity_data.txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@filePath))
            {
                using (var streamReader = new StreamReader(asm.GetManifestResourceStream("WasfaProgram.files.quantity.txt"), Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        file.WriteLine(line);
                    }
                }
            }
        }

        public void initiateUsageDataFile()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\usage_data.txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@filePath))
            {
                using (var streamReader = new StreamReader(asm.GetManifestResourceStream("WasfaProgram.files.usage.txt"), Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        file.WriteLine(line);
                    }
                }
            }
        }

        public void initiateDrugsDataFile()
        {
            // initiating content of drugs_data.txt file from drugs.txt file
            Assembly asm = Assembly.GetExecutingAssembly();
            List<DrugRecord> tempDrugRecordList = new List<DrugRecord>();
            SimpleSecurity ss = new SimpleSecurity();

            // reading content of drugs.txt file into tempDrugUsageList
            using (var streamReader = new StreamReader(asm.GetManifestResourceStream("WasfaProgram.files.drugs.txt"), Encoding.UTF8))
            {
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    DrugRecord drugRecord = JsonConvert.DeserializeObject<DrugRecord>(line);
                    tempDrugRecordList.Add(drugRecord);

                }
            }

            // writing content from tempDrugUsageList into drugs_data.txt file
            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita" + "\\database" + "\\drugs_data.txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@filePath))
            {
                foreach (DrugRecord drugRecord in tempDrugRecordList)
                {
                    file.WriteLine(ss.Encrypt(JsonConvert.SerializeObject(drugRecord)));
                }
            }
        }

        public void refreshDrugRecordTextBox(DrugRecord selectedDrugRecord)
        {
            drugRecordTextBox.Text = getSingleTreatment();
        }
        // ---------------------- controls related procedures -------------------------------------------

        private void addDrugButton_Click(object sender, EventArgs e)
        {
            string drugName = "";
            searchBox.Text = "";
            switchLangToEnglish();

            if (InputBox("اصافة دواء جديد", "اسم الدواء", ref drugName) == DialogResult.OK)
            {
                drugName = drugName.ToUpper();
                int isFoundIndex = drugsListBox.FindString(drugName);
                if (isFoundIndex != -1)
                {
                    MessageBox.Show("اسم الدواء موجود في قاعدة البيانات: " + Environment.NewLine + Environment.NewLine + drugName);
                    drugsListBox.SetSelected(isFoundIndex, true);
                }
                else
                {
                    drugRecordList.Add(new DrugRecord(drugName, 0, "", "", "", ""));
                    writeFromDrugUsageListIntoDrugsDataFile();

                    drugRecordList.Clear();
                    loadFromDrugsDataFileIntoDrugRecordList();
                    copyFromDrugRecordListIntoDrugsListBox();

                    int index = drugsListBox.FindString(drugName);
                    if (index != -1)
                        drugsListBox.SetSelected(index, true);
                    MessageBox.Show("تم اضافة دواء جديد" + Environment.NewLine + Environment.NewLine + drugName);
                    
                    StatusLabel.Text = "تم اضافة دواء جديد";
                }
            }
        }

        private void searchtBox_TextChanged(object sender, EventArgs e)
        {
            filterFromDrugUsageListDrugsListBox(searchBox.Text);
        }

        private void searchBox_Enter(object sender, EventArgs e)
        {

            if (searchBox.Focused)
                //StatusLabel.Text = "ابدأ بالكتابة هنا للبحث عن الدواء";
                switchLangToEnglish();
                StatusLabel.Text = "تم تحويل اللغة الى الانكليزية";
        }

        private void nameTextBox_Enter(object sender, EventArgs e)
        {
            if (nameTextBox.Focused)
                //StatusLabel.Text = "ابدأ بالكتابة هنا للبحث عن الدواء";
                switchLangToArabic();
            StatusLabel.Text = "تم تحويل اللغة الى العربية";
        }

        private void switchLangToEnglish()
        {
            CultureInfo cultInfo = new CultureInfo("en-us");
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(cultInfo);
        }

        private void switchLangToArabic()
        {
            CultureInfo cultInfo = new CultureInfo("ar-iq");
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(cultInfo);
        }

        private void drugsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (drugsListBox.SelectedItem != null)
            {
                var selectedDrugRecord = drugRecordList.FirstOrDefault(x => x.name == drugsListBox.SelectedItem.ToString());
                if (selectedDrugRecord != null && selectedDrugRecord.count != 1)
                {

                    selectedDrugRecord.count = 1;
                    StatusLabel.Text = "تم اصافة الدواء الى الادوية المفضلة";
                    resortDrugRecordList();
                    copyFromDrugRecordListIntoDrugsListBox();
                    searchBox.Text = "";
                    writeFromDrugUsageListIntoDrugsDataFile();

                    int isFoundIndex = drugsListBox.FindString(selectedDrugRecord.name);
                    if (isFoundIndex != -1)
                    {
                        drugsListBox.SetSelected(isFoundIndex, true);
                    }

                    MessageBox.Show("تم اضافة الدواء الى قائمة الادوية المفضلة");
                }
                else
                {
                    StatusLabel.Text = "يوجد هذا الدواء ضمن الادوية المفضلة..";
                }

            }
        }

        private void Printbutton_Click(object sender, EventArgs e)
        {
            string str = buildHtmlString();
            printWebBrowser.DocumentText = str;
            printWebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(printWebBrowser_DocumentCompleted);

            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita\\documents\\";
            File.WriteAllText(@filePath + nameTextBox.Text+ "_" + dateTime.ToString("ddMMyyyy")  + ".html", str, Encoding.Unicode);
            StatusLabel.Text = "تم حفظ الراجيته في " + Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita\\documents";
        }

        public static void printWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            if (wb.ReadyState.Equals(WebBrowserReadyState.Complete))
                wb.ShowPrintDialog();
        }

        private void reviewButton_Click(object sender, EventArgs e)
        {
            string str = buildHtmlString();
            reviewWebBrowser.DocumentText = str;
            reviewWebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(reviewWebBrowser_DocumentCompleted);

            string filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita\\documents\\";
            File.WriteAllText(@filePath + nameTextBox.Text + "_" + dateTime.ToString("ddMMyyyy") + ".html", str, Encoding.Unicode);
            StatusLabel.Text = "تم حفظ الراجيته في " + Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\Rajita\\documents";
        }

        public static void reviewWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            if (wb.ReadyState.Equals(WebBrowserReadyState.Complete))
                wb.ShowPrintPreviewDialog();
        }

        private void drugsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusLabel.Text = "انقر مرتين لأضافة الدواء الى الادوية المفضلة";

            DrugRecord selectedDrug = drugRecordList.Find(x => x.name == drugsListBox.SelectedItem.ToString());
            refreshDrugRecordTextBox(selectedDrug);

            int isFoundTypeIndex = typeComboBox.FindStringExact(selectedDrug.type);
            if (isFoundTypeIndex != -1) 
                typeComboBox.SelectedIndex= isFoundTypeIndex;
            else
                typeComboBox.SelectedIndex = -1;

            int isFoundQuantityIndex = quantityComboBox.FindStringExact(selectedDrug.quantity);
            if (isFoundQuantityIndex != -1)
                quantityComboBox.SelectedIndex = isFoundQuantityIndex;
            else
                quantityComboBox.SelectedIndex = -1;

            int isFoundUsageIndex = usageComboBox.FindStringExact(selectedDrug.usage);
            if (isFoundUsageIndex != -1)
                usageComboBox.SelectedIndex = isFoundUsageIndex;
            else
                usageComboBox.SelectedIndex = -1;

            int isFoundDurationIndex = durationComboBox.FindStringExact(selectedDrug.duration);
            if (isFoundDurationIndex != -1)
                durationComboBox.SelectedIndex = isFoundDurationIndex;
            else
                durationComboBox.SelectedIndex = -1;
        }

        private static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonCancel.Text = "الغاء";
            buttonOk.Text = "موافق";
            
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonOk.DialogResult = DialogResult.OK;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            treatmentTextBox.AppendText(getSingleTreatment()+ Environment.NewLine);
            searchBox.Text = "";
        }

        private async void Blink()
        {
            while (true)
            {
                await Task.Delay(500);
                label1.BackColor = label1.BackColor == Color.Red ? Color.Green : Color.Red;
            }
        }

        private void contactUsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            contactUsLinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.facebook.com/راچيته-1069882199886602/");
        }

        private void freeVersionlinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            freeVersionlinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start("https://rajita.firebaseapp.com");
        }

        private void typeEditButton_Click(object sender, EventArgs e)
        {
            EditForm editfrm = new EditForm();
            editfrm.Tag = "type";
            if (editfrm.ShowDialog(this) == DialogResult.OK) {
                loadFromTypeFileIntoTypeComboBox();
                typeComboBox.Focus();
            }
            
        }

        private void usageEditButton_Click(object sender, EventArgs e)
        {
            EditForm editfrm = new EditForm();
            editfrm.Tag = "usage";
            if (editfrm.ShowDialog(this) == DialogResult.OK)
            {
                loadFromUsageFileIntoUsageComboBox();
                usageComboBox.Focus();
            }
        }

        private void durationEditButton_Click(object sender, EventArgs e)
        {
            EditForm editfrm = new EditForm();
            editfrm.Tag = "duration";
            if (editfrm.ShowDialog(this) == DialogResult.OK)
            {
                loadFromDurationFileIntoDurationComboBox();
                durationComboBox.Focus();
            }
        }

        private string buildHtmlString()
        {
            string str = "";
            str = str + "<html>";
            str = str + "<head><style> body { width: 14.8cm; height: 21cm; margin: 20mm 30mm 30mm 30mm; } </style> </head>";
            str = str + "<p align = \"center\">";
            str = str + "<body><br/>";

            // header
            foreach (string line in headerTextBox.Lines) str = str + "<b>" + line + "</b>" + "<br/>";
            str = str + "</p>";
            str = str + " <hr style=\"border: 1px solid black;\" />" + "<br/>"; //line

            // name & date
            str = str + "<p>";
            str = str + "<div style = \"float:right;display:inline\">";
            str = str + "الاسم ";
            str = str + nameTextBox.Text;
            str = str + "</div>";

            str = str + "<div style = \"float:left;display:inline\">";
            str = str + "التأريخ ";
            str = str + dateValueLabel.Text;
            str = str + "</div>";
            str = str + "</p>";


            str = str + "<br/>";// break

            // age & weight & sex
            if (ageTextBox.Text != "" || weightTextBox.Text != "")
            {
                str = str + "<p>";
                str = str + "<div style = \"float:right;display:inline\">";
                str = str + "العمر ";
                str = str + ageTextBox.Text;
                str = str + "</div>";

                str = str + "<div style = \"float:left;display:inline\">";
                str = str + "الوزن ";
                str = str + weightTextBox.Text;
                str = str + "</div>";
                str = str + "</p>";

                str = str + "<br/>";// break
            }

            str = str + "<br/><br/>";
            //treatement
            int treatmentTextBoxLines = 0;
            foreach (string line in treatmentTextBox.Lines)
            {
                treatmentTextBoxLines++;
                str = str + line + "<br/>";
            }
            for (int i = 1; i < 20 - treatmentTextBoxLines; i++) str = str + "<p><br/></p>";

            // footer
            str = str + " <hr style=\"border: 1px solid black;\" />"; //line
            str = str + "<p align = \"center\">";
            foreach (string line in footerTextBox.Lines) str = str + line + "<br/>";
            str = str + "</p>";

            //copyright
            str = str + "<p align = \"center\">" + "برنامج راچيته لطباعة الوصفة الطبية" + "</p>";
            str = str + "</body></html>";

            return str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("سيتم مسح الاسم والعمر والوزن والعلاج. هل انت متأكد؟", "انتباه", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                nameTextBox.Text = "";
                ageTextBox.Text = "";
                weightTextBox.Text = "";
                treatmentTextBox.Text = "";
            }
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrugRecord selectedDrug = drugRecordList.Find(x => x.name == drugsListBox.SelectedItem.ToString());
            if (selectedDrug != null && typeComboBox.SelectedItem != null)
            {
                selectedDrug.type= typeComboBox.SelectedItem.ToString();
                refreshDrugRecordTextBox(selectedDrug);
            }
        }

        private void usageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrugRecord selectedDrug = drugRecordList.Find(x => x.name == drugsListBox.SelectedItem.ToString());
            if (selectedDrug != null && usageComboBox.SelectedItem != null)
            {
                selectedDrug.usage = usageComboBox.SelectedItem.ToString();
                refreshDrugRecordTextBox(selectedDrug);
            }
        }

        private void durationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrugRecord selectedDrug = drugRecordList.Find(x => x.name == drugsListBox.SelectedItem.ToString());
            if (selectedDrug != null && durationComboBox.SelectedItem != null)
            {
                selectedDrug.duration = durationComboBox.SelectedItem.ToString();
                refreshDrugRecordTextBox(selectedDrug);
            }
        }

        private void drugRecordTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveChangesButton_Click(object sender, EventArgs e)
        {
            writeFromDrugUsageListIntoDrugsDataFile();
            StatusLabel.Text = "تم حفظ الاعدادات الحالية بشكل دائمي في اعدادات البرنامج";
            MessageBox.Show("تم حفظ الاعدادات الحالية بشكل دائمي في اعدادات البرنامج");
        }

        private void headerEditButton_Click(object sender, EventArgs e)
        {
            EditForm editfrm = new EditForm();
            editfrm.Tag = "header";
            if (editfrm.ShowDialog(this) == DialogResult.OK)
            {
                headerTextBox.Text = ConfigurationManager.AppSettings.Get("Key0");
                headerTextBox.SelectAll();
                headerTextBox.SelectionAlignment = HorizontalAlignment.Center;
            }
        }

        private void footerEditButton_Click(object sender, EventArgs e)
        {
            EditForm editfrm = new EditForm();
            editfrm.Tag = "footer";
            if (editfrm.ShowDialog(this) == DialogResult.OK)
            {
                footerTextBox.Text = ConfigurationManager.AppSettings.Get("Key10");
                footerTextBox.SelectAll();
                footerTextBox.SelectionAlignment = HorizontalAlignment.Center;
            }
        }

        private void quantityEditButton_Click(object sender, EventArgs e)
        {
            EditForm editfrm = new EditForm();
            editfrm.Tag = "quantity";
            if (editfrm.ShowDialog(this) == DialogResult.OK)
            {
                loadFromQuantityFileIntoQuantityComboBox();
                quantityComboBox.Focus();
            }
        }

        private void quantityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrugRecord selectedDrug = drugRecordList.Find(x => x.name == drugsListBox.SelectedItem.ToString());
            if (selectedDrug != null && quantityComboBox.SelectedItem != null)
            {
                selectedDrug.quantity = quantityComboBox.SelectedItem.ToString();
                refreshDrugRecordTextBox(selectedDrug);
            }
        }

        private void deleteDrugButton_Click(object sender, EventArgs e)
        {
            if (drugsListBox.SelectedItem != null)
            {
                var selectedDrugRecord = drugRecordList.FirstOrDefault(x => x.name == drugsListBox.SelectedItem.ToString());
                if (selectedDrugRecord != null && !selectedDrugRecord.name.Contains("___________"))
                {
                    DialogResult dialogResult = MessageBox.Show(
                        selectedDrugRecord.name + Environment.NewLine + Environment.NewLine
                        + "سيتم حذف الدواء بشكل دائمي من اعدادات البرنامج. هل انت متأكد؟", "انتباه", 
                        MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        MessageBox.Show("تم حذف الدواء" + Environment.NewLine + Environment.NewLine + selectedDrugRecord.name);
                        drugRecordList.Remove(selectedDrugRecord);
                        writeFromDrugUsageListIntoDrugsDataFile();
                        resortDrugRecordList();
                        copyFromDrugRecordListIntoDrugsListBox();
                    }
                }
            }
        }

        private void drugsGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private string getSingleTreatment()
        {
            string treatmentStr="";
            DrugRecord selectedDrug = drugRecordList.Find(x => x.name == drugsListBox.SelectedItem.ToString());
            if (selectedDrug != null && !selectedDrug.name.Contains("__________"))
            {
                treatmentStr = treatmentStr + "\u2022 " + selectedDrug.name;
                if (selectedDrug.type != "")
                    treatmentStr = treatmentStr + ", " + selectedDrug.type;
                treatmentStr = treatmentStr + Environment.NewLine;
                if (selectedDrug.quantity != "")
                    treatmentStr = treatmentStr + selectedDrug.quantity + " ";
                if (selectedDrug.usage != "")
                    treatmentStr = treatmentStr + selectedDrug.usage;
                if (selectedDrug.duration != "")
                    treatmentStr = treatmentStr + " لمدة " + selectedDrug.duration;
                treatmentStr = treatmentStr + Environment.NewLine;

            }
            /*if (drugsListBox.SelectedItem != null && !drugsListBox.SelectedItem.ToString().Contains("____"))
            {
                treatmentStr = treatmentStr + "\u2022 " + drugsListBox.SelectedItem.ToString();

                if (typeCheckBox.CheckState == CheckState.Checked)
                    treatmentStr = treatmentStr + ", " + typeComboBox.SelectedItem.ToString() + Environment.NewLine;
                else
                    treatmentStr = treatmentStr + Environment.NewLine;

                if (usageCheckBox.CheckState == CheckState.Checked)
                    treatmentStr = treatmentStr + usageComboBox.SelectedItem.ToString();

                if (durationCheckBox.CheckState == CheckState.Checked)
                    treatmentStr = treatmentStr + " لمدة " + durationComboBox.SelectedItem.ToString();

                treatmentStr = treatmentStr + Environment.NewLine + Environment.NewLine;
            }*/
            return treatmentStr;
            }

        }
}

