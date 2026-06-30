export interface AuditLogEntry {
  id: string;
  createdAtUtc: string;
  employeeId: string | null;
  username: string | null;
  branchId: string | null;
  action: string;
  entityType: string;
  entityId: string | null;
  details: string;
  isSuccess: boolean;
}

export interface AuditLogQuery {
  page: number;
  pageSize: number;
  search?: string | null;
}
