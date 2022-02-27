import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { BackgroundService } from 'src/app/services/background.service';

@Component({
  selector: 'app-home',
  styleUrls: ['./home.component.css'],
  templateUrl: './home.component.html'
})
export class HomeComponent {
  
  constructor(private routerService: Router,
              private backgroundService: BackgroundService) {
    
  }
  
}
