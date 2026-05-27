// employee.models.ts map CRUD nhan vien tu backend.
export interface EmployeeSummary {
  id: string;
  username: string;
  fullName: string;
  email: string;
  role: string;
  branchId: string | null;
  branchName: string;
  isActive: boolean;
}

export interface UpsertEmployeeRequest {
  username: string;
  fullName: string;
  email: string;
  role: string;
  branchId: string | null;
  password?: string | null;
  isActive: boolean;
}
