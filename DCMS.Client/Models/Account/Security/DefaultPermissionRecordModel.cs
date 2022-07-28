using System.Collections.Generic;

namespace Wesley.Client.Models.Security
{

    public class DefaultPermissionRecordModel
    {

        public string UserRoleSystemName { get; set; }

        public IEnumerable<PermissionRecordModel> PermissionRecords { get; set; } = new List<PermissionRecordModel>();
    }
}
