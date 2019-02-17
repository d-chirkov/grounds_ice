export enum PaymentFrequency {
	Year = "Year",
	Month = "Month",
	Day = "Day",
	AllPeriod = "AllPeriod"
}

export enum SuretyType {
	Voucher = "Voucher",
	RealState = "RealState",
	PTS = "PTS"
}

export interface Surety {
	Types: SuretyType[],
	Others: string | null
}

export enum CreditType {
	Auto = "Auto",
	Business = "Business",
	Consumer = "Consumer",
	Hypothec = "Hypothec",
	Micro = "Micro",
	Refinancing = "Refinancing",
	Other = "Other"
}

export interface BorrowOrder {
	CreationTime: Date | null
	Amount: number
	Region: string | null
	City: string | null
	TermInDays: number
	Percent: number
	PaymentFrequency: PaymentFrequency
	Surety: Surety
	CreditType: CreditType
	Comment: string | null
}