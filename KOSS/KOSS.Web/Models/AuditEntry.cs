using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KOSS.Web.Models
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string Username { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
        public string AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public AuditLog ToAudit()
        {
            var audit = new AuditLog();
            audit.Username = Username;
            audit.Action = AuditType;
            audit.EntityName = TableName;
            audit.Timestamp = DateTime.Now;
            audit.EntityId = JsonConvert.SerializeObject(KeyValues);
            audit.Description = OldValues.Count == 0 ? 
                $"Added new record to {TableName}. Values: {JsonConvert.SerializeObject(NewValues)}" : 
                $"Updated {TableName}. Old: {JsonConvert.SerializeObject(OldValues)}, New: {JsonConvert.SerializeObject(NewValues)}";
            
            if (AuditType == "Delete")
            {
                audit.Description = $"Deleted record from {TableName}. Values: {JsonConvert.SerializeObject(OldValues)}";
            }
            
            return audit;
        }
    }
}
