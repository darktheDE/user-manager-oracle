using System.Data;

namespace UserManager.DAL.Repositories;

/// <summary>
/// Repository quản lý Tablespaces (Read-only)
/// </summary>
public class TablespaceRepository : BaseRepository
{
    /// <summary>
    /// Lấy danh sách tất cả Tablespaces
    /// </summary>
    public DataTable GetAllTablespaces()
    {
        const string sql = @"
            SELECT 
                TABLESPACE_NAME,
                BLOCK_SIZE,
                STATUS,
                CONTENTS,
                EXTENT_MANAGEMENT
            FROM DBA_TABLESPACES
            ORDER BY TABLESPACE_NAME";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy danh sách Permanent Tablespaces (cho Default Tablespace)
    /// </summary>
    public DataTable GetPermanentTablespaces()
    {
        const string sql = @"
            SELECT TABLESPACE_NAME
            FROM DBA_TABLESPACES
            WHERE CONTENTS = 'PERMANENT'
              AND STATUS = 'ONLINE'
            ORDER BY TABLESPACE_NAME";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy danh sách Temporary Tablespaces
    /// </summary>
    public DataTable GetTemporaryTablespaces()
    {
        const string sql = @"
            SELECT TABLESPACE_NAME
            FROM DBA_TABLESPACES
            WHERE CONTENTS = 'TEMPORARY'
              AND STATUS = 'ONLINE'
            ORDER BY TABLESPACE_NAME";

        return ExecuteQuery(sql);
    }

    /// <summary>
    /// Lấy thông tin dung lượng Tablespace
    /// </summary>
    public DataTable GetTablespaceUsage()
    {
        const string sql = @"
            SELECT 
                t.TABLESPACE_NAME,
                t.STATUS,
                t.CONTENTS,
                ROUND(NVL(df.TOTAL_BYTES, 0) / 1024 / 1024, 2) AS TOTAL_MB,
                ROUND(NVL(fs.FREE_BYTES, 0) / 1024 / 1024, 2) AS FREE_MB,
                ROUND((NVL(df.TOTAL_BYTES, 0) - NVL(fs.FREE_BYTES, 0)) / 1024 / 1024, 2) AS USED_MB,
                ROUND(((NVL(df.TOTAL_BYTES, 0) - NVL(fs.FREE_BYTES, 0)) / NVL(df.TOTAL_BYTES, 1)) * 100, 2) AS USED_PCT
            FROM DBA_TABLESPACES t
            LEFT JOIN (
                SELECT TABLESPACE_NAME, SUM(BYTES) AS TOTAL_BYTES 
                FROM DBA_DATA_FILES 
                GROUP BY TABLESPACE_NAME
            ) df ON t.TABLESPACE_NAME = df.TABLESPACE_NAME
            LEFT JOIN (
                SELECT TABLESPACE_NAME, SUM(BYTES) AS FREE_BYTES 
                FROM DBA_FREE_SPACE 
                GROUP BY TABLESPACE_NAME
            ) fs ON t.TABLESPACE_NAME = fs.TABLESPACE_NAME
            WHERE t.CONTENTS != 'TEMPORARY'
            ORDER BY t.TABLESPACE_NAME";

        return ExecuteQuery(sql);
    }
}
