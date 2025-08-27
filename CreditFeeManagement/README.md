# Hệ thống quản lý phí tín chỉ

## Tổng quan

Hệ thống quản lý phí tín chỉ là một ứng dụng web được xây dựng bằng **ASP.NET Core Blazor Server** để quản lý mảng 3 chiều phí tín chỉ của các trường đại học.

### Cấu trúc mảng 3 chiều:
- **Chiều 1**: Loại sinh viên (StudentType) - từ database
- **Chiều 2**: Loại môn học (ModulesType) - từ database  
- **Chiều 3**: Loại học [Học lần 1, Học lần 2, Học cải thiện] - cố định

## Tính năng chính

### 🎯 Form nhập dữ liệu tối ưu
- **Matrix Grid Layout** với Tab Navigation cho từng loại sinh viên
- Nhập trực tiếp vào cell với validation realtime
- Keyboard navigation (Tab, Enter, Arrow keys)
- Cell selection (single, multiple, range)

### ⚡ Batch Operations (Giảm thiểu thao tác)
- **Fill operations**: Điền cùng giá trị cho nhiều ô
- **Copy/Paste**: Sao chép giá trị giữa các ô
- **Range selection**: Chọn vùng ô với Shift+Click
- **Fill row/column**: Điền cả hàng/cột
- **Templates**: Áp dụng mẫu có sẵn

### 📊 Import/Export Excel
- Xuất dữ liệu ra Excel với format đẹp
- Nhập dữ liệu từ Excel template
- Validate dữ liệu khi import

### ✅ Validation & Error Handling
- Validation business rules:
  - Phí học lần 2 >= Phí học lần 1
  - Phí học cải thiện >= Phí học lần 1
  - Phí không được âm
- Warning cho giá trị bất thường
- Error handling với thông báo chi tiết

## Công nghệ sử dụng

- **Frontend**: Blazor Server, Bootstrap 5, Font Awesome
- **Backend**: ASP.NET Core 8.0, Entity Framework Core
- **Database**: SQL Server
- **Excel**: ClosedXML library
- **UI**: Responsive design, modern UX

## Cài đặt và chạy

### 1. Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server)
- Visual Studio 2022 hoặc VS Code

### 2. Cài đặt
```bash
# Clone repository
git clone <repository-url>
cd CreditFeeManagement

# Restore packages
dotnet restore

# Update database connection string in appsettings.json nếu cần

# Run migrations
dotnet ef database update

# Run application
dotnet run
```

### 3. Truy cập ứng dụng
Mở trình duyệt và truy cập: `https://localhost:5001`

## Hướng dẫn sử dụng

### Nhập dữ liệu
1. **Chọn tab** loại sinh viên cần nhập
2. **Click vào ô** để nhập giá trị phí
3. **Sử dụng Tab/Enter** để di chuyển giữa các ô
4. **Chọn nhiều ô** với Ctrl+Click hoặc Shift+Click

### Thao tác hàng loạt
1. **Nhập giá trị** vào ô "Giá trị điền"
2. **Chọn các ô** cần điền
3. **Click "Điền ô đã chọn"** hoặc sử dụng các nút điền hàng/cột

### Copy/Paste
1. **Chọn ô nguồn** và click "Copy"
2. **Chọn ô đích** và click "Paste"
3. Hoặc sử dụng **Ctrl+C/Ctrl+V**

### Import/Export Excel
1. **Export**: Click "Xuất Excel" để tải file Excel
2. **Import**: Click "Nhập Excel" và chọn file

### Validation
1. Click **"Kiểm tra dữ liệu"** để validate
2. Xem **errors/warnings** trong popup
3. **Sửa lỗi** nếu có và kiểm tra lại

## Kiến trúc hệ thống

### Models
- `StudentType`: Loại sinh viên
- `ModulesType`: Loại môn học  
- `StudyType`: Enum loại học (Lần 1, Lần 2, Cải thiện)
- `CreditFee`: Bảng phí tín chỉ chính

### ViewModels
- `CreditFeeMatrixViewModel`: ViewModel chính cho matrix UI
- `CellSelection`: Model cho việc chọn cell

### Services
- `ICreditFeeService`: Interface service
- `CreditFeeService`: Implementation với các methods:
  - CRUD operations
  - Import/Export Excel
  - Validation
  - Bulk operations

### Components
- `CreditFeeMatrix.razor`: Component chính
- Reusable và có thể tích hợp vào các project khác

## Tối ưu hóa UX

### 🚀 Giảm thiểu thao tác
- **Tab navigation**: Không cần click từng ô
- **Batch fill**: Điền nhiều ô cùng lúc
- **Copy/Paste**: Sao chép nhanh giá trị
- **Range selection**: Chọn vùng ô với 1 lần kéo
- **Keyboard shortcuts**: Ctrl+C/V, Delete key

### 🎨 UI/UX thân thiện
- **Responsive design**: Hoạt động tốt trên mobile
- **Visual feedback**: Highlight cell đã chọn
- **Loading states**: Hiển thị trạng thái loading
- **Status messages**: Thông báo kết quả thao tác
- **Modern design**: Bootstrap 5, gradient, shadows

### ⚡ Performance
- **Virtual scrolling**: Cho dataset lớn (placeholder)
- **Lazy loading**: Load data theo tab
- **Batch operations**: Xử lý hàng loạt hiệu quả
- **Optimistic UI**: Update UI ngay không đợi server

## Mở rộng

### Tính năng có thể thêm
- **Templates**: Mẫu phí có sẵn
- **History**: Lịch sử thay đổi
- **Approval workflow**: Quy trình phê duyệt
- **Reports**: Báo cáo phân tích
- **Multi-tenant**: Hỗ trợ nhiều trường
- **API**: REST API cho integration

### Customization
- **Business rules**: Thay đổi validation rules
- **UI themes**: Thêm themes khác
- **Languages**: Đa ngôn ngữ
- **Export formats**: PDF, CSV...

## Support

Để được hỗ trợ hoặc báo lỗi, vui lòng tạo issue trên repository hoặc liên hệ team phát triển.