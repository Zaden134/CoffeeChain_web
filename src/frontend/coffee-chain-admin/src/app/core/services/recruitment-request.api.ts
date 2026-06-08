import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import {
  CreateRecruitmentRequestPayload,
  RecruitmentRequest,
  ReviewRecruitmentRequestPayload
} from '../models/recruitment.models';

// RecruitmentRequestApi goi yeu cau tuyen dung va thao tac phe duyet.
@Injectable({ providedIn: 'root' })
export class RecruitmentRequestApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/recruitment-requests`;

  getAll() {
    return this.http.get<RecruitmentRequest[]>(this.baseUrl);
  }

  create(payload: CreateRecruitmentRequestPayload) {
    return this.http.post<RecruitmentRequest>(this.baseUrl, payload);
  }

  review(id: string, payload: ReviewRecruitmentRequestPayload) {
    return this.http.put<RecruitmentRequest>(`${this.baseUrl}/${id}/review`, payload);
  }
}
