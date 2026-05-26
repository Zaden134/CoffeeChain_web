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
  role: 'Administrator' | 'BranchManager' | 'Cashier' | string;
  branchId: string | null;
}

export interface AuthResponse {
  accessToken: string;
  expiresAtUtc: string;
  user: UserProfile;
}
