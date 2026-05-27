// recruitment.models.ts map luong yeu cau tuyen dung tu backend.
export interface RecruitmentRequest {
  id: string;
  branchId: string;
  branchName: string;
  requestedByEmployeeId: string;
  requestedByName: string;
  positionTitle: string;
  quantity: number;
  reason: string;
  status: 'Pending' | 'Approved' | 'Rejected' | string;
  adminNote: string | null;
  createdAtUtc: string;
  reviewedAtUtc: string | null;
}

export interface CreateRecruitmentRequestPayload {
  branchId: string;
  positionTitle: string;
  quantity: number;
  reason: string;
}

export interface ReviewRecruitmentRequestPayload {
  decision: 'Approved' | 'Rejected';
  adminNote?: string | null;
}
