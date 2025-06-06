import { Component } from '@angular/core';
import { PersonManagementComponent } from './components/person-management/person-management.component';
import { AstronautDutyManagementComponent } from './components/astronaut-duty-management/astronaut-duty-management.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [PersonManagementComponent, AstronautDutyManagementComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'ACTS';
}
