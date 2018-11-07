using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using OpenNETCF.MTConnect;

namespace SampleClient
{
    public partial class MainForm
    {
        private int m_plotX = 0;
        private ChartArea m_ca;

        private void InitializePlot()
        {
            m_ca = dataPlot.ChartAreas.FindByName("ChartArea1");

            InitializeDataPlotDragDrop();
        }

        private void UpdateDataPlot()
        {
            m_plotX++;
            foreach (var series in dataPlot.Series)
            {
                var item = series.Tag as DataItem;
                if (item == null) continue;

                double dValue;
                var currentValue = m_client.GetDataItemById(item.ID).Value.ToString();
                if (double.TryParse(currentValue, out dValue))
                {
                    if (m_plotX >= 200)
                    {
                        series.Points.RemoveAt(0);
                    }
                    series.Points.AddXY(m_plotX, dValue);
                    
                    m_ca.RecalculateAxesScale();
                }
            }
        }
    }
}
