# Credit Fee Manager - Quản lý Mức phí Tín chỉ

Ứng dụng WinForms để quản lý mức phí tín chỉ theo ma trận 3 chiều:
- **Chiều 1**: Loại sinh viên (StudentType)
- **Chiều 2**: Loại môn học (ModulesType)  
- **Chiều 3**: Loại học (Học lần 1, Học lần 2, Học cải thiện...)

## Hai phiên bản sẵn có

### 📱 **Phiên bản Cơ bản** (CreditFeeForm)
- Giao diện đơn giản, dễ sử dụng
- Nhập dữ liệu trực tiếp vào bảng
- Phù hợp cho người mới sử dụng
- Tập trung vào chức năng cốt lõi

### 🚀 **Phiên bản Nâng cao** (AdvancedCreditFeeForm)
- Hỗ trợ nhiều loại sinh viên (ComboBox)
- **Copy/Paste từ Excel** - Tính năng độc đáo!
- Xuất/nhập dữ liệu JSON
- Giao diện rộng hơn, nhiều tính năng hơn
- Phù hợp cho người dùng có kinh nghiệm

## Tính năng chính

### 🎯 Giao diện tối ưu
- **DataGridView** hiển thị dữ liệu dạng bảng trực quan
- **Cột đầu tiên**: Loại học (cố định)
- **Các cột tiếp theo**: Loại môn học (từ database)
- **Các hàng**: Tự động tạo cho từng loại học

### ⚡ Thao tác nhanh chóng
- **Nhập trực tiếp** vào các ô trong bảng
- **Validation tự động** cho số liệu
- **Format số** tự động (phân cách hàng nghìn)
- **Copy/Paste từ Excel** (phiên bản nâng cao)

### 💾 Quản lý dữ liệu
- **Lưu vào database** với transaction
- **Xuất JSON** để backup
- **Import dữ liệu** (có thể mở rộng)
- **Tải lại dữ liệu** từ database

## Cài đặt và sử dụng

### 1. Chuẩn bị Database
Chạy script `SQL/CreateTables.sql` để tạo các bảng cần thiết:
```sql
-- Tạo bảng StudentType, ModulesType, CreditFees
-- Thêm dữ liệu mẫu
```

### 2. Cấu hình Connection String
Khi chạy ứng dụng, nhập connection string:
```
Server=localhost;Database=YourDatabase;Trusted_Connection=true;
```

### 3. Sử dụng ứng dụng

#### Chọn phiên bản:
1. Sau khi nhập connection string, chọn phiên bản phù hợp
2. **Phiên bản Cơ bản**: Đơn giản, dễ sử dụng
3. **Phiên bản Nâng cao**: Nhiều tính năng, hỗ trợ Excel

#### Nhập dữ liệu:
1. Chọn ô cần nhập trong bảng
2. Nhập số tiền (VD: 150000)
3. Nhấn Enter hoặc Tab để chuyển ô
4. Dữ liệu tự động format thành "150,000"

#### Copy/Paste từ Excel (Phiên bản Nâng cao):
1. Copy dữ liệu từ Excel (bao gồm header)
2. Nhấn nút **"Paste Excel"**
3. Dữ liệu tự động được nhập vào bảng
4. Hoặc nhấn **"Copy Excel"** để copy dữ liệu ra Excel

#### Lưu dữ liệu:
1. Nhấn nút **"Lưu"** 
2. Dữ liệu được lưu vào bảng `CreditFees`
3. Hiển thị thông báo thành công

#### Xuất dữ liệu:
1. Nhấn nút **"Xuất JSON"**
2. Chọn vị trí lưu file JSON
3. File chứa toàn bộ dữ liệu hiện tại

## Cấu trúc dự án

```
CreditFeeManager/
├── Models/
│   ├── StudentType.cs
│   ├── ModuleType.cs
│   └── CreditFee.cs
├── Services/
│   └── DatabaseService.cs
├── Forms/
│   ├── CreditFeeForm.cs
│   └── ConnectionForm.cs
├── SQL/
│   └── CreateTables.sql
├── Program.cs
└── CreditFeeManager.csproj
```

## Ưu điểm của thiết kế

### 🎯 **Tối ưu cho người dùng:**
- **Giao diện trực quan**: Dạng bảng dễ nhìn, dễ nhập
- **Thao tác ít nhất**: Chỉ cần click và nhập số
- **Validation tự động**: Không cần kiểm tra thủ công
- **Format tự động**: Số được hiển thị đẹp

### 🔧 **Dễ mở rộng:**
- **Thêm loại học mới**: Chỉ cần thêm vào mảng `_studyTypes`
- **Thêm loại sinh viên**: Tự động từ database
- **Thêm loại môn học**: Tự động từ database
- **Export/Import**: Có thể mở rộng thêm format Excel

### 💾 **An toàn dữ liệu:**
- **Transaction**: Đảm bảo tính toàn vẹn
- **Validation**: Kiểm tra dữ liệu đầu vào
- **Backup**: Xuất JSON để backup
- **Error handling**: Xử lý lỗi tốt

## Mở rộng trong tương lai

1. **Import từ Excel**: Đọc file Excel trực tiếp
2. **Copy/Paste từ Excel**: Paste dữ liệu từ Excel vào bảng
3. **Phân quyền**: Theo loại sinh viên
4. **Lịch sử thay đổi**: Track thay đổi mức phí
5. **Báo cáo**: Xuất báo cáo PDF/Excel
6. **Web version**: Chuyển sang Blazor/ASP.NET Core

## Yêu cầu hệ thống

- .NET 8.0 hoặc cao hơn
- SQL Server (có thể thay đổi sang SQLite/PostgreSQL)
- Windows Forms support