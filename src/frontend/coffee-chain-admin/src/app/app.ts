import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';

import { DashboardPage } from './features/dashboard/dashboard-page';
import { DashboardStore } from './features/dashboard/dashboard.store';

// App la layout tong cua web admin, giu shell va noi cac feature page chinh.
@Component({
  selector: 'ccm-root',
  standalone: true,
  imports: [CommonModule, DashboardPage],
  providers: [DashboardStore],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly store = inject(DashboardStore);
}
