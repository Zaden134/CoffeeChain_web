import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AuditLogEntry } from '../../core/models/audit.models';
import { PagedResult } from '../../core/models/paged.models';
import { AuditLogApi } from '../../core/services/audit-log.api';

@Component({
  selector: 'ccm-audit-logs-page',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule],
  templateUrl: './audit-logs-page.html',
  styleUrl: './audit-logs-page.css'
})
export class AuditLogsPage {
  private readonly auditLogApi = inject(AuditLogApi);

  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);
  protected readonly pageSize = signal(20);
  protected readonly result = signal<PagedResult<AuditLogEntry> | null>(null);

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.auditLogApi.getPaged({
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.searchTerm()
    }).subscribe({
      next: (result) => {
        this.result.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Khong tai duoc audit log.');
        this.loading.set(false);
      }
    });
  }

  protected changeSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
    this.load();
  }

  protected nextPage(): void {
    const totalPages = this.result()?.totalPages ?? 1;
    this.page.update((value) => Math.min(value + 1, totalPages));
    this.load();
  }

  protected previousPage(): void {
    this.page.update((value) => Math.max(value - 1, 1));
    this.load();
  }
}
