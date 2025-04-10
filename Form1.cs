using CefSharp;
using CefSharp.WinForms;
using System;
using System.IO;
using System.Windows.Forms;

namespace testCefsharp
{
    public partial class Form1 : Form
    {
        private ChromiumWebBrowser browser;
        private string rootCachePath;
        private string baseCachePath;

        public Form1()
        {
            InitializeComponent();
            FormClosing += Form1_FormClosing;

            InitializeChromium();
        }

        private void InitializeChromium()
        {
            baseCachePath = Path.Combine(Path.GetTempPath(), "Cefsharp", "ChromiumCacheRoot");

            CleanupOldCacheFolders();

            rootCachePath = Path.Combine(baseCachePath, $"Cef_Cache_{Guid.NewGuid()}");

            CefSettings settings = new CefSettings
            {
                BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CefSharp.BrowserSubprocess.exe"),
                RootCachePath = rootCachePath,
            };

            if (!File.Exists(settings.BrowserSubprocessPath))
            {
                MessageBox.Show($"Error: CefSharp.BrowserSubprocess.exe not found at {settings.BrowserSubprocessPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Initialize CefSharp if it's not already initialized
            if (!Cef.IsInitialized.HasValue || !Cef.IsInitialized.Value)
            {
                try
                {
                    Cef.Initialize(settings);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error initializing CefSharp: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Create and add the ChromiumWebBrowser control to the form
            try
            {
                browser = new ChromiumWebBrowser("https://www.google.com")
                {
                    Dock = DockStyle.Fill,
                };

                this.Controls.Add(browser);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing browser: {ex.Message}", "Browser Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to dynamically navigate to a new URL
        public void NavigateToUrl(string url)
        {
            if (browser != null && !string.IsNullOrEmpty(url))
            {
                browser.Load(url);
            }
            else
            {
                MessageBox.Show("Invalid URL or browser not initialized.", "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to cleanup old cache folders that are not in use
        private void CleanupOldCacheFolders()
        {
            if (!Directory.Exists(baseCachePath)) return;

            foreach (string dir in Directory.GetDirectories(baseCachePath))
            {
                try
                {
                    if (IsCacheFolderInUse(dir))
                    {
                        // Log that the folder is in use and not deleted
                        File.AppendAllText("log.txt", $"Cache folder {dir} is in use, skipping deletion at {DateTime.Now}" + Environment.NewLine);
                    }
                    else
                    {
                        // Folder is not in use, delete it
                        Directory.Delete(dir, true);
                        File.AppendAllText("log.txt", $"Cache directory {dir} deleted successfully at {DateTime.Now}" + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText("log.txt", $"Error checking/deleting cache folder {dir}: {ex.Message} at {DateTime.Now}" + Environment.NewLine);
                }
            }
        }

        // Check if the cache folder is in use by attempting to open a file inside it
        private bool IsCacheFolderInUse(string folderPath)
        {
            try
            {
                string[] files = Directory.GetFiles(folderPath);
                if (files.Length == 0) return false; // No files, not in use

                foreach (string file in files)
                {
                    // Try opening the file exclusively to see if it's locked
                    if (IsFileLocked(file))
                    {
                        return true; // The file is locked, so the folder is in use
                    }
                }

                return false; // No files are locked, the folder is not in use
            }
            catch (Exception)
            {
                return true;
            }
        }

        // Check if a file is locked (in use)
        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure Cef is properly shut down
            if (Cef.IsInitialized.HasValue && Cef.IsInitialized.Value)
            {
                try
                {
                    Cef.Shutdown();
                }
                catch (Exception ex)
                {
                    File.AppendAllText("log.txt", $"Error shutting down CefSharp: {ex.Message} at {DateTime.Now}" + Environment.NewLine);
                }
            }
        }
    }
}
