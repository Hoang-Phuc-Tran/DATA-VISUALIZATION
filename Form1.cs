/*
 * Project:	    ASSIGNMENT 01 – DATA VISUALIZATION
 * Author:	    Hoang Phuc Tran
 * Student ID:  8789102
 * Date:		2024-01-23
 * Description:  The project is a C#-developed Windows Forms application. 
 * It has a form that shows a Pareto chart, a kind of graphic that is frequently used in statistical 
 * analysis to show the variables in a dataset that are most important. In this instance, the graphic 
 * is used to examine and illustrate the variables causing software project delays.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private BindingList<Data> dataList = new BindingList<Data>();

        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeChart();
            UpdateChart();
        }

        /// <summary>
        /// sets up the DataGridView control so that it may display data. 
        /// Column headers are configured and the DataGridView's data source is established using this technique.
        /// </summary>
        private void InitializeDataGridView()
        {
            // Automatically generate the DataGridView columns from the Data class properties
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = dataList;

            // The public attributes in the Data class serve as the basis for the column names.
            // Once the DataSource has been established, we should verify that the columns are present.
            if (dataGridView1.Columns.Contains("type"))
            {
                dataGridView1.Columns["type"].HeaderText = "Reason";
            }
            if (dataGridView1.Columns.Contains("value"))
            {
                dataGridView1.Columns["value"].HeaderText = "Incidence";
            }

            // Add some example data
            dataList.Add(new Data { type = "Requirements Changes", value = 50 });
            dataList.Add(new Data { type = "Resource Turnover", value = 30 });
            dataList.Add(new Data { type = "Technical Debt", value = 66 });
            dataList.Add(new Data { type = "Inadequate Testing", value = 80 });
            dataList.Add(new Data { type = "Scope Creep", value = 70 });
            // ... Add more as needed
            // Sort the dataList here
            SortDataList();
        }

        /// <summary>
        /// uses the 'value' attribute to sort the dataList in decreasing order.
        /// By rearranging its members, this method directly affects the dataList.
        /// </summary>
        /// <remarks>
        /// This method should be called after any changes to dataList to ensure data is sorted for chart display.
        /// </remarks>
        private void SortDataList()
        {
            // Temporary list to sort the data
            var sorted = dataList.OrderByDescending(d => d.value).ToList();
            dataList.Clear();
            foreach (var item in sorted)
            {
                dataList.Add(item);
            }
        }

        /// <summary>
        /// sets up the required series and chart area configurations to initialize the chart control.
        /// The chart that shows the frequency and cumulative percentage data is set up using this way.
        /// </summary>
        /// <remarks>
        /// The chart is configured with two series - one for frequency (column chart) and one for cumulative percentage (line chart).
        /// </remarks>
        private void InitializeChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
            // Add the series for Frequency which will be the column chart
            var series1 = new Series
            {
                Name = "Frequency",
                ChartType = SeriesChartType.Column,
                XValueMember = "type",
                YValueMembers = "value"
            };
            chart1.Series.Add(series1);

            // Add the series for Cumulative Percentage which will be the line chart
            var series2 = new Series
            {
                Name = "CumulativePercentage",
                ChartType = SeriesChartType.Line,
                YAxisType = AxisType.Secondary
            };
            chart1.Series.Add(series2);

            chart1.Titles.Clear();
            chart1.Titles.Add("Software Project Delay Factors");
            // Bind the chart to the dataList
            chart1.DataSource = dataList;

            // This will ensure that the Y axis for the CumulativePercentage series starts from 0
            chart1.ChartAreas[0].AxisY2.Minimum = 0;
        }

        /// <summary>
        /// Event handler for the click event of button1. 
        /// When invoked, it updates the chart to reflect the current data.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains no event data.</param>
        /// <remarks>
        /// This method should be connected to the Click event of button1 to refresh the chart whenever the button is clicked.
        /// </remarks>
        private void button1_Click(object sender, EventArgs e)
        {
            UpdateChart();
        }


        /// <summary>
        /// This method refreshes the Pareto chart, sorting the data by value and displaying it as a bar graph, 
        /// with a line graph for the cumulative percentage. It aligns the most significant values first and uses
        /// two Y-axes to show frequencies and percentages up to 100%, creating a clear, visual understanding of the
        /// data.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the chart series are not properly configured prior to calling this method.
        /// </exception>
        /// <remarks>
        /// This method should be called after the initialization of the data grid and chart,
        /// and whenever the underlying data changes and the chart needs to be refreshed.
        /// </remarks>
        private void UpdateChart()
        {
            // Sort the data by value in descending order
            var sortedData = dataList.OrderByDescending(d => d.value).ToList();

            // Clear the existing series data
            chart1.Series["Frequency"].Points.Clear();
            chart1.Series["CumulativePercentage"].Points.Clear();

            double total = sortedData.Sum(d => d.value);
            double cumulative = 0;

            // Add points for the sorted data to the frequency series (column chart)
            foreach (var data in sortedData)
            {
                chart1.Series["Frequency"].Points.AddXY(data.type, data.value);
            }

            // Add points for the cumulative percentage to the line series
            foreach (var data in sortedData)
            {
                cumulative += data.value;
                // Use the same X value for alignment with the column chart
                double cumulativePercent = (cumulative / total) * 100; // Ensure this is a percentage
                chart1.Series["CumulativePercentage"].Points.AddXY(data.type, cumulativePercent);
            }

            // Set the primary Y-axis title to "Values"
            chart1.ChartAreas[0].AxisY.Title = "Values";

            // Adjust the secondary Y-axis to show percentages and set max to 100%
            chart1.ChartAreas[0].AxisY2.LabelStyle.Format = "{0}%"; // Format as percentage
            chart1.ChartAreas[0].AxisY2.Title = "Cum %";
            chart1.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            chart1.ChartAreas[0].AxisY2.Maximum = 100; // Ensure max is 100%
            chart1.ChartAreas[0].AxisY2.Interval = 20; // Set intervals at every 20%

            // Refresh the chart to show the updates
            chart1.Invalidate();
        }

       
    }


}
