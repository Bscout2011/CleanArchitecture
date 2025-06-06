import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { PersonAstronaut } from "../models/person-astronaut.model";
import { API_BASE_URL } from "../config/api.config";

@Injectable({
    providedIn: 'root'
})
export class PeopleService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = inject(API_BASE_URL);

    getPeople() {
        return this.http.get<PersonAstronaut[]>(`${this.baseUrl}/people`);
    }

    getPerson(name: string) {
        return this.http.get<PersonAstronaut | null>(`${this.baseUrl}/people/${name}`);
    }

    createPerson(name: string) {
        return this.http.post<number>(`${this.baseUrl}/people`, { name });
    }
}