using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

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

            if (Domain.domainAvailability == false)
            {
                MessageBox.Show("Domain is not available. Program will not work correctly.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            accessData.Text = "LocalPath,AdGroupName,SamAccountName,Status";
            ArrayList folderDataCollection;
            folderDataCollection = this.folderProps.ShowAccessGroupsOfParentOnly(domainPrefix.Text);

            foreach (string[] folderArray in folderDataCollection)
            {

                try
                {
                    ArrayList membersOfThisGroup = Domain.ShowMembers(folderArray[0]);

                    foreach (string member in membersOfThisGroup)
                    {
                        string userAccountStatus;

                        if (checkAccountStatus.IsChecked == true)
                        {
                            userAccountStatus = Domain.AccountStatus(member);
                        }
                        else
                        {
                            userAccountStatus = "";
                        }
                        

                        //accessData.Text += "\n" + folderArray[1] + " ; " + folderArray[0] + " ; " + member + " ; " + userAccountStatus;
                        //AccessData.Add(new AccessRecord(folderArray[1], folderArray[0], member, userAccountStatus));

                        AccessRecord Record = new AccessRecord(folderArray[1], folderArray[0], member, userAccountStatus);
                        AccessData.Add(Record);
                        accessData.Text += "\n" + Record.LocalPath + "," + Record.AdGroupName + "," + Record.SamAccountName + "," + Record.Status;

                    }

                    Domain.DisposeContext();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to query a domain.", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
            }
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

        private void checkAccountStatus_click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult dialogResult;

            if (checkAccountStatus.IsChecked == true)
            {
               dialogResult = MessageBox.Show("This will take more time to display results. Do you want to continue?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information);


                if (dialogResult == MessageBoxResult.No)
                {
                    checkAccountStatus.IsChecked = false;
                }
                else
                {
                    checkAccountStatus.IsChecked = true;
                }
            }
        }
    }
}
