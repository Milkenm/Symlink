using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

using MessageBox = System.Windows.MessageBox;
namespace Symlink
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			comboBox_linkType.ItemsSource = Enum.GetValues(typeof(LinkType));
			comboBox_linkType.SelectedIndex = 0;
		}

		public enum LinkType
		{
			DirectoryJuntion,
			DirectoryLink,
			HardLink,
		}

		private void button_searchTarget_Click(object sender, RoutedEventArgs e)
		{
			if (ShowFolderDialog("Select the target path") is string targetPath)
			{
				textBox_target.Text = targetPath;
			}
		}

		private void button_searchDestination_Click(object sender, RoutedEventArgs e)
		{
			if (ShowFolderDialog("Select the destination path") is string destinationPath)
			{
				textBox_destination.Text = destinationPath;
			}
		}

		private string? ShowFolderDialog(string title)
		{
			using FolderBrowserDialog? dialog = new FolderBrowserDialog
			{
				Description = title,
				UseDescriptionForTitle = true,
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
				ShowNewFolderButton = true,
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				return dialog.SelectedPath;
			}
			return null;
		}

		private void button_link_Click(object sender, RoutedEventArgs e)
		{
			if (textBox_target.Text is not string target || textBox_destination.Text is not string destination || string.IsNullOrEmpty(target) || string.IsNullOrEmpty(destination))
			{
				MessageBox.Show("Select all the required paths.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			string arg = comboBox_linkType.SelectedItem switch
			{
				LinkType.DirectoryJuntion => "J",
				LinkType.DirectoryLink => "D",
				LinkType.HardLink => "H"
			};

			string folderName = target.Split(@"\").Last();
			string destinationPath = Path.Join(destination, folderName);
			using (Process process = new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = "cmd.exe",
					Arguments = $"/C MKLINK /{arg} \"{destinationPath}\" \"{target}\"\"",
					UseShellExecute = true,
					CreateNoWindow = true,
				}
			})
			{
				process.Start();
			}
		}
	}
}
