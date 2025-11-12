import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule, Router } from '@angular/router';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { HasPermissionDirective } from "../../shared/directives/has-permission.directive";

interface NavItem {
  label: string;
  icon: string;
  route?: string;
  badge?: number;
  children?: NavItem[];
  isExpanded?: boolean;
  permissionResource?: PermissionResource;
  permissionAction?: PermissionAction;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatTooltipModule,
    RouterModule,
    HasPermissionDirective
],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  @Input() isCollapsed = false; // Add this Input

  constructor(private router: Router) {}

  navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
    {
      label: 'Users',
      icon: 'people',
      children: [
        { label: 'Doctors', icon: 'local_hospital', route: '/doctor/view'},
        { label: 'Sales Persons', icon: 'person', route: '/sales-person/view' },
        { label: 'Receptionists', icon: 'person_outline', route: '/receptionist/view' },
        { label: 'Nurses', icon: 'healing', route: '/nurse/view' },
        { label: 'Admins', icon: 'admin_panel_settings', route: '/admin/view' },
      ],
      permissionResource: PermissionResource.User,
      permissionAction: PermissionAction.Read
    },
    { label: 'Patients', icon: 'personal_injury', route: '/patients/view', permissionResource: PermissionResource.Patient, permissionAction: PermissionAction.Read },
    { label: 'Leads', icon: 'assignment_ind', route: '/lead-management/view', permissionResource: PermissionResource.Lead, permissionAction: PermissionAction.Read },
    { label: 'Appointments', icon: 'event', route: '/appointment/view' },
    { label: 'Products', icon: 'inventory', route: '/product/view', permissionResource: PermissionResource.Product, permissionAction: PermissionAction.Read },

    { label: 'Pharmacies', icon: 'vaccines', route: '/pharmacy/view', permissionResource: PermissionResource.Pharmacy, permissionAction: PermissionAction.Read },

    { label: 'Price List Items', icon: 'price_check', route: '/pricelistitem/view', permissionResource: PermissionResource.ProductPharmacyPrice, permissionAction: PermissionAction.Read },
    { label: 'Commission Rates', icon: 'percent_discount', route: '/commissionrate/view', permissionResource: PermissionResource.CommissionRatePerProduct, permissionAction: PermissionAction.Read },
    { label: 'Proposals', icon: 'request_quote', route: '/proposals/view' },
    { label: 'Orders', icon: 'shopping_cart', route: '/orders/view' },
    { label: 'Order Refill', icon: 'shopping_cart', route: '/order-product-refill/view'},
    { label: 'Bulk Sms', icon: 'sms', route: '/bulk-sms' },
    { label: 'Commission Pool', icon: 'payments', route: '/commission-pool/view' },
    {label: 'Order Product Schedule Calendar', icon: 'event', route: '/order-product-schedule/view' },
    { label: 'Patient Proposal', icon: 'request_quote', route: '/patient-proposal' },
    {
      label: 'Administrative Settings',
      icon: 'settings',
      children: [
        { label: 'Coupons', icon: 'local_offer', route: '/coupons/view' },
        { label: 'Holidays', icon: 'event', route: '/holiday/add' },
        { label: 'Pharmacy Configurations', icon: 'local_offer', route: '/pharmacyconfiguration/view' },
        { label: 'LifeFile Dashboard', icon: 'local_offer', route: '/lifefiledashboard/view' },
        { label: 'Bulk Sms Approval', icon: 'sms', route: '/bulk-sms-approval' },
        { label: 'Unseen Sms', icon: 'mark_email_unread', route: '/unseen-sms' },
        { label: 'Reminder Dashboard', icon: 'local_offer', route: '/reminderdashboard/view' }
      ]
    },

  ];

  toggleSubmenu(item: NavItem) {
    if (item.children) {
      item.isExpanded = !item.isExpanded;
    }
  }

  navigateTo(route: string) {
    if (route) {
      this.router.navigate([route]);
    }
  }

  isActiveRoute(route: string): boolean {
    return this.router.url === route;
  }
}
