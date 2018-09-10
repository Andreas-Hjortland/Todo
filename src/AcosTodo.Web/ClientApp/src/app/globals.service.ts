import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GlobalsService {
  jwtToken: string = localStorage.getItem('jwtToken');
  username: string = localStorage.getItem('username');

  constructor() { }
}
