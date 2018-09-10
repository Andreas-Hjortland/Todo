import { Component, OnInit, Input } from '@angular/core';
import { TodoItem } from '../todoitem';
import { TodoitemService } from '../todoitem.service';
import { TodolistComponent } from '../todolist/todolist.component';
import { Router } from '@angular/router';

@Component({
  providers: [TodolistComponent],
  selector: 'app-additemform',
  templateUrl: './additemform.component.html',
  styleUrls: ['./additemform.component.css']
})
export class AdditemformComponent implements OnInit {
  @Input() component: TodolistComponent;

  item: TodoItem = {
    id: null,
    title: '',
    description: null,
    tags: [],
    completed: null,
    created: new Date(),
    owner: null
  };

  get tags() {
    return this.item.tags.join(', ');
  }
  set tags(value) {
    this.item.tags = value.split(',').map(s => s.trim());
  }

  submit() {
    this.todoItemService.createItem(this.item)
      .subscribe(() => {
        this.router.navigate(['']);
        setImmediate(() => {
          this.router.navigate(['/todo']);
        });
      });
  }

  constructor(private todoItemService: TodoitemService, private router: Router) { }

  ngOnInit() {
  }

}
