import { bootstrapApplication } from '@angular/platform-browser';

import { appConfig } from './app/app.config';
import { App } from './app/app';

// main.ts la diem vao cua Angular app, bootstrap root component va config toan cuc.
bootstrapApplication(App, appConfig).catch((error: unknown) => {
  console.error('Failed to bootstrap Coffee Chain Admin.', error);
});
