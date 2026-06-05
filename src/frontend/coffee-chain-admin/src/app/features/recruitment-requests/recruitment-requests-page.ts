import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { BranchSummary } from '../../core/models/dashboard.models';
import { CreateRecruitmentRequestPayload, RecruitmentRequest } from '../../core/models/recruitment.models';
import { AuthStore } from '../../core/services/auth.store';
import { BranchApi } from '../../core/services/branch.api';
import { RecruitmentRequestApi } from '../../core/services/recruitment-request.api';

// RecruitmentRequestsPage cho manager gui yeu cau va admin phe duyet.
@Component({
  selector: 'ccm-recruitment-requests-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './recruitment-requests-page.html',
  styleUrl: './recruitment-requests-page.css'
})
export class RecruitmentRequestsPage {
  private readonly recruitmentApi = inject(RecruitmentRequestApi);
  private readonly branchApi = inject(BranchApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly requests = signal<RecruitmentRequest[]>([]);
  protected readonly branches = signal<BranchSummary[]>([]);
  protected readonly reviewNote = signal<Record<string, string>>({});
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);
  protected readonly pageSize = signal(5);

  protected readonly form = signal<CreateRecruitmentRequestPayload>({
    branchId: this.authStore.branchId() ?? '',
    positionTitle: '',
    quantity: 1,
    reason: ''
  });

  constructor() {
    this.load();
  }

  protected readonly filteredRequests = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) {
      return this.requests();
    }

    return this.requests().filter((request) =>
      [request.branchName, request.requestedByName, request.positionTitle, request.status, request.reason]
        .join(' ')
        .toLowerCase()
        .includes(term));
  });

  protected readonly pagedRequests = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filteredRequests().slice(start, start + this.pageSize());
  });

  protected readonly totalPages = computed(() => {
    const total = this.filteredRequests().length;
    return Math.max(1, Math.ceil(total / this.pageSize()));
  });

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.branchApi.getAll().subscribe({ next: (branches) => this.branches.set(branches) });
    this.recruitmentApi.getAll().subscribe({
      next: (requests) => {
        this.requests.set(requests);
        this.page.set(1);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Khong tai duoc yeu cau tuyen dung.');
        this.loading.set(false);
      }
    });
  }

  protected submit(): void {
    const payload = this.form();
    this.error.set(this.validate(payload));
    if (this.error()) {
      return;
    }

    this.submitting.set(true);
    this.recruitmentApi.create(payload).subscribe({
      next: () => {
        this.submitting.set(false);
        this.form.set({
          branchId: this.authStore.branchId() ?? '',
          positionTitle: '',
          quantity: 1,
          reason: ''
        });
        this.load();
      },
      error: (error) => {
        this.submitting.set(false);
        this.error.set(error?.error?.message ?? 'Khong tao duoc yeu cau tuyen dung.');
      }
    });
  }

  protected review(request: RecruitmentRequest, decision: 'Approved' | 'Rejected'): void {
    this.recruitmentApi.review(request.id, {
      decision,
      adminNote: this.reviewNote()[request.id] ?? ''
    }).subscribe({
      next: () => this.load(),
      error: (error) => this.error.set(error?.error?.message ?? 'Khong review duoc yeu cau.')
    });
  }

  protected updateReviewNote(requestId: string, value: string): void {
    this.reviewNote.update((note) => ({ ...note, [requestId]: value }));
  }

  protected getReviewNote(requestId: string): string {
    return this.reviewNote()[requestId] ?? '';
  }

  protected canCreate(): boolean {
    return this.authStore.can('recruitment.write', this.form().branchId);
  }

  protected canReview(): boolean {
    return this.authStore.can('recruitment.review');
  }

  protected nextPage(): void {
    this.page.update((value) => Math.min(value + 1, this.totalPages()));
  }

  protected previousPage(): void {
    this.page.update((value) => Math.max(value - 1, 1));
  }

  protected changeSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  private validate(payload: CreateRecruitmentRequestPayload): string | null {
    if (!payload.branchId || !payload.positionTitle.trim() || !payload.reason.trim()) {
      return 'Chi nhanh, vi tri va ly do la bat buoc.';
    }

    return null;
  }
}
