import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { API_BASE_URL } from "../config/api.config";
import { AstronautDuty } from "../models/astronaut-duty.model";
import { CreateAstronautDuty } from "../models/create-astronaut-duty.model";

@Injectable({
    providedIn: 'root'
})
export class AstronautDutyService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = inject(API_BASE_URL);
 
    getAstronautDutyByName(name: string) {
        return this.http.get<AstronautDuty[]>(`${this.baseUrl}/AstronautDuty/${name}`);
    }

    addAstronautDuty(duty: CreateAstronautDuty) {
        return this.http.post<number>(`${this.baseUrl}/AstronautDuty`, duty);
    }
}