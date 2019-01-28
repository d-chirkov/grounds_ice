export interface IMessage {
	header: string,
	message: string
}

export class Messager {
	static growl_: any
	
	static setGrowl(growl: any) {
		Messager.growl_ = growl;
	}
	
	static showErrors(messages: IMessage[]) {
		if (Messager.growl_ != undefined) {
			Messager.growl_.show(messages.map(v => ({severity: 'error', summary: v.header, detail: v.message})));
		}
	}
	
	static showError(header: string, message: string) {
		this.showErrors([{header, message}]);
	}
}
