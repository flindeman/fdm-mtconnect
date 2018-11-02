using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;

using OpenNETCF.MTConnect;

namespace SampleClient
{
    public partial class MainForm : Form
    {
        private EntityClient       m_client;
        private AppSettingsSection m_appSettings;


        public MainForm()
        {
            InitializeConfiguration();

            InitializeComponent();

            agentAddress.Text = m_appSettings.Settings["AgentAddress"].Value;
            recordData.Text = "Recording to " + m_appSettings.Settings["RecordingFile"].Value;

            InitializeAgentTree();

            InitializeDataList();
            
            InitializePlot();
            
            InitializeDataItemWatcher();
        }
    }
}
