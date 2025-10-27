-- Tạo bảng StudentType
CREATE TABLE StudentType (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500)
);

-- Tạo bảng ModulesType
CREATE TABLE ModulesType (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500)
);

-- Tạo bảng CreditFees để lưu mức phí tín chỉ
CREATE TABLE CreditFees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StudentTypeId INT NOT NULL,
    ModuleTypeId INT NOT NULL,
    StudyType NVARCHAR(50) NOT NULL, -- Học lần 1, Học lần 2, Học cải thiện, etc.
    Fee DECIMAL(10,2) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    
    FOREIGN KEY (StudentTypeId) REFERENCES StudentType(Id),
    FOREIGN KEY (ModuleTypeId) REFERENCES ModulesType(Id)
);

-- Tạo index để tối ưu truy vấn
CREATE INDEX IX_CreditFees_StudentTypeId ON CreditFees(StudentTypeId);
CREATE INDEX IX_CreditFees_ModuleTypeId ON CreditFees(ModuleTypeId);
CREATE INDEX IX_CreditFees_StudyType ON CreditFees(StudyType);

-- Thêm dữ liệu mẫu cho StudentType
INSERT INTO StudentType (Name, Description) VALUES 
(N'Sinh viên chính quy', N'Sinh viên đại học chính quy'),
(N'Sinh viên tại chức', N'Sinh viên đại học tại chức'),
(N'Sinh viên liên thông', N'Sinh viên liên thông từ cao đẳng'),
(N'Sinh viên quốc tế', N'Sinh viên nước ngoài');

-- Thêm dữ liệu mẫu cho ModulesType
INSERT INTO ModulesType (Name, Description) VALUES 
(N'Lý thuyết', N'Các môn học lý thuyết cơ bản'),
(N'Thực hành', N'Các môn học thực hành'),
(N'Đồ án', N'Đồ án tốt nghiệp, tiểu luận'),
(N'Thực tập', N'Thực tập tốt nghiệp'),
(N'Chuyên đề', N'Các chuyên đề chuyên sâu');