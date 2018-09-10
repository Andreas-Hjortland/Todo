import { Component, OnInit } from '@angular/core';
import { GlobalsService } from '../globals.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.css']
})
export class TodoComponent implements OnInit {

  constructor(private globals: GlobalsService, private router: Router) { }

  ngOnInit() {
    if (!this.globals.jwtToken) {
      this.router.navigate(['login']);
    }
  }
}
