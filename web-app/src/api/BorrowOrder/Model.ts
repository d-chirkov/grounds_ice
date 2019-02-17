export enum PaymentFrequency {
	Year = "Year",
	Month = "Month",
	Day = "Day",
	AllPeriod = "AllPeriod"
}

enum SuretyType {
	Voucher = "Voucher",
	RealState = "RealState",
	PTS = "PTS"
}

export interface Surety {
	Types: SuretyType[],
	Others: string | null
}

enum CreditType {
	Auto = "Auto",
	Business = "Business",
	Consumer = "Consumer",
	Hypothec = "Hypothec",
	Micro = "Micro",
	Refinancing = "Refinancing",
	Other = "Other"
}

export interface NewBorrowOrder {
	Amount: number
	Region: string | null
	City: string | null
	Years: number
	Months: number
	Days: number
	Percent: number
	PaymentFrequency: PaymentFrequency
	Surety: Surety
	CreditType: CreditType
	Comment: string | null
}