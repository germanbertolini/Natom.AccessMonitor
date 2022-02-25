import { Component, OnInit } from "@angular/core";
import { UserDTO } from "src/app/classes/dto/user.dto";
import { AuthService } from "src/app/services/auth.service";
import { BackgroundService } from "src/app/services/background.service";

@Component({
  selector: '[sidebar-menu]',
  templateUrl: './nav-sidebar.component.html',
  styleUrls: ['./nav-sidebar.component.css']
})
export class NavSidebarComponent implements OnInit {
  user: UserDTO;

  constructor(private _authService: AuthService,
              private backgroundService: BackgroundService) {
    
  }

  ngOnInit(): void {

    $(".option-body").slideToggle();
    (<any>$(".option-header")).click(function() {
      var option = $(this).attr("option");
      $(".option-body[option='" + option + "']").slideToggle();
    });

    this.user = this._authService.getCurrentUser();
  }

}