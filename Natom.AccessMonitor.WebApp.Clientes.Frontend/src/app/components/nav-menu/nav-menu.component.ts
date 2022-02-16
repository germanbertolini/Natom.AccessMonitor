import { Component } from '@angular/core';
import { Output, EventEmitter } from '@angular/core'; 
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  @Output() toggleSidebarEvent = new EventEmitter<boolean>();
  isSidebarExpanded = false;
  isExpanded = false;

  constructor(private authService: AuthService) {
    
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  onSidebarMenuClick() {
    this.isSidebarExpanded = !this.isSidebarExpanded;
    this.toggleSidebarEvent.emit(this.isSidebarExpanded);
  }
}
