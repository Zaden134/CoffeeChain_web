import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';

import { DashboardState } from './dashboard.store';
import { DashboardStore } from './dashboard.store';

// DashboardPage hien thi KPI, chi nhanh noi bat va mon ban chay tu du lieu backend.
@Component({
  selector: 'ccm-dashboard-page',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.css'
})
export class DashboardPage {
  protected readonly store = inject(DashboardStore);
  protected readonly state = this.store.state;
  protected readonly activities = [
    {
      action: 'Don hang moi',
      detail: '#ORD-2026-0458 - CN Quan 1',
      time: '2 phut truoc',
      status: 'success'
    },
    {
      action: 'Canh bao ton kho',
      detail: 'Sửa tuoi sap het tai CN Hai Chau',
      time: '15 phut truoc',
      status: 'warning'
    },
    {
      action: 'Ca lam viec moi',
      detail: 'Cashier.q1 vua dang nhap POS',
      time: '35 phut truoc',
      status: 'info'
    }
  ] as const satisfies ReadonlyArray<{
    action: string;
    detail: string;
    time: string;
    status: 'success' | 'warning' | 'info';
  }>;
}
