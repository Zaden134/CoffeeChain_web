// auth.models.ts map response JWT va user profile tu backend.
export interface LoginRequest {
  username: string;
  password: string;
}

export interface UserProfile {
  id: string;
  username: string;
  fullName: string;
  email: string;
  role: 'Administrator' | 'BranchManager' | 'Cashier' | 'WarehouseStaff' | string;
  branchId: string | null;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
  user: UserProfile;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface ResetPasswordRequest {
  newPassword: string;
}

export interface UserSession {
  id: string;
  employeeId: string;
  username: string;
  fullName: string;
  role: string;
  branchId: string | null;
  createdAtUtc: string;
  expiresAtUtc: string;
  revokedAtUtc: string | null;
  createdByIp: string | null;
  revokedByIp: string | null;
  isActive: boolean;
}

export interface UserSessionQuery {
  page: number;
  pageSize: number;
  search?: string | null;
  activeOnly?: boolean;
}
