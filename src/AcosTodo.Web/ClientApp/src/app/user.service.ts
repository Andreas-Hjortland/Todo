import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';
import { MessageService } from './messages.service';
import { Observable, of } from 'rxjs';
import { GlobalsService } from './globals.service';
import { User } from './user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  authUrl = 'api/user/token';
  registerUrl = 'api/user/register';
  userlistUrl = 'api/user';

  constructor(
    private http: HttpClient,
    private messageService: MessageService,
    private globals: GlobalsService) { }

  private log(message: string) {
    this.messageService.add(`HeroService: ${message}`);
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  login(username: string, password: string) {
    return this.http.post<{ token: string }>(this.authUrl, { username, password })
      .pipe(
        tap(token => {
          localStorage.setItem('username', username);
          localStorage.setItem('jwtToken', token.token);
          this.globals.jwtToken = token.token;
        })
      );
  }

  register(username: string, password: string, email: string, about: string) {
    return this.http.post<{ token: string }>(this.registerUrl, { username, password, email, about })
      .pipe(
        tap(token => {
          localStorage.setItem('username', username);
          localStorage.setItem('jwtToken', token.token);
          this.globals.jwtToken = token.token;
        })
      );
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.userlistUrl, {
        headers: new HttpHeaders({'Authorization': `Bearer ${this.globals.jwtToken}` })
    })
      .pipe(
        tap(users => console.log('got user list', users))
      );
  }
}
