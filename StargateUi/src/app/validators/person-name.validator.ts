import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { map, catchError, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { PeopleService } from '../services/people.service';

export function personExistsValidator(peopleService: PeopleService): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    if (!control.value || control.value.trim() === '') {
      return of(null); // Don't validate empty values
    }

    return of(control.value).pipe(
      debounceTime(300), // Wait 300ms after user stops typing
      distinctUntilChanged(), // Only emit when value actually changes
      switchMap(name => 
        peopleService.getPerson(name.trim()).pipe(
          map(person => person ? null : { personNotFound: true }),
          catchError(() => of({ validationError: true }))
        )
      )
    );
  };
}

export function personDoesNotExistValidator(peopleService: PeopleService): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    if (!control.value || control.value.trim() === '') {
      return of(null); // Don't validate empty values
    }

    return of(control.value).pipe(
      debounceTime(300), // Wait 300ms after user stops typing
      distinctUntilChanged(), // Only emit when value actually changes
      switchMap(name => 
        peopleService.getPerson(name.trim()).pipe(
          map(person => person ? { personAlreadyExists: true } : null),
          catchError(() => of({ validationError: true }))
        )
      )
    );
  };
}