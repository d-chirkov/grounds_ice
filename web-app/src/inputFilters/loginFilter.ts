let loginMaxLength = 20;

export function filter(login: string): string {
	let filteredLogin = login.trim();
	if (!/\S/.test(filteredLogin)) {
		return "";
	}
	if (filteredLogin.length > loginMaxLength) {
		return filteredLogin.substr(0, loginMaxLength);	
	}
	return filteredLogin;
}