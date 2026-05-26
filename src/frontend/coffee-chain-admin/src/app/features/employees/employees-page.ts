import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

// EmployeesPage hien thi danh sach nhan su theo role tu mau giao dien.
@Component({
  selector: 'ccm-employees-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './employees-page.html',
  styleUrl: './employees-page.css'
})
export class EmployeesPage {
  protected readonly employees = [
    { name: 'System Administrator', role: 'Administrator', branch: 'Toan he thong', status: 'Active' },
    { name: 'Nguyen Minh Chau', role: 'BranchManager', branch: 'CN Quan 1', status: 'Active' },
    { name: 'Tran Bao Han', role: 'Cashier', branch: 'CN Quan 1', status: 'Active' },
    { name: 'Le Thu Ha', role: 'BranchManager', branch: 'CN Hai Chau', status: 'Pending sync' }
  ];
}
