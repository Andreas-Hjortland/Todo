import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { TodolistComponent } from './todolist/todolist.component';
import { AdditemformComponent } from './additemform/additemform.component';
import { AppRoutingModule } from './/app-routing.module';
import { TodoComponent } from './todo/todo.component';
import { AuthComponent } from './auth/auth.component';
import { RegisterComponent } from './register/register.component';
import { MessagesComponent } from './messages/messages.component';
import { UserlistComponent } from './userlist/userlist.component';

@NgModule({
  declarations: [
    AppComponent,
    TodolistComponent,
    AdditemformComponent,
    TodoComponent,
    AuthComponent,
    RegisterComponent,
    MessagesComponent,
    UserlistComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    HttpClientModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
