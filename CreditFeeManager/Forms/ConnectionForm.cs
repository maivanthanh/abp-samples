namespace CreditFeeManager.Forms
{
    public partial class ConnectionForm : Form
    {
        private TextBox _txtConnectionString;
        private Button _btnOK;
        private Button _btnCancel;
        private Label _lblConnectionString;

        public string ConnectionString { get; private set; } = string.Empty;

        public ConnectionForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Cấu hình kết nối Database";
            this.Size = new Size(600, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };

            // Label
            _lblConnectionString = new Label
            {
                Text = "Connection String:",
                Dock = DockStyle.Top,
                Height = 25
            };

            // TextBox
            _lblConnectionString = new Label
            {
                Text = "Connection String:",
                Dock = DockStyle.Top,
                Height = 25
            };

            _txtConnectionString = new TextBox
            {
                Text = "Server=localhost;Database=YourDatabase;Trusted_Connection=true;",
                Dock = DockStyle.Fill,
                Multiline = true,
                Height = 60
            };

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

            mainPanel.Controls.Add(_lblConnectionString, 0, 0);
            mainPanel.Controls.Add(_txtConnectionString, 0, 1);
            mainPanel.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(mainPanel);
            this.AcceptButton = _btnOK;
            this.CancelButton = _btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtConnectionString.Text))
            {
                MessageBox.Show("Vui lòng nhập Connection String!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            ConnectionString = _txtConnectionString.Text.Trim();
        }
    }
}