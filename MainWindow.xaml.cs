using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Windows.Navigation;


namespace Sharing_Inspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FoldersProps folderProps
        {
            get { return new FoldersProps(Folders.Text); }
            set { }

        }

        private ADactions Domain
        {
            get { return new ADactions(); }
            set { }
        }

        List<AccessRecord> AccessData = new List<AccessRecord>();

        public MainWindow()
        {
            InitializeComponent();

            domain.Text = Domain.domain;
            domainPrefix.Text = Domain.domainPrefix;
            ContainerPath.Text = Domain.ContainerPath;

            accessData.Text = "Folder,FullName,AdGroupName,SamAccountName,Status,FullPath";

            if (Domain.domainAvailability == false)
            {
                MessageBox.Show("Domain is not available. Program will not work correctly.",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            if (!IsAdministrator())
            {
                MessageBox.Show("Application must be launched as Administrator.",
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            }

        }

        private async void submitButton_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();
            Timer.Text = "";


            submitButton.IsEnabled = false;
            submitButton.Content = "Inspecting...";

            // Warn if user provided prefix diffrent from identifed by class method
            if (domainPrefix.Text != Domain.domainPrefix)
            {
                MessageBox.Show("Provided prefix is diffrent from identifed by application. There might be a problem with getting results.",
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            }

            ArrayList folderDataCollection = new ArrayList();
            folderDataCollection = this.folderProps.ShowAccessGroupsOfParentOnly(domainPrefix.Text);

            // add another set of data if user want to scan subfolders
            if (Subfolders.IsChecked == true)
            {
                folderDataCollection.AddRange(this.folderProps.ShowAccessGroupsOfChildsParallel(domainPrefix.Text));
            }

            // Progress
            decimal folderDataCollectionLength = folderDataCollection.Count;
            decimal completedItems = 0;
            Progress.Text += "";

            foreach (Dictionary<string, string> folderData in folderDataCollection)
            {
                ArrayList membersOfThisGroup;

                try
                {
                    membersOfThisGroup = await Task.Run(() => Domain.ShowMembers(folderData["Group"]));
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to query a domain.", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }

                // AD account details
                List<Task<Dictionary<string, string>>> AccountStatusTasks = new List<Task<Dictionary<string, string>>>();

                foreach (string member in membersOfThisGroup)
                {
                    //string[] userAccountInfo = new string[3];
                    AccountStatusTasks.Add(Task.Run(() => Domain.AccountStatus(member)));
                }

                var AccountStatusResults = await Task.WhenAll(AccountStatusTasks);
                //

                foreach (var status in AccountStatusResults)
                {
                    if (folderData["Group"] == status["SAMAccountName"])
                    {
                        folderData["Group"] = "Assigned locally";
                    }

                    AccessRecord Record = new AccessRecord(folderData["SAMAccountName"], status["FullName"], folderData["Group"], status["SAMAccountName"], status["Status"], folderData["FullName"]);
                    AccessData.Add(Record);
                    accessData.Text += "\n" + Record.Folder + "," + Record.FullName + "," + Record.AdGroupName + "," + Record.SamAccountName + "," + Record.Status + "," + Record.FullPath;
                }

                completedItems += 1;

                if (completedItems == folderDataCollectionLength)
                {
                    submitButton.Content = "Completed!";
                    Progress.Text = "";
                }
                else
                {
                    Progress.Text = decimal.Round(((completedItems / folderDataCollectionLength) * 100), 0).ToString() + "%";
                }
            }

            Domain.DisposeContext();
            watch.Stop();
            double timeOfRun = watch.ElapsedMilliseconds / 1000.00;
            Timer.Text += "Elapsed time: " + timeOfRun + " sec.";
        }

        public static string OpenFileBrowserDialog(bool multiselect)
        {
            FolderSelectDialog fbd = new FolderSelectDialog();
            fbd.Multiselect = multiselect;
            fbd.ShowDialog();
            string selected_folders = "";
            if (multiselect)
            {
                string[] names = fbd.FileNames;
                selected_folders = string.Join(";\n", names);
            }
            else
            {
                selected_folders = fbd.FileName;
            }
            return selected_folders;
        }

        private void addFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Folders.Text += OpenFileBrowserDialog(true) + ";\n";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Domain.DisposeContext();
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Grid_Drag(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CSV_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(@"Inspection.csv", accessData.Text);
            Saved.Text = @"Data saved to CSV file. (.\Inspection.csv)";
        }

        private void JSON_Click(object sender, RoutedEventArgs e)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string jsonData = JsonSerializer.Serialize(AccessData, options);
            File.WriteAllText(@"Inspection.json", jsonData);
            Saved.Text = @"Data saved to JSON file. (.\Inspection.json)";
        }

        private void XML_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer XMLdata = new XmlSerializer(AccessData.GetType());
            XMLdata.Serialize(Console.Out, AccessData);
            string path = Environment.CurrentDirectory + "//Inspection.xml";
            FileStream file = File.Create(path);
            XMLdata.Serialize(file, AccessData);
            file.Close();
            Saved.Text = @"Data saved to XML file. (.\Inspection.xml)"; 
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            accessData.Text = "Folder,FullName,AdGroupName,SamAccountName,Status,FullPath";
            submitButton.IsEnabled = true;
            submitButton.Content = "Inspect";
            Timer.Text = "";
            Progress.Text = "";

            AccessData = null;
            AccessData = new List<AccessRecord>();
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ClearFolders_Click(object sender, RoutedEventArgs e)
        {
            Folders.Text = "";
        }
    }
}
