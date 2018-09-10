import { Injectable } from '@angular/core';
import { TodoItem } from './todoitem';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap  } from 'rxjs/operators';
import { MessageService } from './messages.service';
import { GlobalsService } from './globals.service';
import { ComponentFactoryBoundToModule } from '@angular/core/src/linker/component_factory_resolver';

@Injectable({
  providedIn: 'root'
})
export class TodoitemService {
  private todoUrl = 'api/todo';
  private toggleUrl = this.todoUrl + '/toggleCompleted/';

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

  getItems(userId: number = 0): Observable<TodoItem[]> {
    let url = this.todoUrl;
    if (userId > 0) {
      url += `/byUser/${userId}`;
    }
    return this.http.get<TodoItem[]>(url, {
        headers: new HttpHeaders({'Authorization': `Bearer ${this.globals.jwtToken}` })
    })
      .pipe(
        tap(items => console.log('fetched items')),
        catchError(this.handleError('getItems', [])));
  }

  toggleItem(id: number): Observable<TodoItem> {
    return this.http.post<TodoItem>(`${this.toggleUrl}${id}`, undefined, {
        headers: new HttpHeaders({'Authorization': `Bearer ${this.globals.jwtToken}` })
    })
      .pipe(
        tap(item => console.log('toggled item state', item)),
        catchError(this.handleError('getItems', null)));
  }

  createItem(item: TodoItem): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.todoUrl, {
      title: item.title,
      description: item.description,
      tags: item.tags,
    }, {
        headers: new HttpHeaders({'Authorization': `Bearer ${this.globals.jwtToken}` })
    })
      .pipe(
        tap(i => console.log('added new item', i)),
        catchError(this.handleError('createItem', null)));
  }

  constructor(
    private http: HttpClient,
    private messageService: MessageService,
    private globals: GlobalsService) { }
}
