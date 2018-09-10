import { Component, OnInit } from '@angular/core';
import { GlobalsService } from '../globals.service';
import { Router } from '@angular/router';
import { UserService } from '../user.service';

class Model {
  username: string;
  password: string;
  email: string;
  about: string;
}


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: Model = {
    username: '',
    password: '',
    email: '',
    about: '',
  };

  constructor(
    private userService: UserService,
    private router: Router,
    private globals: GlobalsService) { }

  ngOnInit() {
  }

  onSubmit() {
    this.globals.username = this.model.username;
    this.userService.register(this.model.username, this.model.password, this.model.email, this.model.about)
      .subscribe(items => {
        console.log('items retrieved', items);
        this.router.navigate(['todo']);
      });
  }
}
