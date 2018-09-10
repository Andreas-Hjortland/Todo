import { Component, OnInit } from '@angular/core';
import { TodoitemService } from '../todoitem.service';
import { TodoItem } from '../todoitem';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-todolist',
  templateUrl: './todolist.component.html',
  styleUrls: ['./todolist.component.css']
})
export class TodolistComponent implements OnInit {
  allItems: TodoItem[] = [];
  items: TodoItem[] = [];
  tags: string[] = [];
  filterTags: string[] = [];
  mode = 'all';

  getItems(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('userid'), 10);
    this.todoitemService.getItems(id || 0)
      .subscribe(items => {
        console.log('items retrieved', items);
        this.allItems = items;
        const allTags = items.reduce((acc, elt) => [...acc, ...elt.tags], []); // Flatmap

        this.items = items;
        this.tags = allTags
          .filter((t, i) =>
            t.length > 0 &&
            allTags.indexOf(t) === i)
          .sort();

        this.filter();
      });
  }

  hasTag(tag: string): boolean {
    return this.filterTags.indexOf(tag) > 0;
  }

  filter() {
    if (this.filterTags.length) {
      this.items = this.allItems.filter(item =>
        this.filterTags.some(t => item.tags.indexOf(t) >= 0));
    } else {
      this.items = this.allItems;
    }
    this.items = this.items.filter(s => {
      switch (this.mode) {
        case 'completed':
          return s.completed;
        case 'uncompleted':
          return !s.completed;
        case 'all':
        default:
          return true;
      }
    });
  }

  toggleTag(tag: string) {
    console.log('toggleTag', tag);
    const idx = this.filterTags.indexOf(tag);
    if (idx >= 0) {
      this.filterTags = this.filterTags.filter(s => s !== tag);
    } else {
      this.filterTags = [tag, ...this.filterTags];
    }
    this.filter();
  }

  toggleCompleted(id: number) {
    this.todoitemService.toggleItem(id)
    .subscribe(item => {
      this.getItems(); // TODO improve by inline editing the entry
    });
    console.log('toggleCompleted', id);
  }

  show(category: string) {
    this.mode = category;
    this.filter();
  }

  constructor(private route: ActivatedRoute, private todoitemService: TodoitemService) { }

  ngOnInit() {
    this.getItems();
  }
}
