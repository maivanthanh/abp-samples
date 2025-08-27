using CreditFeeManager.Forms;

namespace CreditFeeManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            // Cấu hình connection string - thay đổi theo database của bạn
            var connectionString = "Server=localhost;Database=YourDatabase;Trusted_Connection=true;";
            
            // Hiển thị dialog để nhập connection string
            using (var connectionForm = new ConnectionForm())
            {
                if (connectionForm.ShowDialog() == DialogResult.OK)
                {
                    connectionString = connectionForm.ConnectionString;
                }
                else
                {
                    return; // Thoát nếu không nhập connection string
                }
            }
            
            // Hiển thị dialog để chọn loại form
            using (var formSelector = new FormSelectorDialog())
            {
                if (formSelector.ShowDialog() == DialogResult.OK)
                {
                    if (formSelector.UseAdvancedForm)
                    {
                        Application.Run(new AdvancedCreditFeeForm(connectionString));
                    }
                    else
                    {
                        Application.Run(new CreditFeeForm(connectionString));
                    }
                }
            }
        }
    }
}