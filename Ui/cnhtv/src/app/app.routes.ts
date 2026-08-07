import { Routes } from '@angular/router';
import { AppRoutes } from './domain/enums/app-routes.enum';
import { TvDevicePage } from './features/tv-device/tv-device.page';
import { HomePage } from './pages/home/home.page';
import { MainLayoutComponent } from './core/layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: AppRoutes.Home },
      { path: AppRoutes.Home, component: HomePage },
      { path: AppRoutes.TvDevices, component: TvDevicePage },
    ],
  },
  { path: '**', redirectTo: AppRoutes.Home },
];
