namespace ES.Web.Areas.EsAdmin.Helpers
{
    public class RowPermission
    {
        private readonly ApplicationDbContext _context;
        public RowPermission(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> HasRowLevelPermissionAsync(string roleId, string tableName, int rowId, string permissionType)
        {
            var permission = await _context.RowLevelPermissions
                .FirstOrDefaultAsync(p => p.RoleId == roleId
                                       && p.TableName == tableName
                                       && p.RowId == rowId
                                       && p.PermissionType == permissionType);

            return permission != null;
        }

        public async Task<IList<RowLevelPermission>> GetRowLevelPermissionsOfRoleAsync(string roleId, string tableName)
        {
            return await _context.RowLevelPermissions
                .Where(p => p.RoleId == roleId && p.TableName == tableName)
                .ToListAsync();
        }

        public void AddRowLevelPermissionsRange(IList<RowLevelPermission> rowLevelPermissions)
        {
            _context.RowLevelPermissions.AddRange(rowLevelPermissions);
            _context.SaveChanges();
        }

        public async Task<IList<RowLevelPermission>> GetRowLevelPermissionsToDeleteAsync(int rowId, string tableName)
        {
            return await _context.RowLevelPermissions
                .Where(p => p.RowId == rowId && p.TableName == tableName)
                .ToListAsync();
        }

        public void RemoveRange(IList<RowLevelPermission> existingRowPermissions)
        {
            _context.RowLevelPermissions.RemoveRange(existingRowPermissions);
            _context.SaveChanges();
        }



    }
}
