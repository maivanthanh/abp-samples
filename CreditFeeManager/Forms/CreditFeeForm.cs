using CreditFeeManager.Models;
using CreditFeeManager.Services;
using Newtonsoft.Json;

namespace CreditFeeManager.Forms
{
    public partial class CreditFeeForm : Form
    {
        private readonly DatabaseService _databaseService;
        private List<StudentType> _studentTypes = new();
        private List<ModuleType> _moduleTypes = new();
        private List<CreditFee> _creditFees = new();
        private DataGridView _dataGridView;
        private Button _btnSave;
        private Button _btnLoad;
        private Button _btnExport;
        private Button _btnImport;
        private Label _lblStatus;

        private readonly string[] _studyTypes = { "Học lần 1", "Học lần 2", "Học cải thiện", "Học lại", "Học bổ sung" };

        public CreditFeeForm(string connectionString)
        {
            _databaseService = new DatabaseService(connectionString);
            InitializeComponent();
            LoadData();
            SetupDataGridView();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Mức phí Tín chỉ";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tạo panel chính
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };

            // Panel cho buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight
            };

            _btnLoad = new Button
            {
                Text = "Tải dữ liệu",
                Width = 100,
                Height = 35
            };
            _btnLoad.Click += BtnLoad_Click;

            _btnSave = new Button
            {
                Text = "Lưu",
                Width = 100,
                Height = 35
            };
            _btnSave.Click += BtnSave_Click;

            _btnExport = new Button
            {
                Text = "Xuất Excel",
                Width = 100,
                Height = 35
            };
            _btnExport.Click += BtnExport_Click;

            _btnImport = new Button
            {
                Text = "Nhập Excel",
                Width = 100,
                Height = 35
            };
            _btnImport.Click += BtnImport_Click;

            buttonPanel.Controls.AddRange(new Control[] { _btnLoad, _btnSave, _btnExport, _btnImport });

            // Status label
            _lblStatus = new Label
            {
                Text = "Sẵn sàng",
                Dock = DockStyle.Bottom,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // DataGridView
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false
            };

            mainPanel.Controls.Add(buttonPanel, 0, 0);
            mainPanel.Controls.Add(_dataGridView, 0, 1);
            mainPanel.Controls.Add(_lblStatus, 0, 2);

            this.Controls.Add(mainPanel);
        }

        private void LoadData()
        {
            try
            {
                _studentTypes = _databaseService.GetStudentTypes();
                _moduleTypes = _databaseService.GetModuleTypes();
                _lblStatus.Text = $"Đã tải {_studentTypes.Count} loại sinh viên và {_moduleTypes.Count} loại môn học";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            _dataGridView.Columns.Clear();
            _dataGridView.Rows.Clear();

            // Cột đầu tiên cho loại học
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudyType",
                HeaderText = "Loại học",
                ReadOnly = true,
                Width = 120
            });

            // Các cột cho từng loại môn học
            foreach (var moduleType in _moduleTypes)
            {
                _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = $"Module_{moduleType.Id}",
                    HeaderText = moduleType.Name,
                    Width = 100
                });
            }

            // Thêm các hàng cho từng loại học
            foreach (var studyType in _studyTypes)
            {
                var row = new DataGridViewRow();
                row.CreateCells(_dataGridView);
                row.Cells[0].Value = studyType;
                
                // Khởi tạo giá trị mặc định
                for (int i = 1; i < _dataGridView.Columns.Count; i++)
                {
                    row.Cells[i].Value = "0";
                }
                
                _dataGridView.Rows.Add(row);
            }

            // Thiết lập validation cho các ô số
            _dataGridView.CellValidating += DataGridView_CellValidating;
            _dataGridView.CellFormatting += DataGridView_CellFormatting;
        }

        private void DataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 0) return; // Bỏ qua cột loại học

            if (!decimal.TryParse(e.FormattedValue.ToString(), out decimal value))
            {
                e.Cancel = true;
                MessageBox.Show("Vui lòng nhập số hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (value < 0)
            {
                e.Cancel = true;
                MessageBox.Show("Mức phí không được âm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0) return; // Bỏ qua cột loại học

            if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
            {
                e.Value = value.ToString("N0");
                e.FormattingApplied = true;
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
            SetupDataGridView();
            _lblStatus.Text = "Đã tải lại dữ liệu";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var creditFees = new List<CreditFee>();

                foreach (DataGridViewRow row in _dataGridView.Rows)
                {
                    var studyType = row.Cells[0].Value?.ToString() ?? "";
                    
                    for (int i = 1; i < _dataGridView.Columns.Count; i++)
                    {
                        var moduleTypeId = int.Parse(_dataGridView.Columns[i].Name.Replace("Module_", ""));
                        var feeValue = decimal.Parse(row.Cells[i].Value?.ToString() ?? "0");
                        
                        var moduleType = _moduleTypes.FirstOrDefault(m => m.Id == moduleTypeId);
                        
                        creditFees.Add(new CreditFee
                        {
                            StudentTypeId = 1, // Mặc định, có thể mở rộng sau
                            ModuleTypeId = moduleTypeId,
                            StudyType = studyType,
                            Fee = feeValue,
                            ModuleTypeName = moduleType?.Name ?? ""
                        });
                    }
                }

                _databaseService.SaveCreditFees(creditFees);
                _lblStatus.Text = $"Đã lưu {creditFees.Count} mức phí";
                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    FileName = $"credit_fees_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var data = new
                    {
                        StudentTypes = _studentTypes,
                        ModuleTypes = _moduleTypes,
                        CreditFees = GetCurrentData()
                    };

                    var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    File.WriteAllText(saveFileDialog.FileName, json);
                    
                    _lblStatus.Text = $"Đã xuất dữ liệu ra {saveFileDialog.FileName}";
                    MessageBox.Show("Xuất dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var json = File.ReadAllText(openFileDialog.FileName);
                    var data = JsonConvert.DeserializeObject<dynamic>(json);
                    
                    // Có thể mở rộng để import dữ liệu
                    MessageBox.Show("Tính năng import sẽ được phát triển trong phiên bản tiếp theo!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi import dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<CreditFee> GetCurrentData()
        {
            var creditFees = new List<CreditFee>();

            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                var studyType = row.Cells[0].Value?.ToString() ?? "";
                
                for (int i = 1; i < _dataGridView.Columns.Count; i++)
                {
                    var moduleTypeId = int.Parse(_dataGridView.Columns[i].Name.Replace("Module_", ""));
                    var feeValue = decimal.Parse(row.Cells[i].Value?.ToString() ?? "0");
                    
                    var moduleType = _moduleTypes.FirstOrDefault(m => m.Id == moduleTypeId);
                    
                    creditFees.Add(new CreditFee
                    {
                        StudentTypeId = 1,
                        ModuleTypeId = moduleTypeId,
                        StudyType = studyType,
                        Fee = feeValue,
                        ModuleTypeName = moduleType?.Name ?? ""
                    });
                }
            }

            return creditFees;
        }
    }
}