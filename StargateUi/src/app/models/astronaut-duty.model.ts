
export interface AstronautDuty {
    id: number;
    personId: number;
    name: string;
    rank: string;
    dutyTitle: string;
    dutyStartDate: Date;
    dutyEndDate?: Date | null;
}