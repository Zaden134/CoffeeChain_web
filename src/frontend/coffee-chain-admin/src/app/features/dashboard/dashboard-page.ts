import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, Input } from '@angular/core';

import { DashboardState } from './dashboard.store';

// DashboardPage hien thi KPI, chi nhanh noi bat va mon ban chay tu du lieu backend.
@Component({
  selector: 'ccm-dashboard-page',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.css'
})
export class DashboardPage {
  @Input({ required: true }) state!: DashboardState;
}
