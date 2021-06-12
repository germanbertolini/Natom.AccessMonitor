import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';

  constructor(private router: Router) {

    router.events.subscribe((val) => {
        //SI HAY CAMBIO DE URL
        (<any>$('[data-toggle="tooltip"]')).tooltip('dispose');

        //LUEGO DEL CAMBIO
        if(val instanceof NavigationEnd) {
          
        }
    });
    
  }
  
}
