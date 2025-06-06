import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { AstronautDutyService } from '../../services/astronaut-duty.service';
import { AstronautDuty } from '../../models/astronaut-duty.model';
import { CreateAstronautDuty } from '../../models/create-astronaut-duty.model';
import { personExistsValidator } from '../../validators/person-name.validator';
import { PeopleService } from '../../services/people.service';

@Component({
  selector: 'app-astronaut-duty-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './astronaut-duty-management.component.html',
})
export class AstronautDutyManagementComponent {
  private dutyService = inject(AstronautDutyService);
  private readonly peopleService = inject(PeopleService);
  private fb = inject(FormBuilder);

  dutyForm: FormGroup = this.fb.group({
    name: [
      '',
      {
        validators: [Validators.required, Validators.minLength(1)],
        asyncValidators: [personExistsValidator(this.peopleService)],
        updateOn: 'blur',
      }

    ],
    rank: ['', [Validators.required, Validators.minLength(1)]],
    dutyTitle: ['', [Validators.required, Validators.minLength(1)]],
    dutyStartDate: ['', [Validators.required]],
  });

  loading = signal(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  duties = signal<AstronautDuty[] | null>(null);

  clearMessages() {
    this.error.set(null);
    this.successMessage.set(null);
  }

  searchDuties(name: string) {
    if (!name.trim()) {
      this.error.set('Please enter a name to search');
      return;
    }

    this.clearMessages();
    this.loading.set(true);
    this.duties.set(null);

    this.dutyService.getAstronautDutyByName(name.trim()).subscribe({
      next: (duties) => {
        this.loading.set(false);
        this.duties.set(duties);
        if (duties.length > 0) {
          this.successMessage.set(
            `Found ${duties.length} duty assignment(s) for ${name}`
          );
        } else {
          this.error.set(`No duty assignments found for ${name}`);
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(
          'Error searching for duties: ' + (err.error?.message || err.message)
        );
      },
    });
  }

  addDuty() {
    if (this.dutyForm.invalid) {
      this.dutyForm.markAllAsTouched();
      return;
    }

    const formValue = this.dutyForm.value;
    const duty: CreateAstronautDuty = {
      name: formValue.name.trim(),
      rank: formValue.rank.trim(),
      dutyTitle: formValue.dutyTitle.trim(),
      dutyStartDate: new Date(formValue.dutyStartDate),
    };

    this.clearMessages();
    this.loading.set(true);

    this.dutyService.addAstronautDuty(duty).subscribe({
      next: (dutyId) => {
        this.loading.set(false);
        this.successMessage.set(
          `Astronaut duty added successfully with ID: ${dutyId}`
        );
        this.dutyForm.reset();
        // Refresh duties if we were showing results for this astronaut
        const currentDuties = this.duties();
        if (
          currentDuties &&
          currentDuties.length > 0 &&
          currentDuties[0].name === duty.name
        ) {
          this.searchDuties(duty.name);
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(
          'Error adding astronaut duty: ' + (err.error?.message || err.message)
        );
      },
    });
  }
}
