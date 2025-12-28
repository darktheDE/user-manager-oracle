using Oracle.ManagedDataAccess.Client;
using System.Data;
using UserManager.Common.Exceptions;

namespace UserManager.DAL;

/// <summary>
/// Base class cho tất cả Repository, chứa các phương thức truy vấn cơ bản
/// </summary>
public abstract class BaseRepository
{
    protected readonly OracleConnectionManager _connectionManager;

    protected BaseRepository()
    {
        _connectionManager = OracleConnectionManager.Instance;
    }

    /// <summary>
    /// Lấy kết nối hiện tại
    /// </summary>
    protected OracleConnection GetConnection()
    {
        return _connectionManager.GetConnection();
    }

    /// <summary>
    /// Xử lý lỗi Oracle và chuyển thành thông báo thân thiện
    /// </summary>
    protected Exception HandleOracleException(OracleException ex)
    {
        // ORA-00942: table or view does not exist (thiếu quyền SELECT trên DBA views)
        // ORA-01031: insufficient privileges
        // ORA-00604: error occurred at recursive SQL level (thường do thiếu quyền)
        // ORA-01720: grant option does not exist
        // ORA-01749: you may not GRANT/REVOKE privileges to/from yourself

        return ex.Number switch
        {
            942 => new InsufficientPrivilegeException(
                "Bạn không có đủ quyền hạn để truy cập chức năng này. Vui lòng liên hệ quản trị viên.", ex),
            1031 => new InsufficientPrivilegeException(
                "Bạn không có đủ quyền hạn để thực hiện thao tác này.", ex),
            604 => new InsufficientPrivilegeException(
                "Bạn không có đủ quyền hạn. Vui lòng đăng nhập bằng tài khoản có quyền quản trị.", ex),
            1720 => new InsufficientPrivilegeException(
                "Bạn không có quyền GRANT OPTION để cấp quyền này cho người khác.", ex),
            1749 => new InvalidOperationException(
                "Không thể GRANT/REVOKE quyền cho chính mình.", ex),
            1918 => new InvalidOperationException(
                "User không tồn tại.", ex),
            1917 => new InvalidOperationException(
                "User hoặc Role không tồn tại.", ex),
            1919 => new InvalidOperationException(
                "Role không tồn tại.", ex),
            1935 => new InvalidOperationException(
                "Thiếu tên User khi tạo User mới.", ex),
            1940 => new InvalidOperationException(
                "Không thể xóa User đang được kết nối.", ex),
            1921 => new InvalidOperationException(
                "Role đã tồn tại.", ex),
            1920 => new InvalidOperationException(
                "User đã tồn tại.", ex),
            28003 => new InvalidOperationException(
                "Mật khẩu không đáp ứng yêu cầu bảo mật của Profile.", ex),
            _ => ex // Trả về lỗi gốc nếu không xử lý được
        };
    }

    /// <summary>
    /// Thực thi query và trả về DataTable
    /// </summary>
    protected DataTable ExecuteQuery(string sql, params OracleParameter[] parameters)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new OracleCommand(sql, conn);
            
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new OracleDataAdapter(cmd);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }
        catch (OracleException ex)
        {
            throw HandleOracleException(ex);
        }
    }

    /// <summary>
    /// Thực thi query và trả về scalar value
    /// </summary>
    protected object? ExecuteScalar(string sql, params OracleParameter[] parameters)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new OracleCommand(sql, conn);
            
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteScalar();
        }
        catch (OracleException ex)
        {
            throw HandleOracleException(ex);
        }
    }

    /// <summary>
    /// Thực thi non-query (INSERT, UPDATE, DELETE, DDL)
    /// </summary>
    protected int ExecuteNonQuery(string sql, params OracleParameter[] parameters)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new OracleCommand(sql, conn);
            
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteNonQuery();
        }
        catch (OracleException ex)
        {
            throw HandleOracleException(ex);
        }
    }

    /// <summary>
    /// Thực thi DDL statement (CREATE, ALTER, DROP, GRANT, REVOKE)
    /// DDL tự động commit nên không cần transaction
    /// </summary>
    protected void ExecuteDDL(string sql)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new OracleCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
        catch (OracleException ex)
        {
            throw HandleOracleException(ex);
        }
    }

    /// <summary>
    /// Thực thi stored procedure và trả về DataTable
    /// </summary>
    protected DataTable ExecuteStoredProcedure(string procedureName, params OracleParameter[] parameters)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new OracleCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            using var adapter = new OracleDataAdapter(cmd);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }
        catch (OracleException ex)
        {
            throw HandleOracleException(ex);
        }
    }

    /// <summary>
    /// Thực thi stored procedure không trả về dữ liệu (INSERT, UPDATE, DELETE)
    /// </summary>
    protected void ExecuteProcedure(string procedureName, params OracleParameter[] parameters)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new OracleCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            cmd.ExecuteNonQuery();
        }
        catch (OracleException ex)
        {
            throw HandleOracleException(ex);
        }
    }

    /// <summary>
    /// Thực thi nhiều câu lệnh trong một transaction
    /// </summary>
    protected bool ExecuteTransaction(params string[] sqlStatements)
    {
        using var conn = GetConnection();
        using var transaction = conn.BeginTransaction();

        try
        {
            foreach (var sql in sqlStatements)
            {
                using var cmd = new OracleCommand(sql, conn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
        catch (OracleException ex)
        {
            transaction.Rollback();
            throw HandleOracleException(ex);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Kiểm tra tồn tại
    /// </summary>
    protected bool Exists(string sql, params OracleParameter[] parameters)
    {
        var result = ExecuteScalar(sql, parameters);
        return result != null && Convert.ToInt32(result) > 0;
    }
}

