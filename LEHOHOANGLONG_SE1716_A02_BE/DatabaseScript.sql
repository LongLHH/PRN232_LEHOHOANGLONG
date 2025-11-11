-- =============================================
-- FU News Management System Database Script
-- Student: Lê Hồ Hoàng Long (SE181754)
-- PHIÊN BẢN ĐÃ SỬA LỖI
-- =============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FUNewsManagementSystem')
BEGIN
    ALTER DATABASE FUNewsManagementSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FUNewsManagementSystem;
    PRINT 'Dropped existing FUNewsManagementSystem database.';
END
GO

-- Create database
CREATE DATABASE FUNewsManagementSystem;
GO

USE FUNewsManagementSystem;
GO

-- =============================================
-- Create Tables
-- =============================================

-- SystemAccount Table
-- SỬA 1: Đổi CHECK constraint về (1, 2) để khớp với yêu cầu đề bài
[cite_start]-- (Admin không lưu trong DB) [cite: 29]
CREATE TABLE SystemAccount (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    AccountName NVARCHAR(100) NOT NULL,
    AccountEmail NVARCHAR(100) NOT NULL UNIQUE,
    AccountPassword NVARCHAR(255) NOT NULL,
    AccountRole INT NOT NULL CHECK (AccountRole IN (1, 2)) -- 1=Staff, 2=Lecturer
);
GO

-- Category Table
-- SỬA 2: Thêm các cột bị thiếu (ParentCategoryID, IsActive)
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    CategoryDescription NVARCHAR(500) NULL,
    ParentCategoryID INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Category_ParentCategory FOREIGN KEY (ParentCategoryID) REFERENCES Category(CategoryID)
);
GO

-- Tag Table (Giữ nguyên)
CREATE TABLE Tag (
    TagID INT IDENTITY(1,1) PRIMARY KEY,
    TagName NVARCHAR(50) NOT NULL UNIQUE,
    Note NVARCHAR(200) NULL
);
GO

-- NewsArticle Table
-- SỬA 3: Đổi NewsArticleId thành INT IDENTITY (rất quan trọng)
-- Và sửa tên các cột cho khớp với file .docx (ví dụ: CategoryDesciption)
CREATE TABLE NewsArticle (
    NewsArticleID INT IDENTITY(1,1) PRIMARY KEY,
    NewsTitle NVARCHAR(200) NOT NULL,
    Headline NVARCHAR(500) NULL,
    NewsContent NVARCHAR(MAX) NOT NULL,
    NewsSource NVARCHAR(255) NULL,
    CategoryID INT NOT NULL,
    NewsStatus BIT NOT NULL DEFAULT 1,
    CreatedByID INT NOT NULL,
    UpdatedByID INT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedDate DATETIME NULL,
    CONSTRAINT FK_NewsArticle_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
    CONSTRAINT FK_NewsArticle_CreatedBy FOREIGN KEY (CreatedByID) REFERENCES SystemAccount(AccountID),
    CONSTRAINT FK_NewsArticle_UpdatedBy FOREIGN KEY (UpdatedByID) REFERENCES SystemAccount(AccountID)
);
GO

-- THÊM MỚI: Bảng NewsTag (bị thiếu)
CREATE TABLE NewsTag (
    NewsArticleID INT NOT NULL,
    TagID INT NOT NULL,
    PRIMARY KEY (NewsArticleID, TagID),
    CONSTRAINT FK_NewsTag_NewsArticle FOREIGN KEY (NewsArticleID) REFERENCES NewsArticle(NewsArticleID),
    CONSTRAINT FK_NewsTag_Tag FOREIGN KEY (TagID) REFERENCES Tag(TagID)
);
GO


-- =============================================
-- INSERT DỮ LIỆU MẪU
-- =============================================

/*
--- Password tất cả accounts: 123456 (BCrypt hash) ---
--- Admin không insert vì lấy từ appsettings.json ---
*/

-- 1. INSERT SystemAccount (Staff và Lecturer)
PRINT 'Inserting SystemAccounts...';
INSERT INTO SystemAccount (AccountName, AccountEmail, AccountPassword, AccountRole)
VALUES 
    -- Staff accounts (Role = 1)
    (N'Nguyễn Văn An', 'staff1@funews.edu.vn', '$2a$11$vK3Zz3Zz3Zz3Zz3Zz3Zz3uO7K3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Z', 1),
    (N'Trần Thị Bình', 'staff2@funews.edu.vn', '$2a$11$vK3Zz3Zz3Zz3Zz3Zz3Zz3uO7K3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Z', 1),
    (N'Lê Minh Cường', 'staff3@funews.edu.vn', '$2a$11$vK3Zz3Zz3Zz3Zz3Zz3Zz3uO7K3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Z', 1),
    
    -- Lecturer accounts (Role = 2)
    (N'Phạm Thị Dung', 'lecturer1@funews.edu.vn', '$2a$11$vK3Zz3Zz3Zz3Zz3Zz3Zz3uO7K3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Z', 2),
    (N'Hoàng Văn Em', 'lecturer2@funews.edu.vn', '$2a$11$vK3Zz3Zz3Zz3Zz3Zz3Zz3uO7K3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Z', 2),
    (N'Vũ Thị Phương', 'lecturer3@funews.edu.vn', '$2a$11$vK3Zz3Zz3Zz3Zz3Zz3Zz3uO7K3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Z', 2),
    (N'Đặng Minh Giang', 'lecturer4@funews.edu.vn', '$2a$11$vK3Zz3Zz3Zz3Zz3Zz3Zz3uO7K3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Zz3Z', 2);
GO

-- 2. INSERT Category (Danh mục tin tức)
-- (Sửa lỗi: Thêm ParentCategoryID và IsActive)
PRINT 'Inserting Categories...';
INSERT INTO Category (CategoryName, CategoryDescription, ParentCategoryID, IsActive)
VALUES 
    -- Danh mục cha
    (N'Tin tức FPT', N'Các tin tức chung về FPT University', NULL, 1), -- ID = 1
    (N'Học tập', N'Thông tin về học tập, đào tạo', NULL, 1), -- ID = 2
    (N'Sinh viên', N'Hoạt động sinh viên, câu lạc bộ', NULL, 1), -- ID = 3
    (N'Sự kiện', N'Các sự kiện, hội thảo', NULL, 1), -- ID = 4
    (N'Tuyển sinh', N'Thông tin tuyển sinh', NULL, 1), -- ID = 5
    
    -- Danh mục con (có ParentCategoryID)
    (N'Thông báo học vụ', N'Các thông báo liên quan học vụ', 2, 1), -- ID = 6
    (N'Lịch thi', N'Lịch thi, lịch học', 2, 1), -- ID = 7
    (N'CLB Công nghệ', N'Hoạt động các CLB công nghệ', 3, 1), -- ID = 8
    (N'Hội thảo Khoa học', N'Hội thảo, seminar khoa học', 4, 1), -- ID = 9
    (N'Tuyển sinh Đại học', N'Thông tin tuyển sinh bậc ĐH', 5, 1); -- ID = 10
GO

-- 3. INSERT Tag (Thẻ)
PRINT 'Inserting Tags...';
INSERT INTO Tag (TagName, Note)
VALUES 
    (N'Công nghệ', N'Tin tức về công nghệ'),
    (N'AI', N'Trí tuệ nhân tạo'),
    (N'Blockchain', N'Công nghệ blockchain'),
    (N'IoT', N'Internet of Things'),
    (N'Học bổng', N'Thông tin học bổng'),
    (N'Du học', N'Cơ hội du học'),
    (N'Thực tập', N'Cơ hội thực tập'),
    (N'Tuyển dụng', N'Thông tin tuyển dụng'),
    (N'Nghiên cứu', N'Nghiên cứu khoa học'),
    (N'Cuộc thi', N'Các cuộc thi sinh viên'),
    (N'Workshop', N'Hội thảo, workshop'),
    (N'Hackathon', N'Cuộc thi lập trình'),
    (N'Startup', N'Khởi nghiệp'),
    (N'Career', N'Hướng nghiệp'),
    (N'Alumni', N'Cựu sinh viên');
GO

-- 4. INSERT NewsArticle (Bài viết)
PRINT 'Inserting NewsArticles...';

-- SỬA 4: Dùng biến để lấy ID cho an toàn, thay vì mã hóa cứng
-- Lấy ID của các accounts
DECLARE @Staff1 INT = (SELECT AccountID FROM SystemAccount WHERE AccountEmail = 'staff1@funews.edu.vn');
DECLARE @Staff2 INT = (SELECT AccountID FROM SystemAccount WHERE AccountEmail = 'staff2@funews.edu.vn');
DECLARE @Staff3 INT = (SELECT AccountID FROM SystemAccount WHERE AccountEmail = 'staff3@funews.edu.vn');
DECLARE @Lecturer1 INT = (SELECT AccountID FROM SystemAccount WHERE AccountEmail = 'lecturer1@funews.edu.vn');
DECLARE @Lecturer2 INT = (SELECT AccountID FROM SystemAccount WHERE AccountEmail = 'lecturer2@funews.edu.vn');
DECLARE @Lecturer3 INT = (SELECT AccountID FROM SystemAccount WHERE AccountEmail = 'lecturer3@funews.edu.vn');
DECLARE @Lecturer4 INT = (SELECT AccountID FROM SystemAccount WHERE AccountEmail = 'lecturer4@funews.edu.vn');

-- Lấy ID của các categories
DECLARE @CatFPT INT = (SELECT CategoryID FROM Category WHERE CategoryName = N'Tin tức FPT');
DECLARE @CatLichThi INT = (SELECT CategoryID FROM Category WHERE CategoryName = N'Lịch thi');
DECLARE @CatSinhVien INT = (SELECT CategoryID FROM Category WHERE CategoryName = N'Sinh viên');
DECLARE @CatHoiThao INT = (SELECT CategoryID FROM Category WHERE CategoryName = N'Hội thảo Khoa học');
DECLARE @CatCLB INT = (SELECT CategoryID FROM Category WHERE CategoryName = N'CLB Công nghệ');
DECLARE @CatTuyenSinhDH INT = (SELECT CategoryID FROM Category WHERE CategoryName = N'Tuyển sinh Đại học');
DECLARE @CatSuKien INT = (SELECT CategoryID FROM Category WHERE CategoryName = N'Sự kiện');


INSERT INTO NewsArticle (NewsTitle, Headline, CreatedDate, NewsContent, NewsSource, CategoryID, NewsStatus, CreatedByID, UpdatedByID, ModifiedDate)
VALUES 
    -- Bài viết của Staff
    (N'FPT University mở rộng hợp tác quốc tế', 
     N'Ký kết biên bản ghi nhớ với 5 trường đại học hàng đầu châu Á',
     '2024-01-15 09:00:00',
     N'FPT University vừa chính thức ký kết...',
     N'FPT News',
     @CatFPT, -- Sửa
     1, 
     @Staff1, 
     NULL,
     NULL),
    
    (N'Thông báo lịch thi giữa kỳ học kỳ Fall 2024',
     N'Lịch thi GK cho tất cả các khối ngành từ ngày 15/03 đến 25/03',
     '2024-02-01 08:00:00',
     N'Phòng Đào tạo thông báo lịch thi giữa kỳ...',
     N'Phòng Đào tạo',
     @CatLichThi, -- Sửa
     1,
     @Staff2,
     @Staff2,
     '2024-02-05 14:30:00'),
    
    (N'Học bổng xuất sắc học kỳ Spring 2024',
     N'200 suất học bổng toàn phần và bán phần...',
     '2024-01-20 10:00:00',
     N'FPT University công bố danh sách 200 sinh viên...',
     N'Phòng CTSV',
     @CatSinhVien, -- Sửa
     1,
     @Staff1,
     NULL,
     NULL),
    
    -- Bài viết của Lecturer
    (N'Hội thảo "AI trong Y tế" - Cơ hội và Thách thức',
     N'Diễn giả: GS.TS Nguyễn Văn A - Chuyên gia AI hàng đầu Việt Nam',
     '2024-02-10 09:30:00',
     N'Khoa Công nghệ Thông tin tổ chức hội thảo...',
     N'Khoa CNTT',
     @CatHoiThao, -- Sửa
     1, 
     @Lecturer1,
     NULL,
     NULL),
    
    (N'Cuộc thi Hackathon 2024 - "Code for Future"',
     N'Giải thưởng tổng trị giá 500 triệu đồng',
     '2024-02-15 08:00:00',
     N'FPT University phối hợp với các doanh nghiệp...',
     N'BTC Hackathon',
     @CatSuKien, -- Sửa (Lấy ID của "Sự kiện" thay vì "Cuộc thi")
     1,
     @Lecturer2,
     @Lecturer2,
     '2024-02-18 16:00:00'),
    
    (N'Chương trình Thực tập tại Nhật Bản 2024',
     N'30 suất thực tập hưởng lương tại các công ty IT Nhật Bản',
     '2024-01-25 09:00:00',
     N'FPT University mở đơn đăng ký chương trình...',
     N'Phòng Hợp tác Quốc tế',
     @CatSinhVien, -- Sửa
     1,
     @Lecturer3,
     NULL,
     NULL),
    
    (N'Workshop "Blockchain và ứng dụng thực tế"',
     N'Thực hành xây dựng smart contract trên Ethereum',
     '2024-02-20 10:00:00',
     N'CLB Blockchain FPT tổ chức workshop...',
     N'CLB Blockchain',
     @CatCLB, -- Sửa
     1,
     @Lecturer4,
     NULL,
     NULL),
    
    -- Thêm bài viết inactive (để test)
    (N'Thông báo bảo trì hệ thống',
     N'Hệ thống FAP tạm ngưng từ 23h đến 2h sáng',
     '2024-02-05 18:00:00',
     N'Trung tâm IT thông báo bảo trì nâng cấp...',
     N'Trung tâm IT',
     @CatFPT, -- Sửa
     0, -- Inactive
     @Staff2,
     NULL,
     NULL),
    
    (N'Tuyển sinh Thạc sĩ Khoa học Máy tính 2024',
     N'Nhận hồ sơ đến hết ngày 30/03/2024',
     '2024-02-25 09:00:00',
     N'FPT University thông báo tuyển sinh chương trình...',
     N'Phòng Tuyển sinh',
     @CatTuyenSinhDH, -- Sửa
     1,
     @Staff3,
     NULL,
     NULL),
    
    (N'Ngày hội việc làm FPT Career Day 2024',
     N'Hơn 100 doanh nghiệp tham gia tuyển dụng',
     '2024-03-01 08:00:00',
     N'FPT Career Day 2024 sẽ diễn ra...',
     N'Trung tâm Hỗ trợ SV',
     @CatSinhVien, -- Sửa
     1,
     @Staff1,
     NULL,
     NULL);
GO

-- 5. INSERT NewsTag (Gắn tag cho bài viết)
PRINT 'Inserting NewsTags...';

-- SỬA 5: Dùng biến để lấy ID cho an toàn
-- Lấy ID bài viết
DECLARE @Article1 INT = (SELECT NewsArticleID FROM NewsArticle WHERE NewsTitle = N'FPT University mở rộng hợp tác quốc tế');
DECLARE @Article3 INT = (SELECT NewsArticleID FROM NewsArticle WHERE NewsTitle = N'Học bổng xuất sắc học kỳ Spring 2024');
DECLARE @Article4 INT = (SELECT NewsArticleID FROM NewsArticle WHERE NewsTitle = N'Hội thảo "AI trong Y tế" - Cơ hội và Thách thức');
DECLARE @Article5 INT = (SELECT NewsArticleID FROM NewsArticle WHERE NewsTitle = N'Cuộc thi Hackathon 2024 - "Code for Future"');
DECLARE @Article6 INT = (SELECT NewsArticleID FROM NewsArticle WHERE NewsTitle = N'Chương trình Thực tập tại Nhật Bản 2024');
DECLARE @Article7 INT = (SELECT NewsArticleID FROM NewsArticle WHERE NewsTitle = N'Workshop "Blockchain và ứng dụng thực tế"');
DECLARE @Article9 INT = (SELECT NewsArticleID FROM NewsArticle WHERE NewsTitle = N'Tuyển sinh Thạc sĩ Khoa học Máy tính 2024');
DECLARE @Article10 INT = (SELECT NewsArticleID FROM NewsArticle WHERE NewsTitle = N'Ngày hội việc làm FPT Career Day 2024');

-- Lấy ID tags
DECLARE @TagCongNghe INT = (SELECT TagID FROM Tag WHERE TagName = N'Công nghệ');
DECLARE @TagAI INT = (SELECT TagID FROM Tag WHERE TagName = N'AI');
DECLARE @TagBlockchain INT = (SELECT TagID FROM Tag WHERE TagName = N'Blockchain');
DECLARE @TagHocBong INT = (SELECT TagID FROM Tag WHERE TagName = N'Học bổng');
DECLARE @TagDuHoc INT = (SELECT TagID FROM Tag WHERE TagName = N'Du học');
DECLARE @TagThucTap INT = (SELECT TagID FROM Tag WHERE TagName = N'Thực tập');
DECLARE @TagTuyenDung INT = (SELECT TagID FROM Tag WHERE TagName = N'Tuyển dụng');
DECLARE @TagNghienCuu INT = (SELECT TagID FROM Tag WHERE TagName = N'Nghiên cứu');
DECLARE @TagCuocThi INT = (SELECT TagID FROM Tag WHERE TagName = N'Cuộc thi');
DECLARE @TagWorkshop INT = (SELECT TagID FROM Tag WHERE TagName = N'Workshop');
DECLARE @TagHackathon INT = (SELECT TagID FROM Tag WHERE TagName = N'Hackathon');
DECLARE @TagCareer INT = (SELECT TagID FROM Tag WHERE TagName = N'Career');
DECLARE @TagAlumni INT = (SELECT TagID FROM Tag WHERE TagName = N'Alumni');


INSERT INTO NewsTag (NewsArticleID, TagID)
VALUES 
    -- Bài 1: FPT University mở rộng hợp tác quốc tế
    (@Article1, @TagDuHoc), 
    
    -- Bài 3: Học bổng xuất sắc
    (@Article3, @TagHocBong), 
    
    -- Bài 4: Hội thảo AI trong Y tế
    (@Article4, @TagAI), 
    (@Article4, @TagNghienCuu), 
    (@Article4, @TagWorkshop), 
    
    -- Bài 5: Cuộc thi Hackathon
    (@Article5, @TagCongNghe), 
    (@Article5, @TagCuocThi), 
    (@Article5, @TagHackathon), 
    
    -- Bài 6: Thực tập Nhật Bản
    (@Article6, @TagDuHoc), 
    (@Article6, @TagThucTap), 
    (@Article6, @TagCareer), 
    
    -- Bài 7: Workshop Blockchain
    (@Article7, @TagBlockchain), 
    (@Article7, @TagCongNghe), 
    (@Article7, @TagWorkshop), 
    
    -- Bài 9: Tuyển sinh Thạc sĩ
    (@Article9, @TagAI), 
    (@Article9, @TagNghienCuu), 
    
    -- Bài 10: Career Day
    (@Article10, @TagTuyenDung), 
    (@Article10, @TagCareer), 
    (@Article10, @TagAlumni);
GO

/*
--- THỐNG KÊ DỮ LIỆU ---
*/
PRINT '';
PRINT '========================================';
PRINT 'DATA INSERTED SUCCESSFULLY!';
PRINT '========================================';
PRINT '';
PRINT 'Statistics:';
PRINT '- SystemAccounts: ' + CAST((SELECT COUNT(*) FROM SystemAccount) AS VARCHAR);
PRINT '  + Staff (Role=1): ' + CAST((SELECT COUNT(*) FROM SystemAccount WHERE AccountRole = 1) AS VARCHAR);
PRINT '  + Lecturer (Role=2): ' + CAST((SELECT COUNT(*) FROM SystemAccount WHERE AccountRole = 2) AS VARCHAR);
PRINT '- Categories: ' + CAST((SELECT COUNT(*) FROM Category) AS VARCHAR);
PRINT '- Tags: ' + CAST((SELECT COUNT(*) FROM Tag) AS VARCHAR);
PRINT '- NewsArticles: ' + CAST((SELECT COUNT(*) FROM NewsArticle) AS VARCHAR);
PRINT '  + Active: ' + CAST((SELECT COUNT(*) FROM NewsArticle WHERE NewsStatus = 1) AS VARCHAR);
PRINT '  + Inactive: ' + CAST((SELECT COUNT(*) FROM NewsArticle WHERE NewsStatus = 0) AS VARCHAR);
PRINT '- NewsTags (relationships): ' + CAST((SELECT COUNT(*) FROM NewsTag) AS VARCHAR);
PRINT '';
PRINT 'Test Accounts:';
PRINT '- Admin: admin@FUNewsManagementSystem.org / @@abc123@@ (from appsettings.json)';
PRINT '- Staff: staff1@funews.edu.vn / 123456';
PRINT '- Lecturer: lecturer1@funews.edu.vn / 123456';
PRINT '';
PRINT '========================================';
GO

-- Hiển thị sample data
SELECT TOP 5 
    na.NewsArticleID,
    na.NewsTitle,
    c.CategoryName,
    sa.AccountName AS Author,
    na.NewsStatus,
    na.CreatedDate
FROM NewsArticle na
JOIN Category c ON na.CategoryID = c.CategoryID
JOIN SystemAccount sa ON na.CreatedByID = sa.AccountID
ORDER BY na.CreatedDate DESC;
GO