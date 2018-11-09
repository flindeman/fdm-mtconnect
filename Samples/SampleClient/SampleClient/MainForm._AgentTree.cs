using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

using OpenNETCF.MTConnect;

namespace SampleClient
{
    public partial class MainForm : Form
    {
        private void InitializeConfiguration()
        {
            m_appSettings = new AppSettingsSection();

            foreach (String strKey in ConfigurationManager.AppSettings)
            {
                m_appSettings.Settings.Add(strKey, ConfigurationManager.AppSettings[strKey]);
            }

            String localAppdataPath = Environment.GetEnvironmentVariable("LocalAppData");

            String configFilePath = @"Stratasys\MTConnect Sample Client";

            String configFileName = "MTConnectSampleClient.config";

            String configFilePathToUse = Path.Combine(localAppdataPath, configFilePath);

            String configFileToUse = Path.Combine(configFilePathToUse, configFileName);

            //
            // One time copy in place.
            //

            // Create folder.
            if (!Directory.Exists(configFilePathToUse))
            {
                Directory.CreateDirectory(configFilePathToUse);
            }

            // If it's not already there,  opy the file from the deployment location to the folder.

            String sourceFilePath = Path.Combine(System.Windows.Forms.Application.StartupPath, configFileName);

            if (!File.Exists(configFileToUse))
            {
                File.Copy(sourceFilePath, configFileToUse);
            }


            //
            // Basic merging of app settings.
            //

            ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();

            exeConfigurationFileMap.ExeConfigFilename = configFileToUse;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);

            AppSettingsSection appSettings = config.AppSettings;

            foreach (String strKey in appSettings.Settings.AllKeys)
            {
                String strValue = appSettings.Settings[strKey].Value;

                if (m_appSettings.Settings[strKey] != null)
                {
                    m_appSettings.Settings[strKey].Value = strValue;
                }
                else
                {
                    m_appSettings.Settings.Add(strKey, strValue);
                }
            }

            String expandedPath = Environment.ExpandEnvironmentVariables(m_appSettings.Settings["RecordingFilePath"].Value);

            m_appSettings.Settings.Add("RecordingFile", Path.Combine(expandedPath, "mtc_data.csv"));

        }

        private void InitializeAgentTree()
        {
            agentTree.AfterSelect += new TreeViewEventHandler(agentTree_AfterSelect);

            InitializeAgentTreeDragDrop();
        }

        void agentTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PopulatePropertyList(e.Node.Tag);
        }

        private void PopulateAgentTree(DeviceCollection devices)
        {
            agentTree.Nodes.Clear();

            // create a root node
            // [FRL] if something does go wrong there could be no AgentInformation.
            //       E.g. no port or invalid IP.
            var root = new TreeNode(devices.AgentInformation.Name);
            root.Tag = devices.AgentInformation;

            // fill the node with devices
            foreach (var device in devices)
            {
                //var deviceNode = new TreeNode(string.Format("{0} [id:{1}]", device.Name, device.ID));
                var deviceNode = new TreeNode(device.Name);
                deviceNode.Tag = device;

                foreach (var component in device.Components)
                {
                    FillComponents(component, deviceNode);
                }

                foreach (var dataItem in device.DataItems)
                {
                    string displayName = dataItem.ID;
                    if (!string.IsNullOrEmpty(dataItem.Name)) displayName += string.Format(" ({0})", dataItem.Name);
                    var dataNode = new TreeNode(displayName);
                    dataNode.Tag = dataItem;
                    deviceNode.Nodes.Add(dataNode);
                }

                root.Nodes.Add(deviceNode);
            }

            root.ExpandAll();

            // put the root node into the tree
            agentTree.Nodes.Add(root);

            // cache the device collection
            agentTree.Tag = devices;
        }

        private void FillComponents(OpenNETCF.MTConnect.Component component, TreeNode parent)
        {
            //var componentNode = new TreeNode(string.Format("{0} [id:{1}]", component.Name, component.ID));
            var componentNode = new TreeNode(component.Name);
            componentNode.Tag = component;

            foreach (var subcomponent in component.Components)
            {
                FillComponents(subcomponent, componentNode);
            }

            foreach (var dataItem in component.DataItems)
            {
                string displayName = dataItem.ID;
                if (!string.IsNullOrEmpty(dataItem.Name)) displayName += string.Format(" ({0})", dataItem.Name);
                var dataNode = new TreeNode(displayName);
                dataNode.Tag = dataItem;
                componentNode.Nodes.Add(dataNode);
            }

            parent.Nodes.Add(componentNode);
        }
    }
}
