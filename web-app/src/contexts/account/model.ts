interface IAccount {
	token: string,
	userId: string,
	userName: string
}

type OIAccount = IAccount | null;

export default OIAccount;