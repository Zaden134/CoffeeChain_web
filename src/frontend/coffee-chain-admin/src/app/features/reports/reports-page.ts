import { CommonModule, CurrencyPipe, PercentPipe } from '@angular/common';
import { Component } from '@angular/core';

// ReportsPage mo phong dashboard bao cao theo bo cuc mau.
@Component({
  selector: 'ccm-reports-page',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, PercentPipe],
  templateUrl: './reports-page.html',
  styleUrl: './reports-page.css'
})
export class ReportsPage {
  protected readonly reportCards = [
    { label: 'Doanh thu tuan', value: 324_500_000, kind: 'currency' },
    { label: 'Doanh thu thang', value: 1_123_000_000, kind: 'currency' },
    { label: 'Tang truong', value: 0.235, kind: 'percent' },
    { label: 'Ty le mon het hang', value: 0.032, kind: 'percent' }
  ] as const;
}
