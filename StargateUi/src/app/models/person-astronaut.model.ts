export interface PersonAstronaut {
    personId: number;
    name: string;
    currentRank: string;
    currentDutyTitle: string;
    careerStartDate?: Date | null;
    careerEndDate?: Date | null;
}
