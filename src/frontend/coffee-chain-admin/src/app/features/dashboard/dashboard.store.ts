import { inject, Injectable, signal } from '@angular/core';

import { DashboardOverview } from '../../core/models/dashboard.models';
import { DashboardApi } from '../../core/services/dashboard.api';

export interface DashboardState {
  loading: boolean;
  data: DashboardOverview | null;
  error: string | null;
}

// DashboardStore giu state nho gon cho trang tong quan, de sau nay co the doi sang ngrx neu can.
@Injectable()
export class DashboardStore {
  private readonly api = inject(DashboardApi);

  readonly state = signal<DashboardState>({
    loading: true,
    data: null,
    error: null
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.state.set({ loading: true, data: null, error: null });

    this.api.getOverview().subscribe({
      next: (data) => this.state.set({ loading: false, data, error: null }),
      error: () =>
        this.state.set({
          loading: false,
          data: null,
          error: 'Khong tai duoc dashboard. Kiem tra backend hoac bien moi truong API.'
        })
    });
  }
}
