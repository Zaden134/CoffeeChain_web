import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

// InventoryPage dung bo cuc theo mau va giu san cho inventory API sau.
@Component({
  selector: 'ccm-inventory-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inventory-page.html',
  styleUrl: './inventory-page.css'
})
export class InventoryPage {
  protected readonly inventoryItems = [
    { name: 'Coffee Beans', stock: '10 kg', reorder: '8 kg', branch: 'CN Quan 1', status: 'On track' },
    { name: 'Fresh Milk', stock: '8 litre', reorder: '15 litre', branch: 'CN Hai Chau', status: 'Low stock' },
    { name: 'Peach Syrup', stock: '3 bottle', reorder: '5 bottle', branch: 'CN Hai Chau', status: 'Low stock' },
    { name: 'Oat Milk', stock: '11 litre', reorder: '6 litre', branch: 'CN Cau Giay', status: 'On track' }
  ];
}
