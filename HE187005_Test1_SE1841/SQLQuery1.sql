CREATE TABLE Employees (
    EmpID INT PRIMARY KEY IDENTITY(1,1),
    EmpName NVARCHAR(255) NOT NULL,
    IDCard NVARCHAR(255) NOT NULL,
    Gender NVARCHAR(50) NULL,
    Address NVARCHAR(255) NULL,
    Phone NVARCHAR(50) NULL
);

INSERT INTO Employees (EmpName, IDCard, Gender, Address, Phone)
VALUES 
(N'Nguyễn Văn A', N'012345678', N'Nam', N'123 Lê Lợi, Hà Nội', N'0901234567'),
(N'Trần Thị B', N'987654321', N'Nữ', N'456 Trần Phú, TP.HCM', N'0912345678'),
(N'Lê Văn C', N'111222333', N'Nam', N'789 Nguyễn Huệ, Đà Nẵng', N'0923456789'),
(N'Phạm Thị D', N'444555666', N'Nữ', N'321 Hai Bà Trưng, Cần Thơ', N'0934567890'),
(N'Hoàng Văn E', N'777888999', N'Nam', N'654 Lý Thường Kiệt, Hải Phòng', N'0945678901');
