using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using VcdsDataPlotter.Gui.ViewModel;
using Serilog;
using Microsoft.Win32;

namespace VcdsDataPlotter.Gui.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        System.Windows.Forms.Application.Exit();
        WindowUtilities.ActivateWindow(this);
    }

    private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is MainWindowVM old)
        {
            old.OnCmdOpenVcdsCsvFile -= HandleCmdOpenVcdsCsvFile;
        }

        if (e.NewValue is MainWindowVM @new)
        {
            @new.OnCmdOpenVcdsCsvFile += HandleCmdOpenVcdsCsvFile;
        }
    }

    private void HandleCmdOpenVcdsCsvFile(object? sender, EventArgs e)
    {
        var localLogger = classLogger.ForMember();
        localLogger.Here().Verbose("Entering function");

        OpenFileDialog openFileDialog = new()
        {
#if NET8_0_OR_GREATER
            ClientGuid = OpenVcdsFileDialogGuid,
#endif
            Filter = "CSV files|*.csv",
            Multiselect = false,
            AddExtension = true,
            CheckFileExists = true,
            CheckPathExists = true,
            DefaultExt = ".csv",
            ReadOnlyChecked = true,
            ShowReadOnly = true
        };

        if (openFileDialog.ShowDialog() != true)
        {
            localLogger.Here().Information("User aborted opening file.");
            return;
        }

        localLogger.Here().Verbose("Selected file: {filePath}", openFileDialog.FileName);

        InternalLoadFile(openFileDialog.FileName);
    }

    private void InternalLoadFile(string filePath)
    {
        var localLogger = classLogger.ForMember();

        DocumentVM documentVM;
        try
        {
            documentVM = DocumentVM.LoadFile(filePath);
        }
        catch (Exception error)
        {
            localLogger.Error("Failed to open file {filePath}: {errorMessage}", filePath, error.Message);
            DisplayError($"Failed to open file {filePath}: {error.Message}");
            return;
        }

        // Create window for document
        DocumentWindow window = new DocumentWindow();
        window.Owner = this;
        window.DataContext = documentVM;
        window.Show();
    }

    private void DisplayError(string errorMessage)
    {
        MessageBox.Show(this, errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
    }

    private ILogger classLogger = Serilog.Log.Logger.ForClass(typeof(MainWindow));
    private Guid OpenVcdsFileDialogGuid = Guid.Parse("C0F5008D-1647-44A7-9063-7FC3B36EF923");
}

