using Oracle.ManagedDataAccess.Client;
using System.Data;
using UserManager.Models;

namespace UserManager.DAL.Repositories;

/// <summary>
/// Repository quản lý bảng USER_INFO (Thông tin cá nhân bổ sung)
/// </summary>
public class UserInfoRepository : BaseRepository
{
    /// <summary>
    /// Lấy tất cả thông tin cá nhân
    /// </summary>
    public DataTable GetAll()
    {
        const string sql = @"
            SELECT 
                USER_INFO_ID,
                USERNAME,
                HO_TEN,
                NGAY_SINH,
                GIOI_TINH,
                DIA_CHI,
                SO_DIEN_THOAI,
                EMAIL,
                CHUC_VU,
                PHONG_BAN,
                MA_NHAN_VIEN,
                CREATED_DATE,
                IS_ACTIVE
            FROM SYSTEM.USER_INFO
            WHERE IS_ACTIVE = 1
            ORDER BY USERNAME";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy thông tin theo Username
    /// </summary>
    public UserInfoModel? GetByUsername(string username)
    {
        const string sql = @"
            SELECT 
                USER_INFO_ID,
                USERNAME,
                HO_TEN,
                NGAY_SINH,
                GIOI_TINH,
                DIA_CHI,
                SO_DIEN_THOAI,
                EMAIL,
                CHUC_VU,
                PHONG_BAN,
                MA_NHAN_VIEN,
                GHI_CHU,
                CREATED_DATE,
                IS_ACTIVE
            FROM SYSTEM.USER_INFO
            WHERE UPPER(USERNAME) = UPPER(:username)";

        var dt = ExecuteQuery(sql, new OracleParameter("username", username));
        
        if (dt.Rows.Count == 0)
            return null;

        var row = dt.Rows[0];
        return new UserInfoModel
        {
            UserInfoId = Convert.ToInt32(row["USER_INFO_ID"]),
            Username = row["USERNAME"].ToString()!,
            HoTen = row["HO_TEN"].ToString()!,
            NgaySinh = row["NGAY_SINH"] == DBNull.Value ? null : Convert.ToDateTime(row["NGAY_SINH"]),
            GioiTinh = row["GIOI_TINH"]?.ToString(),
            DiaChi = row["DIA_CHI"]?.ToString(),
            SoDienThoai = row["SO_DIEN_THOAI"]?.ToString(),
            Email = row["EMAIL"]?.ToString(),
            ChucVu = row["CHUC_VU"]?.ToString(),
            PhongBan = row["PHONG_BAN"]?.ToString(),
            MaNhanVien = row["MA_NHAN_VIEN"]?.ToString(),
            GhiChu = row["GHI_CHU"]?.ToString(),
            CreatedDate = Convert.ToDateTime(row["CREATED_DATE"]),
            IsActive = Convert.ToBoolean(row["IS_ACTIVE"])
        };
    }

    /// <summary>
    /// Thêm mới thông tin cá nhân (gọi Stored Procedure)
    /// </summary>
    public void Insert(UserInfoModel model)
    {
        ExecuteProcedure("SYSTEM.SP_INSERT_USER_INFO",
            new OracleParameter("p_username", model.Username.ToUpper()),
            new OracleParameter("p_ho_ten", model.HoTen),
            new OracleParameter("p_ngay_sinh", model.NgaySinh ?? (object)DBNull.Value),
            new OracleParameter("p_gioi_tinh", model.GioiTinh ?? (object)DBNull.Value),
            new OracleParameter("p_dia_chi", model.DiaChi ?? (object)DBNull.Value),
            new OracleParameter("p_so_dien_thoai", model.SoDienThoai ?? (object)DBNull.Value),
            new OracleParameter("p_email", model.Email ?? (object)DBNull.Value),
            new OracleParameter("p_chuc_vu", model.ChucVu ?? (object)DBNull.Value),
            new OracleParameter("p_phong_ban", model.PhongBan ?? (object)DBNull.Value),
            new OracleParameter("p_ma_nhan_vien", model.MaNhanVien ?? (object)DBNull.Value),
            new OracleParameter("p_ghi_chu", model.GhiChu ?? (object)DBNull.Value),
            new OracleParameter("p_created_by", OracleConnectionManager.Instance.CurrentUsername ?? "SYSTEM")
        );
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân (gọi Stored Procedure)
    /// </summary>
    public void Update(UserInfoModel model)
    {
        ExecuteProcedure("SYSTEM.SP_UPDATE_USER_INFO",
            new OracleParameter("p_username", model.Username),
            new OracleParameter("p_ho_ten", model.HoTen),
            new OracleParameter("p_ngay_sinh", model.NgaySinh ?? (object)DBNull.Value),
            new OracleParameter("p_gioi_tinh", model.GioiTinh ?? (object)DBNull.Value),
            new OracleParameter("p_dia_chi", model.DiaChi ?? (object)DBNull.Value),
            new OracleParameter("p_so_dien_thoai", model.SoDienThoai ?? (object)DBNull.Value),
            new OracleParameter("p_email", model.Email ?? (object)DBNull.Value),
            new OracleParameter("p_chuc_vu", model.ChucVu ?? (object)DBNull.Value),
            new OracleParameter("p_phong_ban", model.PhongBan ?? (object)DBNull.Value),
            new OracleParameter("p_ma_nhan_vien", model.MaNhanVien ?? (object)DBNull.Value),
            new OracleParameter("p_ghi_chu", model.GhiChu ?? (object)DBNull.Value),
            new OracleParameter("p_updated_by", OracleConnectionManager.Instance.CurrentUsername ?? "SYSTEM")
        );
    }

    /// <summary>
    /// Xóa mềm - set IS_ACTIVE = 0 (gọi Stored Procedure)
    /// </summary>
    public void SoftDelete(string username)
    {
        ExecuteProcedure("SYSTEM.SP_DELETE_USER_INFO",
            new OracleParameter("p_username", username),
            new OracleParameter("p_updated_by", OracleConnectionManager.Instance.CurrentUsername ?? "SYSTEM")
        );
    }

    /// <summary>
    /// Xóa cứng (gọi Stored Procedure)
    /// </summary>
    public void Delete(string username)
    {
        ExecuteProcedure("SYSTEM.SP_HARD_DELETE_USER_INFO",
            new OracleParameter("p_username", username)
        );
    }

    /// <summary>
    /// Kiểm tra tồn tại
    /// </summary>
    public bool Exists(string username)
    {
        const string sql = "SELECT COUNT(*) FROM SYSTEM.USER_INFO WHERE UPPER(USERNAME) = UPPER(:username)";
        return base.Exists(sql, new OracleParameter("username", username));
    }
}
