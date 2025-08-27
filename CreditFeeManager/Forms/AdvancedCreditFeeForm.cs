using CreditFeeManager.Models;
using CreditFeeManager.Services;
using Newtonsoft.Json;

namespace CreditFeeManager.Forms
{
    public partial class AdvancedCreditFeeForm : Form
    {
        private readonly DatabaseService _databaseService;
        private List<StudentType> _studentTypes = new();
        private List<ModuleType> _moduleTypes = new();
        private DataGridView _dataGridView;
        private ComboBox _cboStudentType;
        private Button _btnSave;
        private Button _btnLoad;
        private Button _btnExport;
        private Button _btnImport;
        private Button _btnPasteExcel;
        private Button _btnCopyExcel;
        private Label _lblStatus;
        private Label _lblStudentType;

        private readonly string[] _studyTypes = { "Học lần 1", "Học lần 2", "Học cải thiện", "Học lại", "Học bổ sung" };

        public AdvancedCreditFeeForm(string connectionString)
        {
            _databaseService = new DatabaseService(connectionString);
            InitializeComponent();
            LoadData();
            SetupDataGridView();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Mức phí Tín chỉ - Phiên bản Nâng cao";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tạo panel chính
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10)
            };

            // Panel cho controls
            var controlPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                ColumnCount = 2,
                RowCount = 2
            };

            // Panel cho Student Type
            var studentTypePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight
            };

            _lblStudentType = new Label
            {
                Text = "Loại sinh viên:",
                Width = 100,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _cboStudentType = new ComboBox
            {
                Width = 200,
                Height = 25,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboStudentType.SelectedIndexChanged += CboStudentType_SelectedIndexChanged;

            studentTypePanel.Controls.AddRange(new Control[] { _lblStudentType, _cboStudentType });

            // Panel cho buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
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
                Text = "Xuất JSON",
                Width = 100,
                Height = 35
            };
            _btnExport.Click += BtnExport_Click;

            _btnImport = new Button
            {
                Text = "Nhập JSON",
                Width = 100,
                Height = 35
            };
            _btnImport.Click += BtnImport_Click;

            _btnCopyExcel = new Button
            {
                Text = "Copy Excel",
                Width = 100,
                Height = 35
            };
            _btnCopyExcel.Click += BtnCopyExcel_Click;

            _btnPasteExcel = new Button
            {
                Text = "Paste Excel",
                Width = 100,
                Height = 35
            };
            _btnPasteExcel.Click += BtnPasteExcel_Click;

            buttonPanel.Controls.AddRange(new Control[] { _btnLoad, _btnSave, _btnExport, _btnImport, _btnCopyExcel, _btnPasteExcel });

            controlPanel.Controls.Add(studentTypePanel, 0, 0);
            controlPanel.Controls.Add(buttonPanel, 0, 1);

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
                EnableHeadersVisualStyles = false,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            };

            mainPanel.Controls.Add(controlPanel, 0, 0);
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
                
                // Populate combo box
                _cboStudentType.Items.Clear();
                foreach (var studentType in _studentTypes)
                {
                    _cboStudentType.Items.Add(studentType.Name);
                }
                
                if (_cboStudentType.Items.Count > 0)
                    _cboStudentType.SelectedIndex = 0;
                
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

            // Thiết lập validation và formatting
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

        private void CboStudentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Có thể load dữ liệu cho loại sinh viên được chọn
            _lblStatus.Text = $"Đã chọn: {_cboStudentType.Text}";
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
                var selectedStudentType = _studentTypes[_cboStudentType.SelectedIndex];
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
                            StudentTypeId = selectedStudentType.Id,
                            ModuleTypeId = moduleTypeId,
                            StudyType = studyType,
                            Fee = feeValue,
                            StudentTypeName = selectedStudentType.Name,
                            ModuleTypeName = moduleType?.Name ?? ""
                        });
                    }
                }

                _databaseService.SaveCreditFees(creditFees);
                _lblStatus.Text = $"Đã lưu {creditFees.Count} mức phí cho {selectedStudentType.Name}";
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
                    FileName = $"credit_fees_{_cboStudentType.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var data = new
                    {
                        StudentType = _studentTypes[_cboStudentType.SelectedIndex],
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

        private void BtnCopyExcel_Click(object sender, EventArgs e)
        {
            try
            {
                _dataGridView.SelectAll();
                _dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                _dataGridView.DoDragDrop(_dataGridView.GetClipboardContent(), DragDropEffects.Copy);
                
                _lblStatus.Text = "Đã copy dữ liệu vào clipboard";
                MessageBox.Show("Đã copy dữ liệu vào clipboard! Bạn có thể paste vào Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi copy: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPasteExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    var clipboardText = Clipboard.GetText();
                    var lines = clipboardText.Split('\n');
                    
                    if (lines.Length > 1)
                    {
                        // Bỏ qua dòng header
                        for (int i = 1; i < lines.Length && i - 1 < _dataGridView.Rows.Count; i++)
                        {
                            var cells = lines[i].Split('\t');
                            if (cells.Length > 1)
                            {
                                var row = _dataGridView.Rows[i - 1];
                                for (int j = 1; j < cells.Length && j < _dataGridView.Columns.Count; j++)
                                {
                                    if (decimal.TryParse(cells[j].Trim(), out decimal value))
                                    {
                                        row.Cells[j].Value = value.ToString();
                                    }
                                }
                            }
                        }
                        
                        _lblStatus.Text = "Đã paste dữ liệu từ clipboard";
                        MessageBox.Show("Đã paste dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu trong clipboard!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi paste: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<CreditFee> GetCurrentData()
        {
            var selectedStudentType = _studentTypes[_cboStudentType.SelectedIndex];
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
                        StudentTypeId = selectedStudentType.Id,
                        ModuleTypeId = moduleTypeId,
                        StudyType = studyType,
                        Fee = feeValue,
                        StudentTypeName = selectedStudentType.Name,
                        ModuleTypeName = moduleType?.Name ?? ""
                    });
                }
            }

            return creditFees;
        }
    }
}