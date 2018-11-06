using System.Windows.Forms;
using OpenNETCF.MTConnect;
using System.IO;
using System.Text;
using System;

namespace SampleClient
{
    public partial class MainForm
    {
        private string m_dataFilePath;
        private StreamWriter m_dataFile;
        private object m_fileSyncRoot = new object();

        private void InitializeRecordData()
        { 
            m_dataFilePath = m_appSettings.Settings["RecordingFile"].Value;

            recordData.CheckedChanged += new System.EventHandler(recordData_CheckedChanged);
        }

        private String DataFilePath
        {
            get {
                return m_dataFilePath;
            }
        }

        private void WriteLineToFile(string fileLine)
        {
            lock (m_fileSyncRoot)
            {
                if (m_dataFile != null)
                {
                    m_dataFile.Write(fileLine);
                }
            }
        }

        void recordData_CheckedChanged(object sender, System.EventArgs e)
        {
            bool startRecording = recordData.Checked;

            lock (m_fileSyncRoot)
            {
                if (startRecording)
                {
                    if (File.Exists(DataFilePath))
                    {
                        try
                        {
                            File.Delete(DataFilePath);
                        }
                        catch
                        {
                            MessageBox.Show("Unable to delete the existing file.  Close it and try again");
                            recordData.Checked = false;
                            return;
                        }
                    }

                    m_dataFile = File.CreateText(DataFilePath);

                    // Ensure that everything is written to disk when writing to the stream, so we don't loose data.
                    m_dataFile.AutoFlush = true;

                    var headers = new StringBuilder();
                    for (int i = 0; i < dataList.Items.Count; i++)
                    {
                        var lvi = dataList.Items[i];
                        var columnName = ((DataItem)lvi.Tag).Name;
                        headers.AppendFormat("{0}{1}", columnName, i < (dataList.Items.Count - 1) ? "," : Environment.NewLine);
                    }

                    m_dataFile.Write(headers);
                }
                else
                {
                    if (m_dataFile != null)
                    {
                        m_dataFile.Close();
                        m_dataFile.Dispose();
                        m_dataFile = null;
                    }
                }
            }
        }
    }
}
