namespace CreditFeeManager.Forms
{
    public partial class FormSelectorDialog : Form
    {
        private RadioButton _rbBasic;
        private RadioButton _rbAdvanced;
        private Button _btnOK;
        private Button _btnCancel;
        private Label _lblTitle;
        private Label _lblDescription;

        public bool UseAdvancedForm { get; private set; } = false;

        public FormSelectorDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Chọn phiên bản";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(20)
            };

            // Title
            _lblTitle = new Label
            {
                Text = "Chọn phiên bản sử dụng",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Description
            _lblDescription = new Label
            {
                Text = "Chọn phiên bản phù hợp với nhu cầu của bạn:",
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Radio buttons panel
            var radioPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            _rbBasic = new RadioButton
            {
                Text = "Phiên bản Cơ bản",
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                Checked = true
            };

            var lblBasicDesc = new Label
            {
                Text = "• Giao diện đơn giản\n• Nhập dữ liệu trực tiếp\n• Phù hợp cho người mới sử dụng",
                Location = new Point(40, 50),
                Size = new Size(400, 60),
                Font = new Font(Font.FontFamily, 9)
            };

            _rbAdvanced = new RadioButton
            {
                Text = "Phiên bản Nâng cao",
                Location = new Point(20, 120),
                Size = new Size(200, 25)
            };

            var lblAdvancedDesc = new Label
            {
                Text = "• Hỗ trợ nhiều loại sinh viên\n• Copy/Paste từ Excel\n• Tính năng xuất/nhập dữ liệu\n• Phù hợp cho người dùng có kinh nghiệm",
                Location = new Point(40, 150),
                Size = new Size(400, 80),
                Font = new Font(Font.FontFamily, 9)
            };

            radioPanel.Controls.AddRange(new Control[] { _rbBasic, lblBasicDesc, _rbAdvanced, lblAdvancedDesc });

            // Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                FlowDirection = FlowDirection.RightToLeft
            };

            _btnCancel = new Button
            {
                Text = "Hủy",
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };

            _btnOK = new Button
            {
                Text = "OK",
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.OK
            };
            _btnOK.Click += BtnOK_Click;

            buttonPanel.Controls.AddRange(new Control[] { _btnCancel, _btnOK });

            mainPanel.Controls.Add(_lblTitle, 0, 0);
            mainPanel.Controls.Add(_lblDescription, 0, 1);
            mainPanel.Controls.Add(radioPanel, 0, 2);
            mainPanel.Controls.Add(buttonPanel, 0, 3);

            this.Controls.Add(mainPanel);
            this.AcceptButton = _btnOK;
            this.CancelButton = _btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            UseAdvancedForm = _rbAdvanced.Checked;
        }
    }
}