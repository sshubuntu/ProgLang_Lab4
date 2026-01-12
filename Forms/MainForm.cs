using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GradebookApp.Data;
using GradebookApp.Models;

namespace GradebookApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly Gradebook _gradebook;
        private readonly GradebookContext _context;
        private DataGridView? _dataGridView;
        private Button? _btnAdd;
        private Button? _btnDelete;
        private Button? _btnSearch;
        private Button? _btnRefresh;
        private TextBox? _txtStudentName;
        private TextBox? _txtSubjectName;
        private TextBox? _txtMark;
        private TextBox? _txtSearchSubject;
        private Label? _lblStatus;

        public MainForm()
        {
            _context = new GradebookContext();
            _gradebook = new Gradebook(_context);
            InitializeComponent();
            LoadAllRecords();
        }

        private void InitializeComponent()
        {
            this.Text = "Gradebook app";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 500);

            var menuPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.LightGray
            };

            var lblAddSection = new Label { Text = "Add record", Location = new Point(10, 10), Size = new Size(200, 20), Font = new Font("Arial", 10, FontStyle.Bold) };
            var lblStudent = new Label { Text = "Student Name:", Location = new Point(10, 35), Size = new Size(100, 20) };
            _txtStudentName = new TextBox { Location = new Point(115, 33), Size = new Size(150, 20) };
            var lblSubject = new Label { Text = "Subject:", Location = new Point(10, 60), Size = new Size(100, 20) };
            _txtSubjectName = new TextBox { Location = new Point(115, 58), Size = new Size(150, 20) };
            var lblMark = new Label { Text = "Mark (0-100):", Location = new Point(10, 85), Size = new Size(100, 20) };
            _txtMark = new TextBox { Location = new Point(115, 83), Size = new Size(150, 20) };

            _btnAdd = new Button { Text = "Add Record", Location = new Point(10, 110), Size = new Size(100, 30), BackColor = Color.LightGreen };
            _btnAdd.Click += BtnAdd_Click;

            var lblDeleteSection = new Label { Text = "Delete Record", Location = new Point(280, 10), Size = new Size(200, 20), Font = new Font("Arial", 10, FontStyle.Bold) };
            var lblDeleteStudent = new Label { Text = "Student Name:", Location = new Point(280, 35), Size = new Size(100, 20) };
            var txtDeleteStudent = new TextBox { Location = new Point(385, 33), Size = new Size(150, 20), Name = "txtDeleteStudent" };
            var lblDeleteSubject = new Label { Text = "Subject:", Location = new Point(280, 60), Size = new Size(100, 20) };
            var txtDeleteSubject = new TextBox { Location = new Point(385, 58), Size = new Size(150, 20), Name = "txtDeleteSubject" };

            _btnDelete = new Button { Text = "Delete Record", Location = new Point(280, 85), Size = new Size(100, 30), BackColor = Color.LightCoral };
            _btnDelete.Click += (s, e) => BtnDelete_Click(s, e, txtDeleteStudent, txtDeleteSubject);

            var lblSearchSection = new Label { Text = "Search by Subject", Location = new Point(550, 10), Size = new Size(200, 20), Font = new Font("Arial", 10, FontStyle.Bold) };
            var lblSearchSubject = new Label { Text = "Subject Name:", Location = new Point(550, 35), Size = new Size(100, 20) };
            _txtSearchSubject = new TextBox { Location = new Point(655, 33), Size = new Size(150, 20) };

            _btnSearch = new Button { Text = "Search", Location = new Point(550, 60), Size = new Size(100, 30), BackColor = Color.LightBlue };
            _btnSearch.Click += BtnSearch_Click;

            _btnRefresh = new Button { Text = "Refresh All", Location = new Point(550, 95), Size = new Size(100, 30), BackColor = Color.LightYellow };
            _btnRefresh.Click += BtnRefresh_Click;

            _lblStatus = new Label { Text = "Ready", Location = new Point(10, 145), Size = new Size(800, 20), ForeColor = Color.DarkGreen };

            menuPanel.Controls.AddRange(new Control[]
            {
                lblAddSection, lblStudent, _txtStudentName, lblSubject, _txtSubjectName,
                lblMark, _txtMark, _btnAdd,
                lblDeleteSection, lblDeleteStudent, txtDeleteStudent, lblDeleteSubject, txtDeleteSubject, _btnDelete,
                lblSearchSection, lblSearchSubject, _txtSearchSubject, _btnSearch, _btnRefresh,
                _lblStatus
            });

            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };

            var mainPanel = new Panel { Dock = DockStyle.Fill };
            mainPanel.Controls.Add(_dataGridView);

            this.Controls.Add(mainPanel);
            this.Controls.Add(menuPanel);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_txtStudentName?.Text))
                {
                    MessageBox.Show("Please enter a student name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_txtSubjectName?.Text))
                {
                    MessageBox.Show("Please enter a subject name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(_txtMark?.Text, out int mark) || mark < 0 || mark > 100)
                {
                    MessageBox.Show("Please enter a valid mark (0-100).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _gradebook.AddRecord(_txtStudentName.Text, _txtSubjectName.Text, mark);

                _lblStatus!.Text = $"Record added: {_txtStudentName.Text} - {_txtSubjectName.Text}: {mark}";
                _lblStatus.ForeColor = Color.DarkGreen;

                _txtStudentName.Clear();
                _txtSubjectName.Clear();
                _txtMark.Clear();

                LoadAllRecords();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblStatus!.Text = $"Error: {ex.Message}";
                _lblStatus.ForeColor = Color.Red;
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e, TextBox txtDeleteStudent, TextBox txtDeleteSubject)
        {
            try
            {
                var studentInput = txtDeleteStudent.Text?.Trim();
                var subjectInput = txtDeleteSubject.Text?.Trim();

                bool hasStudent = !string.IsNullOrWhiteSpace(studentInput);
                bool hasSubject = !string.IsNullOrWhiteSpace(subjectInput);

                if (!hasStudent && !hasSubject)
                {
                    MessageBox.Show(
                        "Please enter at least a student name or a subject.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string warning;
                if (hasStudent && hasSubject)
                {
                    warning =
                        $"You are about to delete record(s) for student \"{studentInput}\" and subject \"{subjectInput}\".\n" +
                        $"If there are multiple such records, all of them will be deleted.\n\nContinue?";
                }
                else if (hasStudent)
                {
                    warning =
                        $"Warning: all records for student \"{studentInput}\" will be deleted.\n\nContinue?";
                }
                else
                {
                    warning =
                        $"Warning: all records for subject \"{subjectInput}\" will be deleted.\n\nContinue?";
                }

                var result = MessageBox.Show(
                    warning,
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;

                int deletedCount = _gradebook.DeleteRecord(studentInput, subjectInput);

                if (deletedCount > 0)
                {
                    _lblStatus!.Text = $"Deleted {deletedCount} record(s).";
                    _lblStatus.ForeColor = Color.DarkGreen;
                    txtDeleteStudent.Clear();
                    txtDeleteSubject.Clear();
                    LoadAllRecords();
                }
                else
                {
                    MessageBox.Show("Record(s) not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _lblStatus!.Text = "Record(s) not found";
                    _lblStatus.ForeColor = Color.Orange;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblStatus!.Text = $"Error: {ex.Message}";
                _lblStatus.ForeColor = Color.Red;
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_txtSearchSubject?.Text))
                {
                    MessageBox.Show("Please enter a subject name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var records = _gradebook.SearchBySubject(_txtSearchSubject.Text);

                if (records.Count == 0)
                {
                    _lblStatus!.Text = $"No records found for subject: {_txtSearchSubject.Text}";
                    _lblStatus.ForeColor = Color.Orange;
                    _dataGridView!.DataSource = null;
                }
                else
                {
                    _dataGridView!.DataSource = records.Select(r => new
                    {
                        r.Id,
                        StudentName = r.StudentName,
                        SubjectName = r.SubjectName,
                        Mark = r.Mark,
                    }).ToList();

                    var stats = _gradebook.GetSubjectStatistics(_txtSearchSubject.Text);
                    _lblStatus!.Text = $"Found {records.Count} record(s) for {_txtSearchSubject.Text}. " + $"Average: {stats["Average"]:F2}, Max: {stats["Max"]}, Min: {stats["Min"]}";
                    _lblStatus.ForeColor = Color.DarkGreen;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblStatus!.Text = $"Error: {ex.Message}";
                _lblStatus.ForeColor = Color.Red;
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadAllRecords();
        }

        private void LoadAllRecords()
        {
            try
            {
                var records = _gradebook.GetAllRecords();

                _dataGridView!.DataSource = records.Select(r => new
                {
                    r.Id,
                    StudentName = r.StudentName,
                    SubjectName = r.SubjectName,
                    Mark = r.Mark,
                }).ToList();

                _lblStatus!.Text = $"Total records: {records.Count}";
                _lblStatus.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblStatus!.Text = $"Error: {ex.Message}";
                _lblStatus.ForeColor = Color.Red;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}