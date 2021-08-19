using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public MainWindow()
        {
            InitializeComponent();

            domain.Text = Domain.domain;
            domainPrefix.Text = Domain.domainPrefix;
            ContainerPath.Text = Domain.ContainerPath;

        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            accessData.Text = "LocalPath;AdGroupName;SamAccountName;Status";
            ArrayList folderDataCollection;
            folderDataCollection = this.folderProps.ShowAccessGroupsOfParentOnly(domainPrefix.Text);

            foreach (string[] folderArray in folderDataCollection)
            {

                try
                {
                    ArrayList membersOfThisGroup = Domain.ShowMembers(folderArray[0]);

                    foreach (string member in membersOfThisGroup)
                    {
                        
                        accessData.Text += "\n" + folderArray[1] + " ; " + folderArray[0] + " ; " + member + " ; " + Domain.AccountStatus(member);
                    }

                    Domain.DisposeContext();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to query a domain.");
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


        /*

        private void test_Click(object sender, RoutedEventArgs e)
        {

            var domainInfo = new ADactions().IdentifyDomain();

            MessageBox.Show
                (domainInfo["domain"] + "\n" +
                domainInfo["domainPrefix"] + "\n" +
                domainInfo["ContainerPath"],
                "Test", MessageBoxButton.OK, MessageBoxImage.Asterisk);

        }
        */

    }
}
