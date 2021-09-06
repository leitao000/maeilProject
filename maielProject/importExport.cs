using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace maielProject
{
    public partial class importExport : Form
    {
        private Lista<Row> booking = new Lista<Row>();
        private List<RowInfo> rowInfos = new List<RowInfo>();
        private String[] headers = new String[17];
        public importExport()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                this.Enabled = false;

                var workbook = new XLWorkbook("bookings.xlsx");
                var worksheet = workbook.Worksheet(1);
                var rowsWithData = worksheet.RangeUsed();

                booking.Clear();
                rowInfos.Clear();


                progressBarLoadExcel.Maximum = rowsWithData.RowsUsed().Count();
                progressBarLoadExcel.Value = 0;

                foreach (var row in rowsWithData.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                    {
                        for (int i = 1; i <= headers.Length; i++)
                        {
                            headers[i - 1] = row.Cell(i).GetString();
                        }
                        continue;
                    }

                    Row reg = new Row();

                    reg.Reference = row.Cell(1).GetString();
                    reg.Client = row.Cell(2).GetString();
                    reg.State = row.Cell(3).GetString();
                    reg.Type = row.Cell(4).GetString();
                    reg.Matriculation = row.Cell(5).GetString();
                    reg.TypeCargo = row.Cell(6).GetString();
                    reg.Priority = row.Cell(7).GetString() == "" ? new DateTime() : Convert.ToDateTime(row.Cell(7).GetString());
                    reg.RegistryDate = row.Cell(8).GetString() == "" ? new DateTime() : Convert.ToDateTime(row.Cell(8).GetString());
                    reg.BlokedTime = row.Cell(9).GetString() == "" ? new DateTime() : Convert.ToDateTime(row.Cell(9).GetString());
                    reg.POD = row.Cell(10).GetString();
                    reg.Park = row.Cell(11).GetString();
                    reg.KindEquipment = row.Cell(12).GetString();
                    reg.DepotIdBlocking = int.Parse(row.Cell(13).Value.ToString());
                    reg.ExpiredAssignmentDate = row.Cell(14).GetString() == "" ? new DateTime() : Convert.ToDateTime(row.Cell(14).GetString());
                    reg.Vessel = row.Cell(15).GetString();
                    reg.Voyage = row.Cell(16).GetString();
                    reg.POL = row.Cell(17).GetString();

                    booking.Add(reg);
                    rowInfos.Add(
                        new RowInfo()
                        {
                            Row = reg
                        });
                    progressBarLoadExcel.Value += 1;
                }

                var bi = new BindingList<Row>(booking);

                dataGridViewBooking.DataSource = bi;

                dataGridViewBooking.Columns["Guid"].Visible = false;

                progressBarLoadExcel.Visible = false;

                this.Enabled = true;

                btnSave.Enabled = true;

                MessageBox.Show("File imported successfully");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: "+ ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;

                var workbook = new XLWorkbook("bookings.xlsx");
                var worksheet = workbook.Worksheet(1);

                progressBarLoadExcel.Maximum = rowInfos.Count() * 2;
                progressBarLoadExcel.Value = 0;
                progressBarLoadExcel.Visible = true;

                foreach (RowInfo rowInfo in rowInfos)
                {
                    progressBarLoadExcel.Value += 1;

                    if (rowInfo.State == State.delete || rowInfo.State == State.none)
                        continue;

                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(1).Value = rowInfo.Row.Reference;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(2).Value = rowInfo.Row.Client;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(3).Value = rowInfo.Row.State;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(4).Value = rowInfo.Row.Type;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(5).Value = rowInfo.Row.Matriculation;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(6).Value = rowInfo.Row.TypeCargo;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(7).Value = rowInfo.Row.Priority == DateTime.MinValue ? "" : rowInfo.Row.Priority.ToString();
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(8).Value = rowInfo.Row.RegistryDate == DateTime.MinValue ? "" : rowInfo.Row.RegistryDate.ToString();
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(9).Value = rowInfo.Row.BlokedTime == DateTime.MinValue ? "" : rowInfo.Row.BlokedTime.ToString();
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(10).Value = rowInfo.Row.POD;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(11).Value = rowInfo.Row.Park;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(12).Value = rowInfo.Row.KindEquipment;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(13).Value = rowInfo.Row.DepotIdBlocking;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(14).Value = rowInfo.Row.ExpiredAssignmentDate == DateTime.MinValue ? "" : rowInfo.Row.ExpiredAssignmentDate.ToString();
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(15).Value = rowInfo.Row.Vessel;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(16).Value = rowInfo.Row.Voyage;
                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2).Cell(17).Value = rowInfo.Row.POL;
                }

                int count = 0;

                foreach (RowInfo rowInfo in rowInfos)
                {
                    progressBarLoadExcel.Value += 1;

                    if (rowInfo.State == State.update || rowInfo.State == State.add || rowInfo.State == State.none)
                        continue;

                    worksheet.Row(rowInfos.IndexOf(rowInfo) + 2 - count).Delete();

                    count++;
                }

                workbook.Save();

                progressBarLoadExcel.Visible = false;

                this.Enabled = true;

                MessageBox.Show("File saved successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void dataGridViewBooking_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Console.WriteLine("*********Log***********");
            Console.WriteLine("User Deleted object with reference:" + e.Row.Cells[1].Value.ToString());

            foreach (RowInfo rowInfo in rowInfos)
            {
                if (rowInfo.Row.Equals(booking.FindByIndex(e.Row.Index)))
                {
                    rowInfo.State = State.delete;
                    break;
                }
            }
        }

        private void dataGridViewBooking_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            Console.WriteLine("*********Log***********");
            Console.WriteLine("User Add new object");

            rowInfos.Add(new RowInfo()
            {
                Row = booking.FindByIndex(e.Row.Index-1),
                State = State.add
            });
        }

        private void dataGridViewBooking_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("*********Log***********");
            string value = dataGridViewBooking.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null ? "" : dataGridViewBooking.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            if(value == "")
                Console.WriteLine("User update object in cell (" + (e.RowIndex + 1) + ", " + e.ColumnIndex + ") Column (" + headers[e.ColumnIndex - 1] + ") with no value");
            else
                Console.WriteLine("User update object in cell (" + (e.RowIndex + 1) + ", " + e.ColumnIndex + ") Column (" + headers[e.ColumnIndex-1] + ") with value " + value);

            foreach (RowInfo rowInfo in rowInfos)
            {
                if (rowInfo.Row.Equals(booking.FindByIndex(e.RowIndex)))
                {
                    if(rowInfo.State == State.add)
                    {
                        rowInfo.Row = booking.FindByIndex(e.RowIndex);
                        break;
                    }
                    rowInfo.State = State.update;
                    break;
                }
            }
        }
    }
}
