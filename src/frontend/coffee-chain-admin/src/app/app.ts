import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// App root chi giu router outlet, layout va auth duoc tach ra theo route.
@Component({
  selector: 'ccm-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {}
