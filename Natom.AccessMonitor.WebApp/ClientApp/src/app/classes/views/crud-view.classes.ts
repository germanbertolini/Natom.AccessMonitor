import { ActivatedRoute, NavigationEnd } from "@angular/router";

export class CRUDView {
  id: string;
  mode: string;
  //model: TModelView;

  constructor(private route: ActivatedRoute){
    this.mode = this.route.snapshot.url.toString().endsWith("new")
                  ? "Nuevo"
                  : "Editar";
    this.id = this.route.snapshot.paramMap.get('id');
  }
}