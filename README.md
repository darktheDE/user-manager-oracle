# 🗂️ Oracle User Manager

Ứng dụng quản lý người dùng Oracle Database - Windows Forms .NET 8.0

## 📋 Mục lục

1. [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
2. [Cài đặt Database](#cài-đặt-database)
3. [Cài đặt ứng dụng](#cài-đặt-ứng-dụng)
4. [Cấu hình](#cấu-hình)
5. [Chạy ứng dụng](#chạy-ứng-dụng)
6. [Tính năng](#tính-năng)
7. [Stored Procedures](#stored-procedures)
8. [Phân quyền Oracle](#phân-quyền-oracle)
9. [Cấu trúc project](#cấu-trúc-project)

---

## 🔧 Yêu cầu hệ thống

### Phần mềm cần thiết

| Phần mềm | Phiên bản | Bắt buộc |
|----------|-----------|----------|
| .NET SDK | 8.0+ | ✅ |
| Docker Desktop | Latest | ✅ |
| Visual Studio / VS Code | 2022+ | Khuyến nghị |
| Git | Latest | Khuyến nghị |

### Phần cứng

- **RAM:** Tối thiểu 8GB (khuyến nghị 16GB)
- **Disk:** Tối thiểu 10GB trống
- **OS:** Windows 10/11 64-bit

---

## 🐳 Cài đặt Database

### 1. Chạy Oracle 23ai Free trên Docker

```bash
# Pull image Oracle 23ai Free
docker pull container-registry.oracle.com/database/free:latest

# Tạo và chạy container
docker run -d --name oracle-23ai \
  -p 1521:1521 \
  -e ORACLE_PWD=YourStrongPassword123 \
  container-registry.oracle.com/database/free:latest
```

### 2. Đợi database khởi động (khoảng 2-5 phút)

```bash
# Kiểm tra logs
docker logs -f oracle-23ai

# Đợi thấy dòng: "DATABASE IS READY TO USE!"
```

### 3. Cấu hình Stored Procedures

```bash
# Copy file stored_procedures.sql vào container
docker cp stored_procedures.sql oracle-23ai:/tmp/

# Chạy script
docker exec -it oracle-23ai sqlplus SYSTEM/YourStrongPassword123@FREEPDB1 @/tmp/stored_procedures.sql
```

### 4. Tạo bảng USER_INFO (tùy chọn)

```bash
docker cp create_userinfo_table.sql oracle-23ai:/tmp/
docker exec -it oracle-23ai sqlplus SYSTEM/YourStrongPassword123@FREEPDB1 @/tmp/create_userinfo_table.sql
```

---

## 💻 Cài đặt ứng dụng

### 1. Clone repository

```bash
git clone https://github.com/your-username/UserManager.git
cd UserManager
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. Build project

```bash
dotnet build
```

---

## ⚙️ Cấu hình

### File cấu hình: `UserManager/appsettings.json`

```json
{
  "OracleSettings": {
    "Host": "localhost",
    "Port": 1521,
    "ServiceName": "FREEPDB1"
  }
}
```

### Biến môi trường (tùy chọn)

| Biến | Mô tả | Mặc định |
|------|-------|----------|
| ORACLE_HOST | Địa chỉ Oracle server | localhost |
| ORACLE_PORT | Port kết nối | 1521 |
| ORACLE_SERVICE | Tên service | FREEPDB1 |

---

## 🚀 Chạy ứng dụng

### Development

```bash
dotnet run --project UserManager
```

### Build Release

```bash
dotnet publish UserManager -c Release -o ./publish
```

### Đăng nhập

- **Username:** SYSTEM (hoặc user Oracle đã tạo)
- **Password:** YourStrongPassword123 (password của user)

---

## 📱 Tính năng

### 1. Quản lý User (👤)

| Chức năng | Mô tả |
|-----------|-------|
| Xem danh sách | Hiển thị tất cả Oracle users |
| Thêm mới | Tạo user với password, tablespace, profile |
| Sửa | Thay đổi tablespace, quota, profile |
| Khóa/Mở khóa | Lock/Unlock account |
| Đổi mật khẩu | Reset password user |
| Xóa | Drop user CASCADE |

### 2. Quản lý Role (🎭)

| Chức năng | Mô tả |
|-----------|-------|
| Xem danh sách | Hiển thị tất cả roles |
| Thêm mới | Tạo role có/không password |
| Sửa password | Thay đổi/xóa password role |
| Xem privileges | Xem quyền của role |
| Xem grantees | Xem ai được gán role |
| Xóa | Drop role |

### 3. Quản lý Profile (📊)

| Chức năng | Mô tả |
|-----------|-------|
| Xem danh sách | Hiển thị tất cả profiles |
| Thêm mới | Tạo profile với resource limits |
| Sửa | Thay đổi các limit |
| Xem users | Xem user nào dùng profile |
| Xóa | Drop profile |

### 4. Quản lý Quyền (🔑)

| Chức năng | Mô tả |
|-----------|-------|
| System Privileges | Grant/Revoke quyền hệ thống |
| Object Privileges | Grant/Revoke quyền trên table/view |
| Role Grant | Gán/Thu hồi role cho user |

### 5. Báo cáo (📈)

| Chức năng | Mô tả |
|-----------|-------|
| Thông tin user đầy đủ | Xem chi tiết user + roles + privileges + quotas |
| Xuất báo cáo | Export ra file text |

### 6. Thông tin bổ sung (📝)

| Chức năng | Mô tả |
|-----------|-------|
| USER_INFO table | Lưu thông tin cá nhân (họ tên, email, phòng ban...) |

---

## 🗃️ Stored Procedures

Ứng dụng sử dụng **25 Stored Procedures** để tương tác với database:

### User Management

| Procedure | Mô tả |
|-----------|-------|
| `SP_CREATE_USER` | Tạo user mới |
| `SP_UPDATE_USER` | Cập nhật tablespace, quota, profile |
| `SP_DELETE_USER` | Xóa user (kill sessions trước) |
| `SP_LOCK_USER` | Khóa account |
| `SP_UNLOCK_USER` | Mở khóa account |
| `SP_CHANGE_PASSWORD` | Đổi mật khẩu |

### Role Management

| Procedure | Mô tả |
|-----------|-------|
| `SP_CREATE_ROLE` | Tạo role không password |
| `SP_CREATE_ROLE_WITH_PASSWORD` | Tạo role có password |
| `SP_DELETE_ROLE` | Xóa role |
| `SP_CHANGE_ROLE_PASSWORD` | Đổi password role |
| `SP_REMOVE_ROLE_PASSWORD` | Xóa password khỏi role |

### Profile Management

| Procedure | Mô tả |
|-----------|-------|
| `SP_CREATE_PROFILE` | Tạo profile |
| `SP_UPDATE_PROFILE` | Cập nhật profile |
| `SP_DELETE_PROFILE` | Xóa profile |

### Privilege Management

| Procedure | Mô tả |
|-----------|-------|
| `SP_GRANT_SYSTEM_PRIV` | Grant system privilege |
| `SP_REVOKE_SYSTEM_PRIV` | Revoke system privilege |
| `SP_GRANT_OBJECT_PRIV` | Grant object privilege |
| `SP_REVOKE_OBJECT_PRIV` | Revoke object privilege |
| `SP_GRANT_ROLE` | Grant role cho user |
| `SP_REVOKE_ROLE` | Revoke role từ user |

### Audit

| Procedure | Mô tả |
|-----------|-------|
| `SP_WRITE_AUDIT_LOG` | Ghi log audit |

### USER_INFO

| Procedure | Mô tả |
|-----------|-------|
| `SP_INSERT_USER_INFO` | Thêm thông tin cá nhân |
| `SP_UPDATE_USER_INFO` | Cập nhật thông tin |
| `SP_DELETE_USER_INFO` | Soft delete |
| `SP_HARD_DELETE_USER_INFO` | Hard delete |

---

## 🔐 Phân quyền Oracle

### System Privileges (Quyền hệ thống)

Các quyền hệ thống phổ biến được ứng dụng hỗ trợ:

| Privilege | Mô tả |
|-----------|-------|
| `CREATE SESSION` | Cho phép đăng nhập database |
| `CREATE TABLE` | Tạo bảng |
| `CREATE VIEW` | Tạo view |
| `CREATE PROCEDURE` | Tạo procedure/function |
| `CREATE USER` | Tạo user mới |
| `DROP USER` | Xóa user |
| `ALTER USER` | Sửa user |
| `CREATE ROLE` | Tạo role |
| `DROP ANY ROLE` | Xóa role bất kỳ |
| `GRANT ANY ROLE` | Grant role bất kỳ |
| `CREATE PROFILE` | Tạo profile |
| `ALTER PROFILE` | Sửa profile |
| `DROP PROFILE` | Xóa profile |
| `SELECT ANY TABLE` | Xem dữ liệu bảng bất kỳ |
| `INSERT ANY TABLE` | Thêm dữ liệu vào bảng bất kỳ |
| `UPDATE ANY TABLE` | Sửa dữ liệu bảng bất kỳ |
| `DELETE ANY TABLE` | Xóa dữ liệu bảng bất kỳ |
| `CREATE ANY TABLE` | Tạo bảng trong schema bất kỳ |
| `DROP ANY TABLE` | Xóa bảng bất kỳ |
| `ALTER ANY TABLE` | Sửa bảng bất kỳ |
| `UNLIMITED TABLESPACE` | Không giới hạn quota |

### Object Privileges (Quyền trên đối tượng)

| Privilege | Áp dụng |
|-----------|---------|
| `SELECT` | Table, View |
| `INSERT` | Table, View |
| `UPDATE` | Table, View |
| `DELETE` | Table, View |
| `ALTER` | Table |
| `INDEX` | Table |
| `REFERENCES` | Table |
| `EXECUTE` | Procedure, Function, Package |

### Predefined Roles (Roles có sẵn)

| Role | Mô tả |
|------|-------|
| `CONNECT` | Quyền cơ bản để kết nối |
| `RESOURCE` | Quyền tạo objects |
| `DBA` | Quyền admin đầy đủ |
| `SELECT_CATALOG_ROLE` | Xem data dictionary |
| `EXECUTE_CATALOG_ROLE` | Chạy system packages |

### Admin Options

| Option | Ý nghĩa |
|--------|---------|
| `WITH ADMIN OPTION` | Cho phép người nhận grant tiếp privilege/role cho người khác |
| `WITH GRANT OPTION` | Cho phép grant tiếp object privilege |

---

## 📁 Cấu trúc project

```
UserManager/
├── UserManager/                    # Main project
│   ├── BLL/                        # Business Logic Layer
│   │   └── Services/               # Các service classes
│   │       ├── UserService.cs
│   │       ├── RoleService.cs
│   │       ├── ProfileService.cs
│   │       └── PrivilegeService.cs
│   ├── DAL/                        # Data Access Layer
│   │   ├── BaseRepository.cs       # Base class với error handling
│   │   ├── OracleConnectionFactory.cs
│   │   └── Repositories/
│   │       ├── UserRepository.cs
│   │       ├── RoleRepository.cs
│   │       ├── ProfileRepository.cs
│   │       ├── PrivilegeRepository.cs
│   │       ├── TablespaceRepository.cs
│   │       └── UserInfoRepository.cs
│   ├── GUI/                        # Presentation Layer
│   │   ├── Forms/                  # Dialog forms
│   │   │   ├── LoginForm.cs
│   │   │   ├── MainForm.cs
│   │   │   ├── UserEditForm.cs
│   │   │   ├── RoleEditForm.cs
│   │   │   ├── ProfileEditForm.cs
│   │   │   └── GrantPrivilegeForm.cs
│   │   └── UserControls/           # Tab controls
│   │       ├── UserListControl.cs
│   │       ├── RoleListControl.cs
│   │       ├── ProfileListControl.cs
│   │       ├── PrivilegeListControl.cs
│   │       ├── ObjectPrivilegeListControl.cs
│   │       ├── UserReportControl.cs
│   │       └── UserInfoListControl.cs
│   ├── Models/                     # Entity models
│   │   └── EntityModels.cs
│   ├── Common/                     # Shared utilities
│   │   ├── Constants/
│   │   │   └── PrivilegeConstants.cs
│   │   ├── Helpers/
│   │   │   ├── ConfigHelper.cs
│   │   │   ├── MessageHelper.cs
│   │   │   └── PasswordHelper.cs
│   │   └── Exceptions/
│   │       └── InsufficientPrivilegeException.cs
│   ├── appsettings.json            # Cấu hình
│   └── Program.cs                  # Entry point
├── stored_procedures.sql           # All stored procedures
├── create_userinfo_table.sql       # Script tạo bảng USER_INFO
├── .github/workflows/              # CI/CD
│   └── dotnet-desktop.yml
└── README.md                       # Documentation
```

---

## 🛠️ Troubleshooting

### Lỗi kết nối Oracle

```
ORA-12541: TNS:no listener
```
**Giải pháp:** Kiểm tra Oracle container đang chạy và port 1521 không bị chặn.

### Lỗi đăng nhập

```
ORA-01017: invalid username/password
```
**Giải pháp:** Xác minh username và password đúng.

### Lỗi quyền hạn

```
ORA-01031: insufficient privileges
```
**Giải pháp:** User đang đăng nhập không có đủ quyền. Đăng nhập bằng SYSTEM hoặc user có quyền cao hơn.

### Lỗi xóa user đang kết nối

```
ORA-01940: cannot drop a user that is currently connected
```
**Giải pháp:** Đợi vài phút hoặc kiểm tra SP_DELETE_USER đã được deploy đúng.

---

## 📄 License

MIT License - Xem file LICENSE để biết thêm chi tiết.

---

## 👤 Tác giả

- **Name:** [Your Name]
- **Email:** [your.email@example.com]
- **GitHub:** [your-github-username]

---

*Cập nhật lần cuối: 28/12/2025*
