USE MASTER

IF EXISTS(SELECT * FROM MASTER.SYS.SYSDATABASES WHERE NAME='BATDONGSAN')
	drop Database BATDONGSAN
GO
CREATE DATABASE BATDONGSAN
GO
USE BATDONGSAN
GO
----------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------
CREATE TABLE PHANQUYEN						   --PHÂN QUYỀN
(
	MAPQ INT  PRIMARY KEY NOT NULL,    --MÃ PHÂN QUYỀN
	CHUCVU NVARCHAR(50)               --CHỨC CỤ: ADMIN, NHÂN VIÊN,...
)
GO
INSERT INTO PHANQUYEN
VALUES
(1,'Admin'),
(2,N'Nhân viên')
GO
CREATE TABLE NHANVIEN
(
	MANV INT IDENTITY PRIMARY KEY NOT NULL,
	HOTEN NVARCHAR(50) NOT NULL,
	CHUCVU NVARCHAR(50) NOT NULL,
	GIOITINH NVARCHAR(6) CHECK (GIOITINH IN('Male','Female','Unk')) NOT NULL,
	DIACHI NVARCHAR(50),
	EMAIL VARCHAR(50),
	DIENTHOAI VARCHAR(12),
	LINK_FACEBOOK VARCHAR(255),
	TAIKHOAN VARCHAR(50),
	MATKHAU VARCHAR(50),
	MAPQ INT FOREIGN KEY REFERENCES PHANQUYEN(MAPQ)
)
GO
INSERT INTO NHANVIEN
VALUES
(N'Chưa được duyệt', N'Admin', 'Female','', '', 0987934364, '',N'null',123,1),
(N'Lã Thị Phương Thảo', N'Admin', N'Female',N'154 Trần Đình Xu, quận 1, tp.HCM', N'pt0987934364@mail.com', 0987934364, '',N'Thao',123,1),
(N'Mai Chấn Cường', N'Admin', N'Male',N'205/14 Quang Trung, quận 3, tp HCM', N'cuong123@gmail.com', 0957553437, '',N'Cuong',456,1),
(N'Cao Thành Nhơn', N'Admin', N'Male',N'28 Nguyễn Huệ, quận 3, tp HCM', N'nhon123@mail.com', 0947553427, '',N'Nhon',789,1)
GO


CREATE TABLE THANHVIEN                         --KHÁCH HÀNG ĐĂNG KÝ TÀI KHOẢN
(
	MATV INT PRIMARY KEY IDENTITY NOT NULL,
	TENTHANHVIEN NVARCHAR(50),
	GIOITINH  NVARCHAR(6) CHECK (GIOITINH IN('Nam',N'Nữ','Unk')) NOT NULL,
	DIACHI NVARCHAR(50),
	EMAIL VARCHAR(50),
	DIENTHOAI VARCHAR(12),
	NGAYDKTK DATE DEFAULT GETDATE() NOT NULL,            --NGÀY ĐĂNG KÝ TÀI KHOẢN
	TAIKHOAN VARCHAR(50),
	MATKHAU VARCHAR(50),
	LINK_FACEBOOK VARCHAR(255),
	ANH_DAI_DIEN VARCHAR(255),
	STATUS_DELETE INT DEFAULT 1
)
GO
INSERT INTO THANHVIEN 
VALUES
(N'Cao Ngọc Nhi', N'Nữ', N' 132/12 Cầu Giấy, Hà Nội', N'ngocnhi1207@gmail.com', '08364524372', '11-05-2021', N'Nhi', 1234,'', '',1),
(N'Mai Chấn Cường', N'Nam', N' Vũng Tàu', N'm.contactqc@gmail.com', '0382071075', '12-16-2021', N'Cuong', 123,'https://www.facebook.com/mcc1612/','cuong.png',1),
(N'Nguyễn Xuân Anh', N'Nam', N' 28 Trường Chinh, quận 3, tp HCM', N'xanh2341@gmail.com', '0957473527', '11-15-2021', N'Anh', 4567,'','',1)
GO
----------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------
CREATE TABLE LOAIBDS                           --PHÂN LOẠI BẤT ĐỘNG SẢN
(
	MALOAI INT IDENTITY PRIMARY KEY NOT NULL,
	TENLOAI NVARCHAR(100),
	STATUS_DELETE INT DEFAULT 1
)
GO
INSERT INTO LOAIBDS  
VALUES
(N'Nhà riêng', 1),
(N'Căn hộ chung cư', 1),
(N'Shophouse', 1),
(N'Officetel', 1)
GO
CREATE TABLE BDS                               --BẤT ĐỘNG SẢN
(
	MABDS INT IDENTITY PRIMARY KEY NOT NULL,
	MALOAI_BDS INT FOREIGN KEY REFERENCES LOAIBDS(MALOAI),
	TENBDS NVARCHAR(100) NOT NULL,
	THONGTIN NVARCHAR(1500) NOT NULL,
	GIA DECIMAL(10,2) check(GIA>0) NOT NULL,
	COC_TRUOC INT,														  -- SỐ TIỀN ĐƯỢC CỌC TRƯỚC(TÍNH THEO %)
	MA_AD_DUYET INT NOT NULL FOREIGN KEY REFERENCES NHANVIEN(MANV),                --MÃ NHÂN VIÊN KIỂM DUYỆT
	NGAYDANG DATE DEFAULT GETDATE() NOT NULL,
	DUYET INT CHECK (DUYET IN (1,0)) DEFAULT 0,                           --BẤT ĐỘNG SẢN ĐƯỢC DUYỆT = 1, CHƯA DUYỆT = 0
	SAO FLOAT DEFAULT 0,												  --SỐ SAO TRUNG BÌNH CỦA KHÁCH HÀNG ĐÁNH GIÁ CHO BẤT ĐỘNG SẢN
	HINHANH	VARCHAR(100),
	MATV INT FOREIGN KEY REFERENCES THANHVIEN(MATV),
	STATE_DELETE INT DEFAULT 1
)
GO
INSERT INTO BDS  
VALUES
(1, N'Bán nhà QL13, Hiệp Bình Phước 5.72 x 21.2 an ninh, tiện ích, chính chủ',
N'Bán nhà đường QL13 cũ (hoặc đường 17 vào), phường Hiệp Bình Phước, Thủ Đức.&nbsp;
Khu vực yên tĩnh, gần chợ gần trường học gần UBND phường Hiệp Bình Phước.&nbsp;
Diện tích: 121,3m2.&nbsp;
Mặt tiền: 5.71 m.&nbsp;
Đường vào: 8 m.&nbsp;
Pháp lý: Sổ đỏ/ Sổ hồng.&nbsp;
1 trệt, 1 lầu, 3 phòng ngủ.&nbsp;
Hẻm xe hơi trên đường QL13 cũ.&nbsp;
Khu dân cư hiện hữu, đông đúc.&nbsp;
Xung quanh bán kính 500m có đầy đủ chợ, siêu thị, bệnh viện, trường học,...&nbsp;
Pháp lý chuẩn chỉnh, không tranh cấp, không quy hoạch.', 6.6, 10, 2, '10-25-2021',1,0,'Nharieng1.jpg',2, 1),
(1,N'Bán nhà mặt tiền đường 17 khuôn viên sang trọng',
N'Bán nhà mặt tiền đường 17 khuôn viên sang trọng.&nbsp;
Địa chỉ: Đường 17, Phường Hiệp Bình Phước, Thủ Đức, Hồ Chí Minh.&nbsp;
Diện tích 71m2 - 150m2.&nbsp;
Mặt tiền:  12m.&nbsp;
Đường vào: 12m.&nbsp;
Xây dựng 3 lầu sân thượng theo kiến trúc tân cổ điển.&nbsp;
Số phòng ngủ: 4 phòng.&nbsp;
Sổ hồng hoàn công đầy đủ.&nbsp;
Công viên sinh hoạt vui chơi ngoài trời.&nbsp;
Thủ tục: cọc, công chứng sang tên nhanh chóng.&nbsp;
Ngân hàng hỗ trợ 70% dành cho A/C có nhu cầu.&nbsp;',7.8, 10,3,'07-12-2021',1,0,'Nhakhuonvien1.jpg',2, 1),
(1, N'Bán nhà để ở hoặc cho thuê. Đường xe hơi, khu đan cư đông đúc',
N'Bán nhà để ở hoặc cho thuê. Đường xe hơi, khu đan cư đông đúc.&nbsp;
Địa chỉ: Đường Quốc Lộ 13, Phường Hiệp Bình Phước, Thủ Đức, Hồ Chí Minh.&nbsp;
Đường vào: 10 m.&nbsp;
Số phòng: 4 phòng.&nbsp;
Diện tích: 81m2.&nbsp;
Sân rộng, để được xe hơi trong nhà thoải mái.&nbsp;
Đầy đủ tiện ích chợ, trường, quán xá bán kính 500m.&nbsp;
Nhà mới xây, chưa qua sử dụng.&nbsp;
Sổ đã hoàn công, có sẵn ở nhà.&nbsp;',5.5, 10,4,'08-22-2021',1,0,'Bannhaohoacthue1.jpg',2, 1), 
(2,N'Căn hộ Quận Bình Tân 62m2 2PN ở liền',
N'Căn hộ Quận Bình Tân 62m2 2PN ở liền.&nbsp;
Địa chỉ: 96, Đường Trần Đại Nghĩa, Phường Tân Tạo A, Quận Bình Tân, Tp Hồ Chí Minh.&nbsp;
Dự án: Chung Cư Vision.&nbsp;
Diện tích: 62 m2.&nbsp;
Căn hộ gồm: 2 phòng ngủ ; 2WC; ban công.&nbsp;
Nhà mới bàn giao, sạch sẽ thoáng mát.&nbsp;
Khu dân cư an ninh, đầy đủ tiện ích, gần chợ siêu thị Big C, trường học, bệnh viện, lên xem đảm bảo thích.&nbsp;',1.5, 10, 3, '09-25-2021',1,0,'Canho1.jpg',2, 1),
(2,N'Căn hộ chung cư Miếu Nổi tầng 17 thoáng mát, cửa hướng Tây Bắc',
N'BÁN CĂN HỘ CHUNG CƯ MIẾU NỔI, QUẬN BÌNH THẠNH.&nbsp;
- Địa chỉ: Vũ Huy Tấn, Phường 3, Bình Thạnh,tp HCM.&nbsp;
- Diện tích: 57m2.&nbsp;
- Kết cấu: 2 phòng ngủ và 1 phòng tắm.&nbsp;
- Căn hộ hướng Tây Bắc, từ phòng khách có thể ngắm view sông thông thoáng.&nbsp;
- Được bàn giao kèm đầy đủ nội thất.&nbsp;
Căn hộ 2 phòng ngủ là sự lựa chọn hàng đầu dành cho các đôi vợ chồng trẻ, hộ gia đình từ 2-4 thành viên muốn tìm kiếm một chốn an cư để yên tâm lập nghiệp nơi thành phố đông đúc này.&nbsp;
Vị trí căn nhà nằm tại quận Bình Thạnh, cách chợ Vạn Kiếp 100m, thuận lợi di chuyển sang các quận như: quận 1, Phú Nhuận,...&nbsp;
Khu vực xung quanh có nhiều tiện ích như: chợ Vạn Kiếp, các hàng quán lớn nhỏ, trường học các cấp, khu ăn uống Vạn Kiếp, bưu điện, công viên văn hóa Phú Nhuận,...mang lại cuộc sống tiện ích cho cá nhân và cả gia đình.&nbsp;',2.8, 10,2,'03-23-2021',1,0,'Canhomieunoi1.jpg',2, 1),
(2,N'Căn hộ tầng 20 The Gold View ban công hướng Tây Bắc, đầy đủ nội thất',
N'BÁN CĂN HỘ THE GOLD VIEW TẦNG 20, QUẬN 4.&nbsp;
- Địa chỉ: Bến Vân Đồn, Phường 1, Quận 4, tp HCM.&nbsp;
- Diện tích: 80m2.&nbsp;
- Kết cấu: 2 phòng ngủ và 2 phòng tắm.&nbsp;
- Thiết kế hiện đại với tone màu sáng, nhà bàn giao nội thất đầy đủ hiện đại với sàn gỗ sang trọng cùng các trang thiết bị cần thiết khác.&nbsp;
- Ban công hướng Tây Bắc, View tầng cao đón gió thoáng mát.&nbsp;
Phòng khách và các phòng ngủ đều có mặt thoáng nhìn ra không gian bên ngoài, hỗ trợ hướng sáng và đón gió vào không gian bên trong căn hộ. Đây là một sự lựa chọn vô cùng hoàn hảo dành cho gia đình mới cưới, có không gian sinh hoạt ấm cúng và gần gũi. Gia chủ tương lai có thể tự mình bày trí và sắp xếp thêm nội thất và không gian sống tùy thuộc vào sở thích và nhu cầu của gia đình.&nbsp;
The Gold View là dự án quy mô lớn và được đầu tư bài bản và cao cấp nhất khu vực Quận 4 với quỹ đất hơn 2,3 ha với mật độ xây dựng 40%, còn lại là công viên cây xanh, hồ bơi, trung tâm thương mại.&nbsp;
Thiết kế 2 khối căn hộ: Tháp A – 33 tầng và Tháp B – 27 tầng.&nbsp;', 4.8, 10, 2,'09-05-2021',1,0,'Canhogold1.jpg',2, 1),
(2,N'Căn hộ First Home Thạnh Lộc, hướng Tây Bắc',
N'BÁN CĂN HỘ FIRST HOME THẠNH LỘC TẦNG 12A, QUẬN 12.&nbsp;
- Địa chỉ: Thạnh Lộc 27, Thạnh Lộc, Quận 12, tp HCM.&nbsp;
- Diện tích: 48m2.&nbsp;
- Kết cấu: 2 phòng ngủ và 1 phòng tắm.&nbsp;
- Thiết kế hiện đại, bàn giao nội thất cơ bản.&nbsp;
- Ban công hướng Tây Bắc, rộng rãi, view thoáng mát.&nbsp;
Căn hộ 2 phòng ngủ là sự lựa chọn hàng đầu dành cho các đôi vợ chồng trẻ, hộ gia đình từ 2-4 thành viên muốn tìm kiếm một chốn an cư để yên tâm lập nghiệp nơi thành phố đông đúc này.&nbsp;
Dự án căn hộ First Home – Thạnh Lộc nằm trên đường Vườn Lài nối dài, phường Thạnh Lộc quận 12, thành phố Hồ Chí Minh. Dự án được thi công bởi công ty Hangdong – Korea; công ty Shin Yeong, Korea chịu trách nhiệm giám sát và công ty ADU, Korea đảm nhận khâu thiết kế.&nbsp;', 1.4, 10, 3, '08-22-2021', 1, 0,'Canhofirst1.jpg',2, 1),
(2, N'Căn hộ chung cư Nguyễn Ngọc Phương view thành phố, hướng Đông.',
N'BÁN CĂN HỘ CHUNG CƯ NGUYỄN NGỌC PHƯƠNG TẦNG 6, BÌNH THẠNH.&nbsp;
- Địa chỉ: Nguyễn Ngọc Phương, Phường 19, Bình Thạnh, tp HCM.&nbsp;
- Diện tích: 68m2.&nbsp;
- Kết cấu: 2 phòng ngủ và 2 phòng tắm.&nbsp;
- Căn hộ ban công hướng Đông thoáng mát, view thành phố.&nbsp;
- Bàn giao kèm đầy đủ nội thất, nên có thể dọn vào ở ngay.&nbsp;
Căn hộ 2 phòng ngủ là sự lựa chọn hàng đầu dành cho các đôi vợ chồng trẻ, hộ gia đình từ 2-4 thành viên muốn tìm kiếm một chốn an cư để yên tâm lập nghiệp nơi thành phố đông đúc này.&nbsp;
Căn hộ Nguyễn Ngọc Phương hội tụ đầy đủ những tiện ích cho một khu chung cư hiện đại. Dự án tọa lạc ngay gần trung tâm thành phố, liền kề quận 1, giao thông thuận lợi, gần siêu thị, chợ, cây xăng, bệnh viện, trường học. Chung cư sở hữu vị trí đắc địa, khu dân trí cao, an ninh 24/24, 
tiện sang những quận trung tâm. Căn hộ gần chợ Thị Nghè, cầu Thị Nghè 1,2, ngay bờ kênh đường Trường Sa. Đi lại thuận tiện giữa các quận 1,3,2, Phú Nhuận.&nbsp;',3.3, 10, 3,'04-16-2021',1,0,'CanhoNguyenNgocPhuong1.jpg',2, 1),
(3,N'Shophouse Phú Mỹ Hưng Midtown',
N' Bán SHOP-HOUSE TẦNG 3 PHÚ MỸ HƯNG MIDTOWN, QUẬN 7.&nbsp;
- Địa chỉ: Nguyễn Lương Bằng Phường, Tân Phú, Quận 7, tp HCM.&nbsp;
- Diện tích: 54m2 (5m x 10.8m).&nbsp;
- Căn hộ được bàn giao thô nên phù hợp cho người kinh doanh có thể tự mình thiết kế và lên ý tưởng cửa hàng theo ý riêng của mình.&nbsp;
- Shophouse cửa hướng Bắc, ban công hướng Nam thoáng mát.&nbsp;
Shop-house tại Phú Mỹ Hưng Midtown hứa hẹn tiềm năng kinh doanh cho gia chủ tương lai khi dân cư tại đây đa số tập trung người có tri thức, địa vị cao trong xã hội và có trải nghiệm tốt. Bên cạnh đó, khu dân cư đông đúc, 
văn minh và điều kiện cho cơ sở vật chất tại đây sẽ là một lợi thế cho những người kinh doanh đa ngành nghề tại Phú Mỹ Hưng Midtown.&nbsp;
Đây là khu căn hộ tọa lạc tại vị trí đắc địa trên đường Nguyễn Lương Bằng, với đa dạng tiện ích nội khu như: hồ bơi, sauna, gym, trường học, phòng họp, khu vui chơi, học tập trẻ,... Cư dân tại khu căn hộ Phú Mỹ Hưng Midtown còn dễ dàng tiếp cận chuỗi dịch vụ – tiện ích đô thị hoàn thiện.
Nổi bật là hệ thống trường học quốc tế chỉ trong khoảng cách đi bộ: trường Nhật Bản, Hàn Quốc, Đài Bắc, Canada, Nam Sài Gòn,... cũng như các trung tâm thương mại sầm uất, bệnh viện quốc tế uy tín,..&nbsp;',4.8, 10, 4, '01-16-2021',1,0,'Shophouse1.jpg',2, 1),
(4,N'Bán nhà đẹp góc 2 mặt tiền đường 10m phường Hiệp Thành, Bình Chánh, tp Thủ Đức',
N'BÁN NHÀ 2 MẶT TIỀN Đường 10m Phường Hiệp Bình Phước Tp Thủ Đức.&nbsp;
Địa chỉ: Đường Số 15, Phường Hiệp Bình Phước, Thủ Đức, Hồ Chí Minh.&nbsp;
Diện tích: 75,3m2 (5x15). Dtsd 232,2 m... Một trệt + 3 lầu... Nhà thiết kế sang trọng...&nbsp;
Vị trí kinh doanh thuận lợi, đường nối chợ Hiệp Bình Phước, gần UBND, CA,YT,Trường học...&nbsp;
Thích hợp cho thuê, mở văn phòng, công ty, đầy đủ tiện ích xung quanh.&nbsp;
Sổ hồng hoàn công đầy đủ..&nbsp;', 6.7, 10, 2, '09-15-2021',1,0,'Officetel1.jpeg',2, 1)
GO
---------ĐẶC ĐIỂM BẤT ĐỘNG SẢN---------------------------------------------------------------------------

CREATE TABLE DACDIEM_BDS
(
	MADACDIEM_BDS INT IDENTITY PRIMARY KEY NOT NULL,
	MABDS INT FOREIGN KEY REFERENCES BDS(MABDS),						--CARE----------------------------------------------------------
	MALOAI_BDS INT FOREIGN KEY REFERENCES LOAIBDS(MALOAI),
	DIACHI NVARCHAR(100),
	DIENTICH NVARCHAR(30),
	SOPHONGNGU INT,
	SOPHONGTAM INT,
	SOGARA INT,
	PHAPLY NVARCHAR(50)
)
GO
INSERT INTO DACDIEM_BDS
VALUES
(1,1,N'QL13, đường 17 vào, phường Hiệp Bình Phước, Thủ Đức, tp HCM',N'121,3m2',1,3,1,N'Sổ đỏ/ Sổ hồng'),
(2,1,N'Đường 17, Phường Hiệp Bình Phước, Thủ Đức, tp Hồ Chí Minh',N'71m2 - 150m2', 3,4, 2,N'Sổ hồng'),
(3,1, N'Đường Quốc Lộ 13, Phường Hiệp Bình Phước, Thủ Đức, Hồ Chí Minh.',N'81m2.',1,4,2,N'Sổ hồng'),
(4,2,N'96 đường Trần Đại Nghĩa, Phường Tân Tạo A, Quận Bình Tân, Tp Hồ Chí Minh',N'62 m2',0, 2,2,N'Sổ hồng'),
(5,2,N'Vũ Huy Tấn, Phường 3, Bình Thạnh,tp HCM',N'57m2',0,2,1,N'Sổ hồng'),
(6,2,N'Bến Vân Đồn, Phường 1, Quận 4, tp HCM.',N'80 m2',0,2,2,N'Sổ hồng'),
(7,2,N'Thạnh Lộc 27, Thạnh Lộc, Quận 12, tp HCM.',N'48 m2',0,2,1,N'Sổ hồng'),
(8,2,N'Nguyễn Ngọc Phương, Phường 19, Bình Thạnh, tp HCM.',N'68 m2',0,2,2,N'Sổ hồng'),
(9,3,N' Nguyễn Lương Bằng Phường, Tân Phú, Quận 7, tp HCM.',N'54 m2',0,0,0,N'Sổ hồng'),
(10,4,N'Đường Số 15, Phường Hiệp Bình Phước, Thủ Đức, Hồ Chí Minh.',N'75,3 m2',0,3,3,N'Sổ hồng')
GO

CREATE TABLE HINH_ANH_BDS					   --MỘT BẤT ĐỘNG SẢN THÌ SẼ CÓ NHIỀU HÌNH HẢNH, TABLE NÀY DÙNG ĐỂ LƯU ẢNH CỦA 1 BDS
(
	MA_HINHANH INT IDENTITY PRIMARY KEY NOT NULL,
	MABDS INT FOREIGN KEY REFERENCES BDS(MABDS),
	HINHANH VARCHAR(100)
)
GO
INSERT INTO HINH_ANH_BDS 
VALUES
(1,N'Nharieng1.jpg'),
(1,N'Nharieng2.jpg'),
(1,N'Nharieng3.jpg'),
(2,N'Nhakhuonvien1.jpg'),
(2,N'Nhakhuonvien2.jpg'),
(2,N'Nhakhuonvien3.jpg'),
(2,N'Nhakhuonvien4.jpg'),
(3,N'Bannhaohoacthue1.jpg'),
(3,N'Bannhaohoacthue2.jpg'),
(3,N'Bannhaohoacthue3.jpg'),
(4,N'Canho1.jpg'),
(4,N'Canho2.jpg'),
(4,N'Canho3.jpg'),
(5,N'Canhomieunoi1.jpg'),
(5,N'Canhomieunoi2.jpg'),
(5,N'Canhomieunoi3.jpg'),
(5,N'Canhomieunoi4.jpg'),
(6,N'Canhogold1.jpg'),
(6,N'Canhogold2.jpg'),
(6,N'Canhogold3.jpg'),
(6,N'Canhogold4.jpg'),
(6,N'Canhogold5.jpg'),
(7,N'Canhofirst1.jpg'),
(7,N'Canhofirst2.jpg'),
(7,N'Canhofirst3.jpg'),
(8,N'CanhoNguyenNgocPhuong1.jpg'),
(8,N'CanhoNguyenNgocPhuong2.jpg'),
(8,N'CanhoNguyenNgocPhuong3.jpg'),
(8,N'CanhoNguyenNgocPhuong4.jpg'),
(8,N'CanhoNguyenNgocPhuong5.jpg'),
(9,N'Shophouse1.jpg'),
(9,N'Shophouse2.jpg'),
(9,N'Shophouse3.jpg'),
(9,N'Shophouse4.jpg'),
(10,N'Officetel1.jpeg'),
(10,N'Officetel2.jpeg'),
(10,N'Officetel3.jpeg'),
(10,N'Officetel4.jpeg')
GO
CREATE TABLE DONDATCOC                         --ĐƠN ĐẶT CỌC
(
	MADON INT IDENTITY PRIMARY KEY NOT NULL,
	NGAYDAT DATE DEFAULT GETDATE(),
	NGAYHT DATE NULL,
	DUYET INT CHECK(DUYET IN(0,1)) DEFAULT 0 NOT NULL           --MẶC ĐỊNH DUYET = 0: CHƯA DUYỆT, DUYET = 1: ĐƠN ĐẶT CỌC ĐÃ DUYỆT  
)																--(TRONG PHẦN "QUẢN LÝ ĐƠN ĐẶT CỌC", NẾU MUỐN DUYỆT THÌ ĐIỀN THÔNG TIN NGÀY GIAO, SAU ĐÓ NHẤN XÁC NHẬN DUYỆT, ĐƠN SẼ ĐƯỢC CHUYỂN SANG TRANG ĐÃ DUYỆT)
GO
  --data mẫu

CREATE TABLE CHITIETDONDATCOC                  --CHI TIẾT ĐƠN ĐẶT CỌC
(
	MACT INT IDENTITY PRIMARY KEY NOT NULL,
	MADON INT FOREIGN KEY REFERENCES DONDATCOC(MADON),
	MATV INT FOREIGN KEY REFERENCES THANHVIEN(MATV),
	MABDS INT FOREIGN KEY REFERENCES BDS(MABDS),
	MALOAI INT FOREIGN KEY REFERENCES LOAIBDS(MALOAI),
	TONGTIEN DECIMAL(10,2) check(TONGTIEN>0),
	TINHTRANG_THANHTOAN INT CHECK(TINHTRANG_THANHTOAN IN(0,1)) DEFAULT 0 NOT NULL,            --KIỂM TRA XEM TÌNH TRẠNG CỦA ĐƠN ĐẶT CỌC --ĐÃ THANH TOÁN HAY CHƯA, 0: CHƯA THANH TOÁN
	STATE_DELETE INT DEFAULT 1
)																							   
GO
----------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------
CREATE TABLE BINHLUANDANHGIA                   --BÌNH LUẬN ĐÁNH GIÁ, PHỤC VỤ CHO VIỆC LƯU THÔNG TIN ĐÁNH GIÁ CỦA KHÁCH HÀNG 
(											   --BAO GỒM: SỐ SAO, ĐOẠN VĂN BẢN
	MABL INT IDENTITY PRIMARY KEY NOT NULL,
	MABDS INT FOREIGN KEY REFERENCES BDS(MABDS),
	MATV INT FOREIGN KEY REFERENCES THANHVIEN(MATV),
	HOTEN NVARCHAR(50) NOT NULL,
	SOSAO INT,
	NOIDUNG NVARCHAR(1000),
	NGAYBL DATETIME DEFAULT GETDATE(),                    -- NGÀY BÌNH LUẬN
	EMAIL VARCHAR(50) NOT NULL,
	STATUS_DELETE INT DEFAULT 1
)
GO
CREATE TABLE ADMIN_REPLY                       --PHẢN HỒI TIN NHẮN TỪ KHÁCH HÀNG
(
	MAREPLY INT IDENTITY PRIMARY KEY NOT NULL,
	MABL INT FOREIGN KEY REFERENCES BINHLUANDANHGIA(MABL),
	MABDS INT FOREIGN KEY REFERENCES BDS(MABDS),
	MANV INT FOREIGN KEY REFERENCES NHANVIEN(MANV),
	NOIDUNG NVARCHAR(1000),
	NGAYTRALOI DATETIME DEFAULT GETDATE()
)
GO
----------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------
CREATE TABLE TINNHAN_MAIL                      --TIN NHẮN TỪ KHÁCH HÀNG THÔNG QUA TRANG PHẢN HỒI
(
	MATINNHAN INT IDENTITY PRIMARY KEY NOT NULL,
	HOTEN NVARCHAR(30) NOT NULL,
	TIEUDE NVARCHAR(100) NOT NULL,
	NOIDUNG NVARCHAR(2000) NOT NULL,
	EMAIL VARCHAR(50) NOT NULL,
	NGAYNHAN DATE DEFAULT GETDATE(),
	HINHANH VARCHAR(100),
	TINHTRANG_PHANHOI INT CHECK(TINHTRANG_PHANHOI IN(0,1)) DEFAULT 0       --KIỂM TRA TÌNH TRẠNG TIN NHẮN ĐÃ ĐƯỢC PHẢN HỒI HAY CHƯA
)																	       --MẶC ĐỊNH 0: CHƯA PHẢN HỒI
GO
CREATE TABLE REPLY_TINNHAN_MAIL				   --PHẢN HỒI TIN NHẮN CỦA KHÁCH HÀNG THÔNG QUA MAIL
(
	MA_REPLY_MAIL INT IDENTITY PRIMARY KEY NOT NULL,
	MATINNHAN INT FOREIGN KEY REFERENCES TINNHAN_MAIL(MATINNHAN),
	TIEUDE NVARCHAR(100) NOT NULL,
	NOIDUNG NVARCHAR(2000) NOT NULL,
	NGAYTRALOI DATE DEFAULT GETDATE()
)
GO
--DROP TABLE REPLY_TINNHAN_MAIL	
--GO
--DROP TABLE TINNHAN_MAIL
--GO
INSERT INTO DONDATCOC 
VALUES
('12-16-2021', '11/24/2021', 0) 
INSERT INTO CHITIETDONDATCOC
VALUES
(1, 2, 1, 1, 0.66, 0, 1) 