using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DotNetPlugin.NativeBindings.SDK;
using DotNetPlugin.Properties;

namespace DotNetPlugin
{
    partial class Plugin
    {
        // Menu entry IDs for dynamic enable/disable
        private static int _startMenuId;
        private static int _stopMenuId;
        private static int _restartMenuId;
        private static int _statusMenuId;
        private static int _pluginHandle;

        protected override void SetupMenu(Menus menus)
        {
            // Store plugin handle for static access
            _pluginHandle = PluginHandle;
            
            // Set main plugin menu icon (PNG resource only)
            try
            {
                menus.Main.SetIcon(Resources.MainIcon);
            }
            catch
            {
                // Last resort: keep default without icon
            }

            // Build the menu and capture entry IDs
            var startItem = menus.Main.AddAndConfigureItem("&Start MCP Server", StartMCPServer);
            startItem.SetIcon(Resources.AboutIcon);
            _startMenuId = startItem.Id;

            var stopItem = startItem.Parent.AddAndConfigureItem("S&top MCP Server", StopMCPServer);
            stopItem.SetIcon(Resources.AboutIcon);
            _stopMenuId = stopItem.Id;

            var restartItem = stopItem.Parent.AddAndConfigureItem("&Restart MCP Server", RestartMCPServer);
            restartItem.SetIcon(Resources.AboutIcon);
            _restartMenuId = restartItem.Id;

            restartItem.Parent.AddSeparator();

            // Status indicator (use a no-op handler)
            var statusItem = restartItem.Parent.AddAndConfigureItem("Status: Stopped", OnStatusClick);
            _statusMenuId = statusItem.Id;

            statusItem.Parent.AddSeparator();

            statusItem.Parent
                .AddAndConfigureItem("&Configure MCP Server...", ConfigureMCPServer).SetIcon(Resources.AboutIcon).Parent
                .AddSeparator()
                .AddAndConfigureItem("&About...", OnAboutMenuItem).SetIcon(Resources.AboutIcon);

            // Set up the delegate for updating menu state from commands
            Plugin.UpdateMcpMenuState = UpdateMenuState;

            // Initialize menu state
            UpdateMenuState();
        }

        /// <summary>
        /// No-op handler for status menu item click.
        /// </summary>
        private static void OnStatusClick(MenuItem menuItem)
        {
            // Status item is informational only, no action needed
        }

        /// <summary>
        /// Updates menu items based on current server state.
        /// </summary>
        private static void UpdateMenuState()
        {
            bool isRunning = Plugin.IsMcpServerRunning();

            try
            {
                // Start: visible only when stopped
                Plugins._plugin_menuentrysetvisible(_pluginHandle, _startMenuId, !isRunning);
                
                // Stop: visible only when running
                Plugins._plugin_menuentrysetvisible(_pluginHandle, _stopMenuId, isRunning);
                
                // Restart: visible only when running
                Plugins._plugin_menuentrysetvisible(_pluginHandle, _restartMenuId, isRunning);
                
                // Status text
                string statusText = isRunning ? "Status: Running ✓" : "Status: Stopped";
                Plugins._plugin_menuentrysetname(_pluginHandle, _statusMenuId, statusText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateMenuState] Error: {ex.Message}");
            }
        }

        public void OnAboutMenuItem(MenuItem menuItem)
        {
            MessageBox.Show(HostWindow, "x64DbgMCPServer Plugin For x64dbg\nCoded By AgentSmithers", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void OnDumpMenuItem(MenuItem menuItem)
        {
            if (!Bridge.DbgIsDebugging())
            {
                Console.WriteLine("You need to be debugging to use this Command");
                return;
            }
            Bridge.DbgCmdExec("DotNetDumpProcess");
        }

        public static void ExecuteCustomCommand(MenuItem menuItem)
        {
            if (!Bridge.DbgIsDebugging())
            {
                Console.WriteLine("You need to be debugging to use this Command");
                return;
            }
            Bridge.DbgCmdExec("DumpModuleToFile");
        }

        public static void StartMCPServer(MenuItem menuItem)
        {
            Bridge.DbgCmdExec("StartMCPServer");
        }

        public static void StopMCPServer(MenuItem menuItem)
        {
            Bridge.DbgCmdExec("StopMCPServer");
        }

        public static void RestartMCPServer(MenuItem menuItem)
        {
            Bridge.DbgCmdExec("RestartMCPServer");
        }

        public static void ConfigureMCPServer(MenuItem menuItem)
        {
            // Get current configuration
            var config = Plugin.GetMcpServerConfig();

            // Show dialog on STA thread (required for Windows Forms)
            var t = new Thread(() =>
            {
                using (var dialog = new McpConfigDialog(config))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        // Update configuration (in-memory only)
                        config.IpAddress = dialog.IpAddress;
                        config.Port = dialog.Port;
                        Plugin.SetMcpServerConfig(config);

                        MessageBox.Show(
                            $"Configuration updated.\n\nNew URL: {config.GetDisplayUrl()}\n\nRestart the MCP server for changes to take effect.\n\nNote: Settings will reset when x64dbg closes.",
                            "MCP Server Configuration",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }
    }
}
