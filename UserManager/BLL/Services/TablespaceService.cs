using UserManager.DAL.Repositories;
using System.Data;

namespace UserManager.BLL.Services;

/// <summary>
/// Service xử lý truy vấn Tablespaces
/// </summary>
public class TablespaceService
{
    private readonly TablespaceRepository _tablespaceRepo;

    public TablespaceService()
    {
        _tablespaceRepo = new TablespaceRepository();
    }

    /// <summary>
    /// Lấy tất cả Tablespaces
    /// </summary>
    public DataTable GetAllTablespaces()
    {
        return _tablespaceRepo.GetAllTablespaces();
    }

    /// <summary>
    /// Lấy danh sách Permanent Tablespaces (cho Default Tablespace dropdown)
    /// </summary>
    public DataTable GetPermanentTablespaces()
    {
        return _tablespaceRepo.GetPermanentTablespaces();
    }

    /// <summary>
    /// Lấy danh sách Temporary Tablespaces (cho Temp Tablespace dropdown)
    /// </summary>
    public DataTable GetTemporaryTablespaces()
    {
        return _tablespaceRepo.GetTemporaryTablespaces();
    }

    /// <summary>
    /// Lấy thông tin sử dụng Tablespace
    /// </summary>
    public DataTable GetTablespaceUsage()
    {
        return _tablespaceRepo.GetTablespaceUsage();
    }

    /// <summary>
    /// Lấy danh sách tên Permanent Tablespaces
    /// </summary>
    public List<string> GetPermanentTablespaceNames()
    {
        var dt = GetPermanentTablespaces();
        return dt.AsEnumerable()
            .Select(r => r["TABLESPACE_NAME"].ToString()!)
            .ToList();
    }

    /// <summary>
    /// Lấy danh sách tên Temporary Tablespaces
    /// </summary>
    public List<string> GetTemporaryTablespaceNames()
    {
        var dt = GetTemporaryTablespaces();
        return dt.AsEnumerable()
            .Select(r => r["TABLESPACE_NAME"].ToString()!)
            .ToList();
    }
}
