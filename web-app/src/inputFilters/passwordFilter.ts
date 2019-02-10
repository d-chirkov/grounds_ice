let passwordMaxLength = 30;

export function filter(password: string): string {
	if (password.length > passwordMaxLength) {
		return password.substr(0, passwordMaxLength);	
	}
	return password;
}