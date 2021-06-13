import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  styleUrls: ['./app.component.css'],
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  sidebarOpened = false;

  constructor(private router: Router) {

    router.events.subscribe((val) => {
        //SI HAY CAMBIO DE URL
        (<any>$('[data-toggle="tooltip"]')).tooltip('dispose');

        //LUEGO DEL CAMBIO
        if(val instanceof NavigationEnd) {
          
        }
    });
    
  }

  toggleSidebar(expanded) {
    if (expanded) {
      $(".nav-menu-button").addClass("active");
      this.sidebarOpened = true;
    }
    else {
      $(".nav-menu-button").removeClass("active");
      this.sidebarOpened = false;
    }
  }
  
}
