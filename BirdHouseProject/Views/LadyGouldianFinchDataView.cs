﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BirdHouseProject.Models;

namespace BirdHouseProject.Views
{
    public partial class LadyGouldianFinchDataView : Form
    {
        // Fields
        SqlConnection connectionString = new SqlConnection(@"Data Source=MAOR-ATAR-LAPTO;Initial Catalog=BirdHouseProjectDb;Integrated Security=True;");

        // Constructors
        public LadyGouldianFinchDataView(int serialNumber, string species, string subSpecies,
    string hatchDate, string gender, string cageNumber, int fSerialNumber, int mSerialNumber,
    string headColor, string breastColor, string bodyColor)
        {
            InitializeComponent();
            showChickTable(serialNumber);
            // Fill the current bird details
            serialBox.Text = serialNumber.ToString();
            speciesBox.Text = species;
            subSpeciesBox.Text = subSpecies;
            hatchDateBox.Text = hatchDate;
            genderBox.Text = gender;
            cageNumberBox.Text = cageNumber;
            fSerialBox.Text = fSerialNumber.ToString();
            mSerialBox.Text = mSerialNumber.ToString();
            headColorBox.Text = headColor;
            breastColorBox.Text = breastColor;
            bodyColorBox.Text = bodyColor;
            // Fill the Chick inherited details
            speciesBox2.Text = species;
            subSpeciesBox2.Text = subSpecies;
            cageNumberBox2.Text = cageNumber;
            fSerialBox2.Text = serialNumber.ToString();
            // Add Chick
            addChickBtn.Click += delegate
            {
                AddNewEvent?.Invoke(this, EventArgs.Empty);
                tabControl1.TabPages.Remove(tabPage1);
                tabControl1.TabPages.Add(tabPage2);
                tabPage2.Text = "Add new bird";
            };
            // Cancel
            cancelBtn2.Click += delegate
            {
                CancelEvent?.Invoke(this, EventArgs.Empty);
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Add(tabPage1);
            };
            tabControl1.TabPages.Remove(tabPage2);
        }

        // Getters & Setters
        public string LadyGouldianFinchSerialNumber { get => serialBox2.Text; set => serialBox2.Text = value; }
        public string LadyGouldianFinchSpecies { get => speciesBox2.Text; set => speciesBox2.Text = value; }
        public string LadyGouldianFinchSubSpecies { get => subSpeciesBox2.Text; set => subSpeciesBox2.Text = value; }
        public string LadyGouldianFinchHatchDate { get => hatchDateBox2.Text; set => hatchDateBox2.Text = value; }
        public string LadyGouldianFinchGender { get => genderComboBox2.Text.ToString(); set => genderComboBox2.Text = value; }
        public string LadyGouldianFinchCageNumber { get => cageNumberBox2.Text; set => cageNumberBox2.Text = value; }
        public int LadyGouldianFinchFSerialNumber { get => Convert.ToInt32(fSerialBox2.Text); set => fSerialBox2.Text = Convert.ToString(value); }
        public int LadyGouldianFinchMSerialNumber { get => Convert.ToInt32(mSerialBox2.Text); set => mSerialBox2.Text = Convert.ToString(value); }
        public string LadyGouldianFinchHeadColor { get => headColorBox2.Text.ToString(); set => headColorBox2.Text = value; }
        public string LadyGouldianFinchBreastColor { get => breastColorBox2.Text.ToString(); set => breastColorBox2.Text = value; }
        public string LadyGouldianFinchBodyColor { get => bodyColorBox2.Text.ToString(); set => bodyColorBox2.Text = value; }

        // Events
        public event EventHandler AddNewEvent;
        public event EventHandler CancelEvent;

        // Methods
        private void showChickTable(int serialNumber)
        {
            connectionString.Open();
            string query = "Select * From LadyGouldianFinch WHERE F_Serial_Number = @serial_number Or M_Serial_Number = @serial_number";
            SqlCommand command = new SqlCommand(query, connectionString);
            command.Parameters.AddWithValue("@serial_number", serialNumber);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView.DataSource = dt;
            connectionString.Close();
        }

        private void saveBtn2_Click(object sender, EventArgs e)
        {
            using (var connection = this.connectionString)
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "Insert into LadyGouldianFinch values (@species, @sub_species, @hatch_date, @gender, " +
                    "@cage_number, @f_serial_number, @m_serial_number, @head_color, @breast_color, @body_color)";
                command.Parameters.Add("@species", SqlDbType.NVarChar).Value = LadyGouldianFinchSpecies;
                command.Parameters.Add("@sub_species", SqlDbType.NVarChar).Value = LadyGouldianFinchSubSpecies;
                command.Parameters.Add("@hatch_date", SqlDbType.NVarChar).Value = LadyGouldianFinchHatchDate;
                command.Parameters.Add("@gender", SqlDbType.NVarChar).Value = LadyGouldianFinchGender;
                command.Parameters.Add("@cage_number", SqlDbType.NVarChar).Value = LadyGouldianFinchCageNumber;
                command.Parameters.Add("@f_serial_number", SqlDbType.Int).Value = LadyGouldianFinchFSerialNumber;
                command.Parameters.Add("@m_serial_number", SqlDbType.Int).Value = LadyGouldianFinchMSerialNumber;
                command.Parameters.Add("@head_color", SqlDbType.NVarChar).Value = CalcChickHeadColor();
                command.Parameters.Add("@breast_color", SqlDbType.NVarChar).Value = LadyGouldianFinchBreastColor;
                command.Parameters.Add("@body_color", SqlDbType.NVarChar).Value = LadyGouldianFinchBodyColor;
                command.ExecuteNonQuery();
            }
            this.Close();
        }

        private string CalcChickHeadColor()
        {
            string f_head_color = null;
            string m_head_color = null;

            string connection = "Data Source=MAOR-ATAR-LAPTO;Initial Catalog=BirdHouseProjectDb;Integrated Security=True;";
            string query1 = "SELECT Head_Color FROM LadyGouldianFinch WHERE Serial_Number = @f_serial_number";
            string query2 = "SELECT Head_Color FROM LadyGouldianFinch WHERE Serial_Number = @m_serial_number";

            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                // Retrieve the head color of the female bird
                using (SqlCommand command1 = new SqlCommand(query1, sqlConnection))
                {
                    command1.Parameters.AddWithValue("@f_serial_number", LadyGouldianFinchFSerialNumber);
                    sqlConnection.Open();
                    SqlDataReader reader1 = command1.ExecuteReader();
                    if (reader1.HasRows)
                    {
                        reader1.Read();
                        f_head_color = reader1.GetString(0);
                    }
                    reader1.Close();
                }

                // Retrieve the head color of the male bird
                using (SqlCommand command2 = new SqlCommand(query2, sqlConnection))
                {
                    command2.Parameters.AddWithValue("@m_serial_number", LadyGouldianFinchMSerialNumber);
                    SqlDataReader reader2 = command2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        reader2.Read();
                        m_head_color = reader2.GetString(0);
                    }
                    reader2.Close();
                }

                sqlConnection.Close();
            }
            if(f_head_color == "Red" && m_head_color == "Red")
            {
                return "Blue";
            }
            return "Yellow";
        }
    }
}