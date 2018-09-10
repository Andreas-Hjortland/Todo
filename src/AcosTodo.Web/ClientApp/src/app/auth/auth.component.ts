import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { Router } from '@angular/router';
import { GlobalsService } from '../globals.service';

class Model {
  username: string;
  password: string;
}

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent implements OnInit {
  model: Model = {
    username: '',
    password: '',
  };

  constructor(
    private userService: UserService,
    private router: Router,
    private globals: GlobalsService) { }

  ngOnInit() {
    if (this.globals.jwtToken) {
      this.router.navigate(['todo']);
    }
  }

  onSubmit() {
    this.globals.username = this.model.username;
    this.userService.login(this.model.username, this.model.password)
      .subscribe(items => {
        console.log('items retrieved', items);
        this.router.navigate(['todo']);
      });
  }
}
