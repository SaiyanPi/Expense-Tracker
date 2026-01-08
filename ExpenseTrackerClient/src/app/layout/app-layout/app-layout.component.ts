import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';
import { SignalRService } from '../../core/services/signalr.service';
import { ToastContainerComponent } from '../../shared/toast-container/toast-container.component';
import { ToastService } from '../../shared/toast.service';

@Component({
  selector: 'app-app-layout',
  imports: [RouterOutlet, NavbarComponent, ToastContainerComponent],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.css'
})
export class AppLayoutComponent {
  constructor(
  private signalRService: SignalRService,
  private toastService: ToastService
) {
  const token = localStorage.getItem('access_token');
  if (token) {
    this.signalRService.startConnection(token);

    this.signalRService.budgetExceeded$.subscribe(notification => {
      if (notification) {
        this.toastService.show(
          `You've used ${notification.percentageUsed}% of your "${notification.budgetName}" budget.\n Remaining Amount = ${notification.remainingAmount}`,
          'warning'
        );
      }
    });
  }
}

}
