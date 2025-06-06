import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PeopleService } from '../../services/people.service';
import { AstronautDutyService } from '../../services/astronaut-duty.service';
import { PersonAstronaut } from '../../models/person-astronaut.model';
import { AstronautDuty } from '../../models/astronaut-duty.model';
import { personDoesNotExistValidator, personExistsValidator } from '../../validators/person-name.validator';

@Component({
  selector: 'app-person-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './person-management.component.html',
})
export class PersonManagementComponent {
  private peopleService = inject(PeopleService);
  private dutyService = inject(AstronautDutyService);
  private fb = inject(FormBuilder);
  personForm: FormGroup = this.fb.group({
    name: [
      '', 
      [Validators.required, Validators.minLength(1)],
      [personDoesNotExistValidator(this.peopleService)]
    ]
  });

  searchForm: FormGroup = this.fb.group({
    name: [
      '', 
      [Validators.required, Validators.minLength(1)],
    ]
  });

  loading = signal(false);
  dutiesLoading = signal(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  searchResult = signal<PersonAstronaut | null>(null);
  allPeople = signal<PersonAstronaut[] | null>(null);
  personDuties = signal<AstronautDuty[] | null>(null);

  isMultipleResults() {
    return this.allPeople() && this.allPeople()!.length > 0;
  }
  clearMessages() {
    this.error.set(null);
    this.successMessage.set(null);
  }

  clearDuties() {
    this.personDuties.set(null);
    this.dutiesLoading.set(false);
  }

  fetchPersonDuties(name: string) {
    this.dutiesLoading.set(true);
    this.personDuties.set(null);

    this.dutyService.getAstronautDutyByName(name).subscribe({
      next: (duties) => {
        this.dutiesLoading.set(false);
        this.personDuties.set(duties);
      },
      error: (err) => {
        this.dutiesLoading.set(false);
        console.error('Error fetching duties:', err);
        // Don't set error state here as it's an optional action
      }
    });  }

  searchPerson(name?: string) {
    // If called from template without parameter, use form value
    const searchName = name || this.searchForm.get('name')?.value;
    
    if (!searchName?.trim()) {
      this.error.set('Please enter a name to search');
      return;
    }

    this.clearMessages();
    this.clearDuties();
    this.loading.set(true);
    this.searchResult.set(null);
    this.allPeople.set(null);

    this.peopleService.getPerson(searchName.trim()).subscribe({
      next: (person) => {
        this.loading.set(false);
        if (person) {
          this.searchResult.set(person);
          this.successMessage.set('Person found successfully');
        } else {
          this.error.set('Person not found');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Error searching for person: ' + (err.error?.message || err.message));
      }
    });
  }

  searchPersonFromForm() {
    if (this.searchForm.invalid) {
      this.searchForm.markAllAsTouched();
      return;
    }
    this.searchPerson();
  }
  getAllPeople() {
    this.clearMessages();
    this.clearDuties();
    this.loading.set(true);
    this.searchResult.set(null);
    this.allPeople.set(null);

    this.peopleService.getPeople().subscribe({
      next: (people) => {
        this.loading.set(false);
        this.allPeople.set(people);
        this.successMessage.set(`Found ${people.length} people`);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Error retrieving people: ' + (err.error?.message || err.message));
      }
    });
  }

  addPerson() {
    if (this.personForm.invalid) {
      this.personForm.markAllAsTouched();
      return;
    }

    const name = this.personForm.get('name')?.value.trim();
    this.clearMessages();
    this.loading.set(true);

    this.peopleService.createPerson(name).subscribe({
      next: (personId) => {
        this.loading.set(false);
        this.successMessage.set(`Person '${name}' added successfully with ID: ${personId}`);
        this.personForm.reset();
        // Refresh the people list if it was previously loaded
        if (this.allPeople()) {
          this.getAllPeople();
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Error adding person: ' + (err.error?.message || err.message));
      }
    });
  }
}
