import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task, CreateTask, UpdateTask, SetCompletion, PaginatedResult } from '../models/task.models';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = 'https://localhost:7271/api/tasks'; 

  constructor(private httpClient: HttpClient) { }

  getTasks(pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedResult<Task>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<Task>>(this.apiUrl, { params: params });
  }

  getTaskById(id: number): Observable<Task> {
    return this.httpClient.get<Task>(`${this.apiUrl}/${id}`);
  }

  createTask(taskData: CreateTask): Observable<Task> {
    return this.httpClient.post<Task>(this.apiUrl, taskData);
  }

  updateTask(id: number, taskData: UpdateTask): Observable<void> {
    return this.httpClient.put<void>(`${this.apiUrl}/${id}`, taskData);
  }

  setTaskCompletion(id: number, completionData: SetCompletion): Observable<void> {
    return this.httpClient.put<void>(`${this.apiUrl}/${id}/completion`, completionData);
  }

  deleteTask(id: number): Observable<void> {
    return this.httpClient.delete<void>(`${this.apiUrl}/${id}`);
  }
}