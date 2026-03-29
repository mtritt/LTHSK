CREATE DATABASE QuanLyCuaHangQuanAo
USE QuanLyCuaHangQuanAo
GO

--Tạo bảng Tài khoản
CREATE TABLE tblTaiKhoan
(
	MaTK int PRIMARY KEY,
	TenTK nvarchar(30) not null UNIQUE,
	Matkhau nvarchar(40) not null,
	Quyen nvarchar(20) not null,
	CHECK (Quyen IN ('Admin', 'NhanVien', 'KhachHang')),
	TrangThai BIT NOT NULL DEFAULT 1
);

--Tạo bảng Nhân viên
CREATE TABLE tblNhanVien
(
	MaNV int not null PRIMARY KEY,
	TenNV nvarchar(50) not null,
	Diachi nvarchar(30) not null,
	Dienthoai nvarchar(15) not null,
	Ngaysinh DATE not null,
	Gioitinh nvarchar(10) not nulL,
	Luong DECIMAL(18,2) not null,
	MaTK int not null UNIQUE,
	CONSTRAINT FK_MaTK FOREIGN KEY (MaTK) REFERENCES tblTaiKhoan(MaTK)
);

--Tạo bảng Khách hàng
CREATE TABLE tblKhachHang
(
	MaKH int not null PRIMARY KEY,
	TenKH nvarchar(50) not null,
	Diachi nvarchar(30) not null,
	Dienthoai nvarchar(15) not null,
	Ngaysinh DATE not null,
	MaTK int not null UNIQUE,
	CONSTRAINT FK_KH_MaTK FOREIGN KEY (MaTK) REFERENCES tblTaiKhoan(MaTK)
);

--Tạo bảng Sản phẩm
CREATE TABLE tblSanPham
(	
	MaSP nvarchar(50) not null PRIMARY KEY,
	TenSP nvarchar(100) not null
);

--Tạo bảng SIZE
CREATE TABLE tblSize
(
	MaSize nvarchar(20) not null PRIMARY KEY,
	TenSize nvarchar(10) not null
);

--Tạo bảng Màu
CREATE TABLE tblMau
(	
	MaMau nvarchar(20) not null PRIMARY KEY,
	TenMau nvarchar(15) not null
);

--Tạo bảng Sản phẩm chi tiết
CREATE TABLE tblSanPhamChiTiet
(	
	MaSPCT nvarchar(20) not null PRIMARY KEY,
	MaSP nvarchar(50) not null,
	MaSize nvarchar(20) not null,
	MaMau nvarchar(20) not null,
	Giaban DECIMAL(18,2) not null,
	SoLuongTon INT not null DEFAULT 0
        CHECK (SoLuongTon >= 0),
	CONSTRAINT FK_SP_MaSP FOREIGN KEY (MaSP) REFERENCES tblSanPham(MaSP),
	CONSTRAINT FK_Size_MaSize FOREIGN KEY (MaSize) REFERENCES tblSize(MaSize),
	CONSTRAINT FK_Mau_MaMau FOREIGN KEY (MaMau) REFERENCES tblMau(MaMau)
);

--Tạo bảng NCC
CREATE TABLE tblNhaCungCap (
    MaNCC nvarchar(20) not null PRIMARY KEY,
    TenNCC nvarchar(100) not null,
    TenGiaoDich nvarchar(100) not null,
    DiaChi nvarchar(200) not null,
    DienThoai varchar(15)
);

CREATE TABLE tblPhieuNhap (
    MaPN nvarchar(20) not null PRIMARY KEY,
    MaNCC nvarchar(20) not null,
    MaNV int not null,
    NgayNhap DATE NOT NULL DEFAULT GETDATE(),
    TongTien DECIMAL(18,2),

    CONSTRAINT FK_PN_NCC
        FOREIGN KEY (MaNCC) REFERENCES tblNhaCungCap(MaNCC),

    CONSTRAINT FK_PN_NV
        FOREIGN KEY (MaNV) REFERENCES tblNhanVien(MaNV)
);


CREATE TABLE tblChiTietPhieuNhap (
    MaPN nvarchar(20) not null,
    MaSPCT nvarchar(20) not null,
    GiaNhap DECIMAL(18,2) NOT NULL,
    SoLuongNhap INT NOT NULL CHECK (SoLuongNhap > 0),
    ThanhTien AS (GiaNhap * SoLuongNhap),

    CONSTRAINT PK_CTPN
        PRIMARY KEY (MaPN, MaSPCT),

    CONSTRAINT FK_CTPN_PN
        FOREIGN KEY (MaPN) REFERENCES tblPhieuNhap(MaPN),

    CONSTRAINT FK_CTPN_SPCT
        FOREIGN KEY (MaSPCT) REFERENCES tblSanPhamChiTiet(MaSPCT)
);


CREATE TABLE tblHoaDonBan (
    MaHDB nvarchar(20) not null PRIMARY KEY,
    MaKH int not null ,
    MaNV INT NULL,
    NgayBan DATE NOT NULL DEFAULT GETDATE(),
    TrangThai NVARCHAR(50),
    TongTien DECIMAL(18,2),

    CONSTRAINT FK_HDB_KH
        FOREIGN KEY (MaKH) REFERENCES tblKhachHang(MaKH),

    CONSTRAINT FK_HDB_NV
        FOREIGN KEY (MaNV) REFERENCES tblNhanVien(MaNV)
);


CREATE TABLE tblChiTietHDB (
    MaHDB nvarchar(20) not null,
    MaSPCT nvarchar(20) not null,
    GiaBan DECIMAL(18,2) NOT NULL,
    SoLuongBan INT NOT NULL CHECK (SoLuongBan > 0),
    ThanhTien AS (GiaBan * SoLuongBan),

    CONSTRAINT PK_CTHDB
        PRIMARY KEY (MaHDB, MaSPCT),

    CONSTRAINT FK_CTHDB_HDB
        FOREIGN KEY (MaHDB) REFERENCES tblHoaDonBan(MaHDB),

    CONSTRAINT FK_CTHDB_SPCT
        FOREIGN KEY (MaSPCT) REFERENCES tblSanPhamChiTiet(MaSPCT)
);


ALTER TABLE tblSanPham
ADD MaNCC nvarchar(20) NOT NULL;

ALTER TABLE tblSanPham
ADD CONSTRAINT FK_SP_NCC
FOREIGN KEY (MaNCC) REFERENCES tblNhaCungCap(MaNCC);

--Nhập dữ liệu cho database
--tblTaiKhoan
INSERT INTO tblTaiKhoan (MaTK, TenTK, Matkhau, Quyen, TrangThai)
VALUES
(1, N'admin', N'202cb962ac59075b964b07152d234b70', N'Admin', 1),
(2, N'nv01', N'202cb962ac59075b964b07152d234b70', N'NhanVien', 1),
(3, N'nv02', N'202cb962ac59075b964b07152d234b70', N'NhanVien', 1),
(4, N'nv03', N'202cb962ac59075b964b07152d234b70', N'NhanVien', 1),
(5, N'kh01', N'202cb962ac59075b964b07152d234b70', N'KhachHang', 1),
(6, N'kh02', N'202cb962ac59075b964b07152d234b70', N'KhachHang', 1),
(7, N'kh03', N'202cb962ac59075b964b07152d234b70', N'KhachHang', 1),
(8, N'kh04', N'202cb962ac59075b964b07152d234b70', N'KhachHang', 1);



--tblNhanVien
INSERT INTO tblNhanVien VALUES
(1, N'Nguyễn Văn A', N'Hà Nội', '0123456789', '2000-01-01', N'Nam', 8000000, 2),
(2, N'Trần Văn B', N'Hà Nội', '0123456790', '1999-03-12', N'Nam', 7500000, 3),
(3, N'Lê Thị C', N'Hà Nội', '0123456791', '2001-07-21', N'Nữ', 8200000, 4);

--tblKhachHang
INSERT INTO tblKhachHang VALUES
(1, N'Phạm Thị D', N'Hà Nội', '0987654321', '2002-05-10', 5),
(2, N'Nguyễn Văn E', N'Hà Nội', '0987654322', '2003-06-15', 6),
(3, N'Trần Thị F', N'Hải Phòng', '0987654323', '2001-09-20', 7),
(4, N'Lê Văn G', N'Đà Nẵng', '0987654324', '2000-11-11', 8);

--tblNhaCungCap
INSERT INTO tblNhaCungCap VALUES
('NCC01', N'Công ty ABC', N'ABC Trading', N'Hà Nội', '0901111111'),
('NCC02', N'Công ty XYZ', N'XYZ Trading', N'HCM', '0902222222'),
('NCC03', N'Công ty Thời Trang Việt', N'VietFashion', N'Hà Nội', '0903333333'),
('NCC04', N'Công ty May Mặc Quốc Tế', N'GlobalWear', N'Đà Nẵng', '0904444444');

--tblSanPham
INSERT INTO tblSanPham VALUES
('SP01', N'Áo thun nam', 'NCC01'),
('SP02', N'Quần jean nữ', 'NCC02'),
('SP03', N'Áo sơ mi', 'NCC03'),
('SP04', N'Váy nữ', 'NCC02'),
('SP05', N'Áo khoác', 'NCC04'),
('SP06', N'Quần short', 'NCC01');

--tblSize
INSERT INTO tblSize VALUES
('S','S'),('M','M'),('L','L'),('XL','XL');

--tblMau
INSERT INTO tblMau VALUES
('DEN',N'Đen'),
('TRANG',N'Trắng'),
('DO',N'Đỏ'),
('XANH',N'Xanh');

--tblSanPhamChiTiet
INSERT INTO tblSanPhamChiTiet VALUES
('SPCT01','SP01','M','DEN',200000,50),
('SPCT02','SP01','L','TRANG',210000,40),
('SPCT03','SP02','M','XANH',350000,30),
('SPCT04','SP03','S','TRANG',250000,25),
('SPCT05','SP04','M','DO',400000,20),
('SPCT06','SP05','L','DEN',500000,15),
('SPCT07','SP06','S','XANH',180000,60);

INSERT INTO tblSanPhamChiTiet VALUES 
('SPCT08', 'SP01', 'S', 'DEN',   190000, 35),
('SPCT09', 'SP01', 'L', 'DEN',   210000, 20),
('SPCT10', 'SP01', 'S', 'TRANG', 190000, 45),
('SPCT11', 'SP01', 'M', 'TRANG', 200000, 50),
('SPCT12', 'SP01', 'M', 'DO',    195000, 40),
('SPCT13', 'SP01', 'L', 'XANH',  205000, 25),
('SPCT14', 'SP02', 'S', 'DEN',   340000, 25),
('SPCT15', 'SP02', 'L', 'TRANG', 360000, 20),
('SPCT16', 'SP02', 'M', 'DO',    350000, 10),

-- Bổ sung phân loại cho SP03 (Áo sơ mi)
('SPCT17', 'SP03', 'M', 'DEN',   250000, 30),
('SPCT18', 'SP03', 'L', 'XANH',  270000, 15),

-- Bổ sung phân loại cho SP04 (Váy nữ)
('SPCT19', 'SP04', 'S', 'DEN',   390000, 20),
('SPCT20', 'SP04', 'L', 'TRANG', 410000, 10),

-- Bổ sung phân loại cho SP05 (Áo khoác)
('SPCT21', 'SP05', 'M', 'TRANG', 490000, 15),
('SPCT22', 'SP05', 'S', 'XANH',  480000, 25),

-- Bổ sung phân loại cho SP06 (Quần short)
('SPCT23', 'SP06', 'M', 'DEN',   180000, 40),
('SPCT24', 'SP06', 'L', 'DO',    190000, 35);
INSERT INTO tblSanPhamChiTiet (MaSPCT, MaSP, MaSize, MaMau, Giaban, SoLuongTon)
VALUES 
-- Bổ sung 4 loại thiếu cho SP01 (Áo thun nam)
('SPCT25', 'SP01', 'S', 'DO',    190000, 30),
('SPCT26', 'SP01', 'S', 'XANH',  190000, 25),
('SPCT27', 'SP01', 'M', 'XANH',  200000, 30),
('SPCT28', 'SP01', 'L', 'DO',    210000, 20),

-- Bổ sung 8 loại thiếu cho SP02 (Quần jean nữ)
('SPCT29', 'SP02', 'S', 'DO',    340000, 20),
('SPCT30', 'SP02', 'S', 'TRANG', 340000, 25),
('SPCT31', 'SP02', 'S', 'XANH',  340000, 15),
('SPCT32', 'SP02', 'M', 'DEN',   350000, 30),
('SPCT33', 'SP02', 'M', 'TRANG', 350000, 20),
('SPCT34', 'SP02', 'L', 'DEN',   360000, 15),
('SPCT35', 'SP02', 'L', 'DO',    360000, 10),
('SPCT36', 'SP02', 'L', 'XANH',  360000, 20),

-- Bổ sung 9 loại thiếu cho SP03 (Áo sơ mi)
('SPCT37', 'SP03', 'S', 'DEN',   240000, 25),
('SPCT38', 'SP03', 'S', 'DO',    240000, 20),
('SPCT39', 'SP03', 'S', 'XANH',  240000, 15),
('SPCT40', 'SP03', 'M', 'DO',    250000, 25),
('SPCT41', 'SP03', 'M', 'TRANG', 250000, 30),
('SPCT42', 'SP03', 'M', 'XANH',  250000, 20),
('SPCT43', 'SP03', 'L', 'DEN',   260000, 15),
('SPCT44', 'SP03', 'L', 'DO',    260000, 10),
('SPCT45', 'SP03', 'L', 'TRANG', 260000, 20),

-- Bổ sung 9 loại thiếu cho SP04 (Váy nữ)
('SPCT46', 'SP04', 'S', 'DO',    390000, 25),
('SPCT47', 'SP04', 'S', 'TRANG', 390000, 20),
('SPCT48', 'SP04', 'S', 'XANH',  390000, 15),
('SPCT49', 'SP04', 'M', 'DEN',   400000, 30),
('SPCT50', 'SP04', 'M', 'TRANG', 400000, 25),
('SPCT51', 'SP04', 'M', 'XANH',  400000, 20),
('SPCT52', 'SP04', 'L', 'DEN',   410000, 15),
('SPCT53', 'SP04', 'L', 'DO',    410000, 10),
('SPCT54', 'SP04', 'L', 'XANH',  410000, 20),

-- Bổ sung 9 loại thiếu cho SP05 (Áo khoác)
('SPCT55', 'SP05', 'S', 'DEN',   480000, 20),
('SPCT56', 'SP05', 'S', 'DO',    480000, 15),
('SPCT57', 'SP05', 'S', 'TRANG', 480000, 25),
('SPCT58', 'SP05', 'M', 'DEN',   490000, 30),
('SPCT59', 'SP05', 'M', 'DO',    490000, 20),
('SPCT60', 'SP05', 'M', 'XANH',  490000, 15),
('SPCT61', 'SP05', 'L', 'DO',    500000, 10),
('SPCT62', 'SP05', 'L', 'TRANG', 500000, 15),
('SPCT63', 'SP05', 'L', 'XANH',  500000, 20),

-- Bổ sung 9 loại thiếu cho SP06 (Quần short)
('SPCT64', 'SP06', 'S', 'DEN',   170000, 30),
('SPCT65', 'SP06', 'S', 'DO',    170000, 25),
('SPCT66', 'SP06', 'S', 'TRANG', 170000, 20),
('SPCT67', 'SP06', 'M', 'DO',    180000, 20),
('SPCT68', 'SP06', 'M', 'TRANG', 180000, 25),
('SPCT69', 'SP06', 'M', 'XANH',  180000, 15),
('SPCT70', 'SP06', 'L', 'DEN',   190000, 20),
('SPCT71', 'SP06', 'L', 'TRANG', 190000, 15),
('SPCT72', 'SP06', 'L', 'XANH',  190000, 25);


--tblPhieuNhap
INSERT INTO tblPhieuNhap VALUES
('PN01','NCC01',1,GETDATE(),0),
('PN02','NCC02',2,GETDATE(),0),
('PN03','NCC03',3,GETDATE(),0),
('PN04','NCC04',1,GETDATE(),0);

--tblChiTietPhieuNhap
INSERT INTO tblChiTietPhieuNhap (MaPN,MaSPCT,GiaNhap,SoLuongNhap) VALUES
('PN01','SPCT01',150000,20),
('PN01','SPCT02',160000,15),
('PN02','SPCT03',250000,10),
('PN03','SPCT04',180000,12),
('PN04','SPCT06',350000,8);

--tblHoaDonBan
INSERT INTO tblHoaDonBan VALUES
('HDB01',1,1,GETDATE(),N'Đã thanh toán',0),
('HDB02',2,2,GETDATE(),N'Đã thanh toán',0),
('HDB03',3,3,GETDATE(),N'Chưa thanh toán',0);

--tblChiTietHDB
INSERT INTO tblChiTietHDB (MaHDB,MaSPCT,GiaBan,SoLuongBan) VALUES
('HDB01','SPCT01',200000,2),
('HDB01','SPCT02',210000,1),
('HDB02','SPCT03',350000,1),
('HDB02','SPCT04',250000,2),
('HDB03','SPCT05',400000,1);

UPDATE tblHoaDonBan
SET TongTien = (
    SELECT SUM(ThanhTien)
    FROM tblChiTietHDB
    WHERE tblChiTietHDB.MaHDB = tblHoaDonBan.MaHDB
);

UPDATE tblPhieuNhap
SET TongTien = (
    SELECT SUM(ThanhTien)
    FROM tblChiTietPhieuNhap
    WHERE tblChiTietPhieuNhap.MaPN = tblPhieuNhap.MaPN
);

select * from tblSanPham
select * from tblKhachHang
select * from tblHoaDonBan
select * from tblNhanVien
select * from tblChiTietHDB
select * from tblSanPhamChiTiet

SELECT * FROM tblSanPhamChiTiet
ORDER BY MaSP ASC;
SELECT COUNT(*) FROM tblSanPhamChiTiet

