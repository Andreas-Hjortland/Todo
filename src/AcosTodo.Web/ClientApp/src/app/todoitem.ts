import { User } from './user';

export class TodoItem {
  id: Number | null;
  title: String;
  description: String;
  created: Date;
  completed: Date | null;
  tags: String[];
  owner: User;
}
