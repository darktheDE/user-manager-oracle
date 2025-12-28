# 📚 UserManager - Quản Lý Người Dùng Oracle Database

## 🎯 Mô Tả
Ứng dụng WinForms C# .NET 8 cho phép quản lý người dùng trên Oracle Database. Được xây dựng theo kiến trúc 3 lớp (3-Layer Architecture) với Passive MVP Pattern.

## 🗂️ Cấu Trúc Thư Mục

```
📦 UserManager/
│
├── 📄 UserManager.csproj           # Project file
├── 📄 Program.cs                   # Entry point
├── 📄 appsettings.json             # Cấu hình ứng dụng
│
├── 📁 Common/                      # Các tiện ích dùng chung
│   ├── 📁 Constants/
│   │   └── PrivilegeConstants.cs   # Hằng số: System/Object Privileges, Status...
│   │
│   └── 📁 Helpers/
│       ├── ConfigHelper.cs         # Đọc cấu hình từ appsettings.json
│       ├── MessageHelper.cs        # Hiển thị MessageBox
│       └── PasswordHelper.cs       # Mã hóa password (SHA256)
│
├── 📁 Models/                      # Các lớp Model (Entity)
│   └── EntityModels.cs             # UserModel, RoleModel, ProfileModel...
│
├── 📁 DAL/                         # Data Access Layer
│   ├── OracleConnectionManager.cs  # Quản lý kết nối Oracle (Singleton)
│   ├── BaseRepository.cs           # Base class cho các Repository
│   │
│   └── 📁 Repositories/
│       ├── UserRepository.cs       # CRUD Oracle Users
│       ├── RoleRepository.cs       # CRUD Oracle Roles
│       ├── ProfileRepository.cs    # CRUD Oracle Profiles
│       ├── PrivilegeRepository.cs  # Grant/Revoke Privileges
│       ├── TablespaceRepository.cs # Query Tablespaces
│       └── UserInfoRepository.cs   # CRUD bảng USER_INFO (thông tin bổ sung)
│
├── 📁 BLL/                         # Business Logic Layer
│   └── 📁 Services/
│       ├── AuthService.cs          # Đăng nhập, session, kiểm tra quyền
│       ├── UserService.cs          # Logic nghiệp vụ User
│       ├── RoleService.cs          # Logic nghiệp vụ Role
│       ├── ProfileService.cs       # Logic nghiệp vụ Profile
│       ├── PrivilegeService.cs     # Logic nghiệp vụ Privilege
│       └── TablespaceService.cs    # Truy vấn Tablespace
│
├── 📁 GUI/                         # Presentation Layer (UI)
│   ├── 📁 Forms/
│   │   ├── LoginForm.cs            # Form đăng nhập
│   │   ├── LoginForm.Designer.cs
│   │   ├── MainForm.cs             # Form chính (Menu, Navigation)
│   │   └── MainForm.Designer.cs
│   │
│   └── 📁 UserControls/
│       ├── UserListControl.cs      # Danh sách Users
│       ├── RoleListControl.cs      # Danh sách Roles
│       ├── ProfileListControl.cs   # Danh sách Profiles
│       ├── PrivilegeListControl.cs # Danh sách Privileges
│       └── UserInfoListControl.cs  # Danh sách thông tin cá nhân
│
└── 📁 Resources/                   # Tài nguyên (icons, images...)
```

## 🏗️ Kiến Trúc 3 Lớp

```
┌─────────────────────────────────────────────────────────────┐
│                   PRESENTATION LAYER (GUI)                  │
│              Forms, UserControls, MessageBox                │
│     • Nhận input từ user, hiển thị kết quả                  │
│     • KHÔNG chứa logic nghiệp vụ                            │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   BUSINESS LAYER (BLL)                      │
│                      Services                               │
│     • Validate dữ liệu                                      │
│     • Kiểm tra quyền hạn                                    │
│     • Xử lý các chức năng chính                             │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    DATA LAYER (DAL)                         │
│               Repositories, ConnectionManager               │
│     • Kết nối Oracle DB                                     │
│     • Thực thi SQL/PL-SQL                                   │
│     • Truy vấn System Catalog                               │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 Công Nghệ

| Thành phần | Công nghệ |
|------------|-----------|
| **Frontend** | C# WinForms (.NET 8) |
| **Database** | Oracle Database |
| **Data Access** | Oracle.ManagedDataAccess.Core |
| **Architecture** | 3-Layer + MVP |
| **Security** | Password Hashing (SHA256) |

## 🚀 Cách Chạy

### 1. Cấu hình Connection String
Mở file `appsettings.json` và cập nhật:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCLPDB1)));User Id=SYSTEM;Password=YourPassword;"
  }
}
```

### 2. Tạo Database Schema
Chạy script `database_schema.sql` hoặc `database_erd.sql` trên Oracle Database.

### 3. Build và Run
```bash
dotnet restore
dotnet build
dotnet run
```

Hoặc mở solution trong Visual Studio và nhấn F5.

## 📋 Các Chức Năng

### ✅ Đã Triển Khai
- [x] Đăng nhập/Đăng xuất
- [x] Xem danh sách Users
- [x] Lock/Unlock User
- [x] Xóa User
- [x] Xem danh sách Roles
- [x] Xem danh sách Profiles
- [x] Xem danh sách System Privileges
- [x] Revoke System Privilege
- [x] Xem thông tin cá nhân bổ sung

### 🔄 Đang Phát Triển
- [ ] Form thêm/sửa User
- [ ] Form thêm/sửa Role
- [ ] Form thêm/sửa Profile
- [ ] Form Grant Privilege
- [ ] Form đổi mật khẩu
- [ ] Báo cáo thông tin User đầy đủ
- [ ] Báo cáo Tablespace

## 👥 Phân Quyền

| Vai trò | Quyền hạn |
|---------|-----------|
| **Admin (DBA)** | Xem/thêm/sửa/xóa Users, Roles, Profiles, Grant/Revoke |
| **User thường** | Chỉ xem thông tin của chính mình, đổi mật khẩu |

## 📝 Ghi Chú

- Ứng dụng sử dụng **Oracle System Catalog** (DBA_USERS, DBA_ROLES, DBA_PROFILES, DBA_SYS_PRIVS, DBA_TAB_PRIVS...) để truy vấn thông tin
- Password được mã hóa bằng **SHA256** trước khi lưu
- Session được quản lý qua `AuthService.CurrentSession`
- Tất cả các hành động đều kiểm tra quyền qua `AuthService.HasPrivilege()`

