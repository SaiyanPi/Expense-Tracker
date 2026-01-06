import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

export interface BudgetNotification {
  budgetId: string;
  budgetName: string;
  totalSpent: number;
  budgetAmount: number;
  exceededAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: HubConnection;
  public budgetExceeded$ = new BehaviorSubject<BudgetNotification | null>(null);

  private apiUrl = 'https://localhost:7106/hubs/notifications'; 

  public startConnection(token: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.apiUrl, { accessTokenFactory: () => token })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR connection error:', err));

    this.registerOnServerEvents();
  }

  private registerOnServerEvents() {
    this.hubConnection.on('BudgetExceeded', (data: BudgetNotification) => {
      console.log('BudgetExceeded received:', data);
      this.budgetExceeded$.next(data);
    });
  }
}
