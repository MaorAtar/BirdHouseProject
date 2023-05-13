﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BirdHouseProject.Views
{
    /// <summary>
    /// Represents a Windows Forms view for managing lady gouldian finch data.
    /// Implements the ILadyGouldianFinchView interface.
    /// </summary>
    public partial class LadyGouldianFinchView : Form, ILadyGouldianFinchView
    {
        // Fields
        SqlConnection connectionString = new SqlConnection(@"Data Source=MAOR-ATAR-LAPTO;Initial Catalog=BirdHouseProjectDb;
Integrated Security=True;");
        private string message;
        private bool isSuccessful;
        private bool isEdit;

        /// <summary>
        /// Initializes a new instance of the LadyGouldianFinchView class.
        /// </summary>
        public LadyGouldianFinchView()
        {
            InitializeComponent();
            AssociateAndRaiseViewEvents();
            tabControl1.TabPages.Remove(tabPage2);
            closeBtn.Click += delegate { this.Close(); };
        }

        // Properties
        public string LadyGouldianFinchSerialNumber { get =>serialBox.Text; set => serialBox.Text = value; }
        public string LadyGouldianFinchSpecies { get => speciesComboBox.Text.ToString(); set => speciesComboBox.Text = value; }
        public string LadyGouldianFinchSubSpecies { get => subSpeciesComboBox.Text.ToString(); set => subSpeciesComboBox.Text = value; }
        public string LadyGouldianFinchHatchDate { get => hatchDateBox.Text; set => hatchDateBox.Text = value; }
        public string LadyGouldianFinchGender { get => genderComboBox.Text.ToString(); set => genderComboBox.Text = value; }
        public string LadyGouldianFinchCageNumber { get => cageNumberBox.Text; set => cageNumberBox.Text = value; }
        public string LadyGouldianFinchFSerialNumber { get => fSerialBox.Text; set => fSerialBox.Text = value; }
        public string LadyGouldianFinchMSerialNumber { get => mSerialBox.Text; set => mSerialBox.Text = value; }
        public string LadyGouldianFinchHeadColor { get => headColorComboBox.Text.ToString(); set => headColorComboBox.Text = value; }
        public string LadyGouldianFinchBreastColor { get => breastColorComboBox.Text.ToString(); set => breastColorComboBox.Text = value; }
        public string LadyGouldianFinchBodyColor { get => bodyColorComboBox.Text.ToString(); set => bodyColorComboBox.Text = value; }
        public string SearchValue { get => searchBox.Text; set => searchBox.Text = value; }
        public bool IsEdit { get => isEdit; set => isEdit = value; }
        public bool IsSuccessful { get => isSuccessful; set => isSuccessful = value; }
        public string Message { get => message; set => message = value; }

        // Events
        public event EventHandler SearchEvent;
        public event EventHandler AddNewEvent;
        public event EventHandler EditEvent;
        public event EventHandler DeleteEvent;
        public event EventHandler SaveEvent;
        public event EventHandler CancelEvent;

        /// <summary>
        /// Associates and raises events for the view.
        /// </summary>
        public void AssociateAndRaiseViewEvents()
        {
            // Search
            searchBtn.Click += delegate { SearchEvent?.Invoke(this, EventArgs.Empty); };
            searchBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    SearchEvent?.Invoke(this, EventArgs.Empty);
                    RefreshBirdList();
            };
            // Add New
            addnewBtn.Click += delegate
            {
                AddNewEvent?.Invoke(this, EventArgs.Empty);
                RefreshBirdList();
                tabControl1.TabPages.Remove(tabPage1);
                tabControl1.TabPages.Add(tabPage2);
                tabPage2.Text = "Add new bird";
            };
            // Edit
            editBtn.Click += delegate
            {
                EditEvent?.Invoke(this, EventArgs.Empty);
                RefreshBirdList();
                tabControl1.TabPages.Remove(tabPage1);
                tabControl1.TabPages.Add(tabPage2);
                tabPage2.Text = "Edit bird";
            };
            // Save changes
            saveBtn.Click += delegate
            {
                // User input validation
                // Species Validation
                if (speciesComboBox.SelectedIndex == -1)
                {
                    speciesErrorProvider.SetError(speciesComboBox, "Invalid Species");
                    return;
                }
                else
                {
                    speciesErrorProvider.SetError(speciesComboBox, string.Empty);
                }
                // Sub Species Validation
                if (subSpeciesComboBox.SelectedIndex == -1)
                {
                    subSpeciesErrorProvider.SetError(subSpeciesComboBox, "Invalid Sub Species");
                    return;
                }
                else
                {
                    subSpeciesErrorProvider.SetError(subSpeciesComboBox, string.Empty);
                }
                // Hatch Date Validation
                if (!IsDateFormatValid(hatchDateBox.Text))
                {
                    hatchDateErrorProvider.SetError(hatchDateBox, "Enter Date in DD/MM/YYYY Format");
                    return;
                }
                else
                {
                    hatchDateErrorProvider.SetError(hatchDateBox, string.Empty);
                }
                // Head Color Validation
                if (headColorComboBox.SelectedIndex == -1)
                {
                    headColorErrorProvider.SetError(headColorComboBox, "Invalid Head Color");
                    return;
                }
                else
                {
                    headColorErrorProvider.SetError(headColorComboBox, string.Empty);
                }
                // Breast Color Validation
                if (breastColorComboBox.SelectedIndex == -1)
                {
                    breastColorErrorProvider.SetError(breastColorComboBox, "Invalid Breast Color");
                    return;
                }
                else
                {
                    breastColorErrorProvider.SetError(breastColorComboBox, string.Empty);
                }
                // Body Color Validation
                if (bodyColorComboBox.SelectedIndex == -1)
                {
                    bodyColorErrorProvider.SetError(bodyColorComboBox, "Invalid Body Color");
                    return;
                }
                else
                {
                    bodyColorErrorProvider.SetError(bodyColorComboBox, string.Empty);
                }
                // Gender Validation
                if (genderComboBox.SelectedIndex == -1)
                {
                    genderErrorProvider.SetError(genderComboBox, "Invalid Gender");
                    return;
                }
                else
                {
                    genderErrorProvider.SetError(genderComboBox, string.Empty);
                }
                // Cage Number Validation
                if (cageNumberBox.Text.Length != 6)
                {
                    cageNumberErrorProvider.SetError(cageNumberBox, "Cage Number must be 6 digits long");
                    return;
                }
                else if (!IsCageNumberInDB(cageNumberBox.Text))
                {
                    cageNumberErrorProvider.SetError(cageNumberBox, "Cage Number does not exist in the system");
                    return;
                }
                else
                {
                    cageNumberErrorProvider.SetError(cageNumberBox, string.Empty);
                }
                // Father Serial Number Validation
                if (fSerialBox.Text.Length != 6)
                {
                    fsnErrorProvider.SetError(fSerialBox, "Father Serial Number must be 6 digits long");
                    return;
                }
                else if (!IsFSerialNumberInDB(fSerialBox.Text))
                {
                    fsnErrorProvider.SetError(fSerialBox, "Father Serial Number does not exist in the system");
                    return;
                }
                else if (!IsFSerialNumberValid(fSerialBox.Text))
                {
                    fsnErrorProvider.SetError(fSerialBox, "Father Serial Number does not match to a Male bird Serial Number");
                    return;
                }
                else if (string.Equals(fSerialBox.Text, mSerialBox.Text))
                {
                    fsnErrorProvider.SetError(fSerialBox, "Father Serial Number and Mother Serial Number can't be the same");
                    return;
                }
                else
                {
                    fsnErrorProvider.SetError(fSerialBox, string.Empty);
                }
                // Mother Serial Number Validation
                if (mSerialBox.Text.Length != 6)
                {
                    msnErrorProvider.SetError(mSerialBox, "Mother Serial Number must be 6 digits long");
                    return;
                }
                else if (!IsMSerialNumberInDB(mSerialBox.Text))
                {
                    msnErrorProvider.SetError(mSerialBox, "Mother Serial Number does not exist in the system");
                    return;
                }
                else if (!IsMSerialNumberValid(mSerialBox.Text))
                {
                    msnErrorProvider.SetError(mSerialBox, "Mother Serial Number does not match to a Female bird Serial Number");
                    return;
                }
                else
                {
                    msnErrorProvider.SetError(mSerialBox, string.Empty);
                }
                SaveEvent?.Invoke(this, EventArgs.Empty);
                RefreshBirdList();
                if (isSuccessful)
                {
                    tabControl1.TabPages.Remove(tabPage2);
                    tabControl1.TabPages.Add(tabPage1);
                }
                MessageBox.Show(Message);
            };
            // Cancel
            cancelBtn.Click += delegate
            {
                CancelEvent?.Invoke(this, EventArgs.Empty);
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Add(tabPage1);
            };
            // Delete
            deleteBtn.Click += delegate
            {
                var result = MessageBox.Show("Are you sure you want to delete the selected bird?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    DeleteEvent?.Invoke(this, EventArgs.Empty);
                    RefreshBirdList();
                    MessageBox.Show(Message);
                }
            };
        }

        /// <summary>
        /// Sets the binding source for the Lady Gouldian Finch data.
        /// </summary>
        /// <param name="ladyGouldianFinchList">The binding source containing Lady Gouldian Finch data.</param>
        public void SetLadyGouldianFinchBindingSource(BindingSource ladyGouldianFinchList)
        {
            dataGridView1.DataSource = ladyGouldianFinchList;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Singleton pattern (open a single form instance)
        private static LadyGouldianFinchView instance;

        /// <summary>
        /// Retrieves the singleton instance of the CageView form.
        /// </summary>
        /// <param name="parentContainer">The parent container form.</param>
        /// <returns>The singleton instance of the CageView form.</returns>
        public static LadyGouldianFinchView GetInstance(Form parentContainer)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new LadyGouldianFinchView();
                instance.MdiParent = parentContainer;
                instance.FormBorderStyle = FormBorderStyle.None;
                instance.Dock = DockStyle.Fill;
            }
            else
            {
                if (instance.WindowState == FormWindowState.Minimized)
                    instance.WindowState = FormWindowState.Normal;
                instance.BringToFront();
            }
            return instance;
        }

        /// <summary>
        /// Handles the cell content click event in the DataGridView1 control.
        /// Retrieves the selected bird data and opens a LadyGouldianFinchDataView form to display the details.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">A DataGridViewCellEventArgs that contains the event data.</param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Get the selected bird data
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            int serialNumber = Convert.ToInt32(row.Cells[0].Value);
            string species = row.Cells[1].Value.ToString();
            string subSpecies = row.Cells[2].Value.ToString();
            string hatchDate = row.Cells[3].Value.ToString();
            string gender = row.Cells[4].Value.ToString();
            string cageNumber = row.Cells[5].Value.ToString();
            int fSerialNumber = Convert.ToInt32(row.Cells[6].Value);
            int mSerialNumber = Convert.ToInt32(row.Cells[7].Value);
            string headColor = row.Cells[8].Value.ToString();
            string breastColor = row.Cells[9].Value.ToString();
            string bodyColor = row.Cells[10].Value.ToString();

            // Create a new instance of the details form and pass the selected bird data
            LadyGouldianFinchDataView ladyGouldianFinchDataView = new LadyGouldianFinchDataView(
                serialNumber, species, subSpecies, hatchDate, gender, cageNumber, fSerialNumber, mSerialNumber, headColor, breastColor, bodyColor);

            // Show the details form
            ladyGouldianFinchDataView.Show();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the speciesComboBox control.
        /// Occurs when the selected index of the speciesComboBox is changed.
        /// Updates the subSpeciesComboBox items based on the selected species.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void speciesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            subSpeciesComboBox.Items.Clear();

            if (speciesComboBox.SelectedItem.ToString() == "Gouldian American")
            {
                subSpeciesComboBox.Items.Add("North America");
                subSpeciesComboBox.Items.Add("Central America");
                subSpeciesComboBox.Items.Add("South America");
            }
            else if (speciesComboBox.SelectedItem.ToString() == "Gouldian European")
            {
                subSpeciesComboBox.Items.Add("East Europe");
                subSpeciesComboBox.Items.Add("Western Europe");
            }
            else if (speciesComboBox.SelectedItem.ToString() == "Gouldian Australian")
            {
                subSpeciesComboBox.Items.Add("Central Australia");
                subSpeciesComboBox.Items.Add("Coastal cities");
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the genderComboBox control.
        /// Occurs when the selected index of the genderComboBox is changed.
        /// Updates the bodyColorComboBox items based on the selected gender.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void genderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            bodyColorComboBox.Items.Clear();

            if (genderComboBox.SelectedItem.ToString() == "Female")
            {
                bodyColorComboBox.Items.Add("Green");
                bodyColorComboBox.Items.Add("Yellow");
                bodyColorComboBox.Items.Add("Blue");
                bodyColorComboBox.Items.Add("Silver");
            }
            else if (genderComboBox.SelectedItem.ToString() == "Male")
            {
                bodyColorComboBox.Items.Add("Green");
                bodyColorComboBox.Items.Add("Yellow");
                bodyColorComboBox.Items.Add("Blue");
                bodyColorComboBox.Items.Add("Silver");
            }
        }

        /// <summary>
        /// Refreshes the bird list displayed in the dataGridView1 control.
        /// Retrieves the latest data from the database table "LadyGouldianFinch"
        /// and updates the dataGridView1 with the retrieved data.
        /// </summary>
        private void RefreshBirdList()
        {
            string query = "Select *from LadyGouldianFinch order by Serial_number desc";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connectionString);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "LadyGouldianFinch");
            dataGridView1.DataSource = dataSet.Tables[0];
        }

        /// <summary>
        /// Checks if the given string is in a correct date format.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>boolean</returns>
        private static bool IsDateFormatValid(string input)
        {
            DateTime parsedDate;
            return DateTime.TryParseExact(input, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate);
        }

        /// <summary>
        /// Checks if a given cage number is in the Cage SQL table.
        /// </summary>
        /// <param name="cage_number"></param>
        /// <returns>boolean</returns>
        private bool IsCageNumberInDB(string cage_number)
        {
            bool flag = false;
            string connection = "Data Source=MAOR-ATAR-LAPTO;Initial Catalog=BirdHouseProjectDb;Integrated Security=True;";
            string query = "SELECT Serial_Number FROM Cage WHERE Serial_Number = @cage_number";

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@cage_number", cage_number);
                    sqlConnection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        flag = true;
                    }
                    reader.Close();
                }
                sqlConnection.Close();
            }
            return flag;
        }

        /// <summary>
        /// Checks if a given father serial number is in the LadyGouldianFinch SQL table.
        /// </summary>
        /// <param name="f_serial_number"></param>
        /// <returns></returns>
        private bool IsFSerialNumberInDB(string f_serial_number)
        {
            bool flag = false;
            string connection = "Data Source=MAOR-ATAR-LAPTO;Initial Catalog=BirdHouseProjectDb;Integrated Security=True;";
            string query = "SELECT Serial_Number FROM LadyGouldianFinch WHERE Serial_Number = @f_serial_number";

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@f_serial_number", f_serial_number);
                    sqlConnection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        flag = true;
                    }
                    reader.Close();
                }
                sqlConnection.Close();
            }
            return flag;
        }

        /// <summary>
        /// Checks if a given father serial number is valid.
        /// </summary>
        /// <param name="f_serial_number"></param>
        /// <returns>boolean</returns>
        private bool IsFSerialNumberValid(string f_serial_number)
        {
            bool flag = false;
            string temp;
            string connection = "Data Source=MAOR-ATAR-LAPTO;Initial Catalog=BirdHouseProjectDb;Integrated Security=True;";
            string query = "SELECT Gender FROM LadyGouldianFinch WHERE Serial_Number = @f_serial_number";

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@f_serial_number", f_serial_number);
                    sqlConnection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        temp = reader.GetString(0);
                        if (temp == "Male")
                        {
                            flag = true;
                        }
                    }
                    reader.Close();
                }
                sqlConnection.Close();
            }
            return flag;
        }

        /// <summary>
        /// Checks if a given mother serial number is in the LadyGouldianFinch SQL table.
        /// </summary>
        /// <param name="m_serial_number"></param>
        /// <returns></returns>
        private bool IsMSerialNumberInDB(string m_serial_number)
        {
            bool flag = false;
            string connection = "Data Source=MAOR-ATAR-LAPTO;Initial Catalog=BirdHouseProjectDb;Integrated Security=True;";
            string query = "SELECT Serial_Number FROM LadyGouldianFinch WHERE Serial_Number = @m_serial_number";

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@m_serial_number", m_serial_number);
                    sqlConnection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        flag = true;
                    }
                    reader.Close();
                }
                sqlConnection.Close();
            }
            return flag;
        }

        /// <summary>
        /// Checks if a given mother serial number is valid.
        /// </summary>
        /// <param name="m_serial_number"></param>
        /// <returns>boolean</returns>
        private bool IsMSerialNumberValid(string m_serial_number)
        {
            bool flag = false;
            string temp;
            string connection = "Data Source=MAOR-ATAR-LAPTO;Initial Catalog=BirdHouseProjectDb;Integrated Security=True;";
            string query = "SELECT Gender FROM LadyGouldianFinch WHERE Serial_Number = @m_serial_number";

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@m_serial_number", m_serial_number);
                    sqlConnection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        temp = reader.GetString(0);
                        if (temp == "Female")
                        {
                            flag = true;
                        }
                    }
                    reader.Close();
                }
                sqlConnection.Close();
            }
            return flag;
        }
    }
}