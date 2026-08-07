import { AppRoutes } from '../enums/app-routes.enum';

export interface SideBarItem {
  icon: string;
  label: string;
  route: AppRoutes;
}

export const RouteRoleIconMap: SideBarItem[] = [
  { icon: 'home', label: 'home', route: AppRoutes.Home },
  { icon: 'connected_tv', label: 'tv.menu', route: AppRoutes.TvDevices },
];
