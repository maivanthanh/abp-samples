# Hệ thống Quản lý Mức phí Tín chỉ

## Tổng quan

Hệ thống quản lý mức phí tín chỉ được thiết kế để nhập và quản lý dữ liệu mảng 3 chiều cho mức phí tín chỉ một cách tối ưu nhất, với ít thao tác nhất cho người dùng.

## Cấu trúc dữ liệu

### 3 Chiều dữ liệu:

1. **Chiều 1**: Loại sinh viên (StudentType)
   - Lấy từ bảng `StudentType`
   - Ví dụ: Sinh viên chính quy, Sinh viên liên thông, Sinh viên quốc tế...

2. **Chiều 2**: Loại môn học (ModuleType)
   - Lấy từ bảng `ModuleType`
   - Ví dụ: Môn học cơ bản, Môn học chuyên ngành, Môn học tự chọn...

3. **Chiều 3**: Loại học (StudyAttemptType) - Cố định
   - Học lần 1
   - Học lần 2
   - Học cải thiện
   - Học lại
   - Học đặc biệt

## Tính năng chính

### 1. Nhập dữ liệu hàng loạt (Bulk Input)
- **Giao diện bảng ma trận**: Hiển thị tất cả kết hợp StudentType × ModuleType
- **Nhập trực tiếp**: Nhập giá trị trực tiếp vào các ô tương ứng
- **Tính năng hỗ trợ**:
  - Tải dữ liệu hiện tại
  - Áp dụng giá trị cho tất cả
  - Sao chép từ kỳ trước
  - Xóa tất cả dữ liệu

### 2. Xem và quản lý dữ liệu
- **Hiển thị dạng bảng**: Dễ dàng xem và so sánh
- **Lọc theo năm học/học kỳ**: Tìm kiếm nhanh chóng
- **Thống kê**: Tổng hợp số liệu
- **Xuất Excel**: Xuất dữ liệu ra file Excel

### 3. Tối ưu hóa UX
- **Responsive design**: Hoạt động tốt trên mọi thiết bị
- **Giao diện trực quan**: Sử dụng màu sắc và icon để phân biệt
- **Validation**: Kiểm tra dữ liệu trước khi lưu
- **Loading states**: Hiển thị trạng thái tải
- **Thông báo**: Feedback rõ ràng cho người dùng

## Cách sử dụng

### Bước 1: Truy cập hệ thống
- Đăng nhập vào hệ thống
- Truy cập trang "Quản lý mức phí tín chỉ"

### Bước 2: Nhập dữ liệu
1. **Chọn năm học và học kỳ**
2. **Tải dữ liệu hiện tại** (nếu có)
3. **Nhập giá trị** vào các ô tương ứng
4. **Kiểm tra dữ liệu** trước khi lưu
5. **Lưu dữ liệu**

### Bước 3: Quản lý dữ liệu
1. Chuyển sang tab "Xem dữ liệu"
2. Chọn năm học và học kỳ cần xem
3. Xem dữ liệu dạng bảng
4. Xuất Excel nếu cần

## Lợi ích của thiết kế

### 1. Tối ưu thao tác
- **Nhập hàng loạt**: Không cần nhập từng record một
- **Giao diện ma trận**: Dễ dàng nhìn thấy tất cả kết hợp
- **Tính năng hỗ trợ**: Giảm thiểu thao tác lặp lại

### 2. Dễ sử dụng
- **Trực quan**: Giao diện rõ ràng, dễ hiểu
- **Responsive**: Hoạt động trên mọi thiết bị
- **Validation**: Ngăn chặn lỗi dữ liệu

### 3. Hiệu quả
- **Tốc độ**: Nhập nhanh chóng với bảng ma trận
- **Chính xác**: Validation và kiểm tra dữ liệu
- **Linh hoạt**: Dễ dàng chỉnh sửa và cập nhật

## Cấu trúc code

### Entities
- `StudentType`: Loại sinh viên
- `ModuleType`: Loại môn học
- `CreditFee`: Mức phí tín chỉ

### DTOs
- `CreditFeeDto`: DTO chính
- `BulkCreditFeeInputDto`: DTO nhập hàng loạt
- `CreditFeeTableDto`: DTO hiển thị bảng

### Components
- `CreditFeeManagement.razor`: Trang chính
- `BulkCreditFeeInput.razor`: Component nhập dữ liệu
- `CreditFeeDataView.razor`: Component xem dữ liệu

### Services
- `ICreditFeeAppService`: Interface service
- `CreditFeeAppService`: Implementation service

## Cài đặt và chạy

1. **Clone dự án**
2. **Restore packages**
3. **Update database**
4. **Chạy ứng dụng**

## Tương lai

### Tính năng có thể mở rộng
- Import/Export Excel
- Template dữ liệu
- Lịch sử thay đổi
- Phê duyệt dữ liệu
- Báo cáo thống kê

### Cải tiến UX
- Drag & drop
- Keyboard shortcuts
- Auto-save
- Undo/Redo
- Batch operations

## Hỗ trợ

Nếu có vấn đề hoặc cần hỗ trợ, vui lòng liên hệ:
- Email: support@example.com
- Phone: +84 xxx xxx xxx
- Documentation: [Link tài liệu]